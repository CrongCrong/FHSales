using FHSales.Classes;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FHSales.views
{
    /// <summary>
    /// Interaction logic for Courier.xaml
    /// </summary>
    public partial class Courier : UserControl
    {
        public Courier()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        string recordID = "";

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgvCourier.ItemsSource = loadDataGridDetails();
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
                txtCourierName.Text = "";
                txtDescription.Text = "";
                dgvCourier.ItemsSource = loadDataGridDetails();
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
            update(recordID);
            await window.ShowMessageAsync("UPDATE RECORD", "Record updated successfully!");
            dgvCourier.ItemsSource = loadDataGridDetails();
            txtCourierName.Text = "";
            txtDescription.Text = "";
            recordID = "";
            btnUpdate.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;

        }

        private void btnEditDirectSales_Click(object sender, RoutedEventArgs e)
        {
            CourierModel selectedCourier = dgvCourier.SelectedItem as CourierModel;

            if (selectedCourier != null)
            {
                txtCourierName.Text = selectedCourier.CourierName;
                txtDescription.Text = selectedCourier.Description;
                recordID = selectedCourier.ID;
                btnUpdate.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Hidden;

            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            txtCourierName.Text = "";
            txtDescription.Text = "";
            recordID = "";
            btnUpdate.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;
        }

        private async Task<bool> checkFields()
        {
            bool ifAllCorrect = false;
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (string.IsNullOrEmpty(txtCourierName.Text))
            {
                await window.ShowMessageAsync("BANK NAME", "Please provide courier name.");
            }
            else if (string.IsNullOrEmpty(txtDescription.Text))
            {
                await window.ShowMessageAsync("DESCRIPTION", "Please provide bank description");
            }
            else
            {
                ifAllCorrect = true;
            }

            return ifAllCorrect;
        }

        private List<CourierModel> loadDataGridDetails()
        {
            conDB = new ConnectionDB();

            string queryString = "SELECT ID, couriername, description FROM dbfh.tblcourier WHERE isDeleted = 0";

            CourierModel courier = new CourierModel();
            List<CourierModel> lstCourier = new List<CourierModel>();

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                courier.ID = reader["ID"].ToString();
                courier.CourierName = reader["couriername"].ToString();
                courier.Description = reader["description"].ToString();
                lstCourier.Add(courier);
                courier = new CourierModel();
            }

            conDB.closeConnection();

            return lstCourier;
        }

        private void saveRecord()
        {
            conDB = new ConnectionDB();

            string queryString = "INSERT INTO dbfh.tblcourier (couriername, description, isDeleted) VALUES (?,?,0)";
            List<string> parameters = new List<string>();
            parameters.Add(txtCourierName.Text);
            parameters.Add(txtDescription.Text);

            conDB.AddRecordToDatabase(queryString, parameters);

            conDB.closeConnection();
        }

        private void update(string strID)
        {
            conDB = new ConnectionDB();

            string queryString = "UPDATE dbfh.tblcourier SET couriername = ?, description = ? WHERE ID = ?";
            List<string> parameters = new List<string>();
            parameters.Add(txtCourierName.Text);
            parameters.Add(txtDescription.Text);
            parameters.Add(strID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();

        }


    }
}
