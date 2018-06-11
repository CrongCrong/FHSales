using FHSales.Classes;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FHSales.views
{
    /// <summary>
    /// Interaction logic for Drugstore.xaml
    /// </summary>
    public partial class Drugstore : UserControl
    {
        public Drugstore()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        string recordID = "";

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgvDrugstore.ItemsSource = loadDataGridDetails();
            btnUpdate.Visibility = Visibility.Hidden;
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool bl = await checkFields();
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (bl)
            {
                saveRecord();
                await window.ShowMessageAsync("SAVE RECORD", "Record saved successfully!");
                txtDrugstore.Text = "";
                txtDescription.Text = "";
                txtdues.Text = "";
                dgvDrugstore.ItemsSource = loadDataGridDetails();
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
            update(recordID);
            await window.ShowMessageAsync("UPDATE RECORD", "Record updated successfully!");
            dgvDrugstore.ItemsSource = loadDataGridDetails();
            txtDrugstore.Text = "";
            txtDescription.Text = "";
            txtdues.Text = "";
            recordID = "";
            btnUpdate.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;

        }

        private void btnEditDirectSales_Click(object sender, RoutedEventArgs e)
        {
            DrugstoreModel selectedCourier = dgvDrugstore.SelectedItem as DrugstoreModel;

            if (selectedCourier != null)
            {
                txtDrugstore.Text = selectedCourier.DrugstoreName;
                txtDescription.Text = selectedCourier.Description;
                recordID = selectedCourier.ID;
                btnUpdate.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Hidden;

            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            txtDrugstore.Text = "";
            txtDescription.Text = "";
            recordID = "";
            btnUpdate.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;
        }

        private async Task<bool> checkFields()
        {
            bool ifAllCorrect = false;
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (string.IsNullOrEmpty(txtDrugstore.Text))
            {
                await window.ShowMessageAsync("DRUGSTORE NAME", "Please provide drugstore name.");
            }
            else if (string.IsNullOrEmpty(txtDescription.Text))
            {
                await window.ShowMessageAsync("DESCRIPTION", "Please provide drugstore description");
            }
            else if (string.IsNullOrEmpty(txtdues.Text))
            {
                await window.ShowMessageAsync("DUES", "Please provide days of payment due");
            }
            else
            {
                ifAllCorrect = true;
            }

            return ifAllCorrect;
        }

        private List<DrugstoreModel> loadDataGridDetails()
        {
            conDB = new ConnectionDB();

            string queryString = "SELECT ID, drugstore, description, paymentdue FROM dbfh.tbldrugstores WHERE isDeleted = 0 order by drugstore ASC";

            DrugstoreModel drugstore = new DrugstoreModel();
            List<DrugstoreModel> lstDrugstore = new List<DrugstoreModel>();

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                drugstore.ID = reader["ID"].ToString();
                drugstore.DrugstoreName = reader["drugstore"].ToString();
                drugstore.Description = reader["description"].ToString();
                drugstore.PaymentDue = reader["paymentdue"].ToString();
                lstDrugstore.Add(drugstore);
                drugstore = new DrugstoreModel();
            }

            conDB.closeConnection();

            return lstDrugstore;
        }

        private void saveRecord()
        {
            conDB = new ConnectionDB();

            string queryString = "INSERT INTO dbfh.tbldrugstores (drugstore, description, paymentdue, isDeleted) VALUES (?,?,?,0)";
            List<string> parameters = new List<string>();
            parameters.Add(txtDrugstore.Text);
            parameters.Add(txtDescription.Text);
            parameters.Add(txtdues.Text);
            conDB.AddRecordToDatabase(queryString, parameters);

            conDB.closeConnection();
        }

        private void update(string strID)
        {
            conDB = new ConnectionDB();

            string queryString = "UPDATE dbfh.tbldrugstores SET drugstore = ?, description = ?, paymentdue = ? WHERE ID = ?";
            List<string> parameters = new List<string>();
            parameters.Add(txtDrugstore.Text);
            parameters.Add(txtDescription.Text);
            parameters.Add(txtdues.Text);
            parameters.Add(strID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();

        }

    }
}
