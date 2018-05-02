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
    /// Interaction logic for SalesOffice.xaml
    /// </summary>
    public partial class SalesOffice : UserControl
    {
        public SalesOffice()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        string recordID = "";

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            btnUpdate.Visibility = Visibility.Hidden;
            dgvSalesOffice.ItemsSource = loadDataGridDetails();
        }

        private List<SalesOfficeModel> loadDataGridDetails()
        {
            conDB = new ConnectionDB();
            List<SalesOfficeModel> lstSalesOfficeModel = new List<SalesOfficeModel>();
            SalesOfficeModel som = new SalesOfficeModel();

            string queryString = "SELECT ID, officename, description FROM dbfh.tblsalesoffice WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                som.ID = reader["ID"].ToString();
                som.OfficeName = reader["officename"].ToString();
                som.Description = reader["description"].ToString();

                lstSalesOfficeModel.Add(som);
                som = new SalesOfficeModel();
            }

            conDB.closeConnection();

            return lstSalesOfficeModel;
        }

        private void saveRecord()
        {
            conDB = new ConnectionDB();

            string queryString = "INSERT INTO dbfh.tblsalesoffice (officename, description, isDeleted) VALUES (?,?,0)";

            List<string> parameters = new List<string>();

            parameters.Add(txtOfficeName.Text);
            parameters.Add(txtDescription.Text);

            conDB.AddRecordToDatabase(queryString, parameters);

            conDB.closeConnection();
        }

        private void updateRecord(string strID)
        {
            conDB = new ConnectionDB();
            string queryString = "UPDATE dbfh.tblsalesoffice SET officename = ?, description = ? WHERE ID = ?";

            List<string> parameters = new List<string>();
            parameters.Add(txtOfficeName.Text);
            parameters.Add(txtDescription.Text);
            parameters.Add(strID);

            conDB.AddRecordToDatabase(queryString, parameters);

            conDB.closeConnection();
        }

        private async Task<bool> checkFields()
        {
            bool ifAllCorrect = false;
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (string.IsNullOrEmpty(txtOfficeName.Text))
            {
                await window.ShowMessageAsync("OFFICE NAME", "Please provide office name.");
            }
            else if (string.IsNullOrEmpty(txtDescription.Text))
            {
                await window.ShowMessageAsync("DESCRIPTION", "Please provide office description");
            }
            else
            {
                ifAllCorrect = true;
            }

            return ifAllCorrect;
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (x)
            {
                saveRecord();
                txtOfficeName.Text = "";
                txtDescription.Text = "";
                dgvSalesOffice.ItemsSource = loadDataGridDetails();
                await window.ShowMessageAsync("SAVE RECORD", "Record saved successfully!");
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
            bool x = await checkFields();
            if (x)
            {
                updateRecord(recordID);
                await window.ShowMessageAsync("UPDATE RECORD", "Record updated successfully!");
                dgvSalesOffice.ItemsSource = loadDataGridDetails();
                txtOfficeName.Text = "";
                txtDescription.Text = "";
                recordID = "";
                btnUpdate.Visibility = Visibility.Hidden;
                btnSave.Visibility = Visibility.Visible;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            txtOfficeName.Text = "";
            txtDescription.Text = "";
            btnSave.Visibility = Visibility.Visible;
            btnUpdate.Visibility = Visibility.Hidden;
        }


        private void btnEditDirectSales_Click(object sender, RoutedEventArgs e)
        {
            
            SalesOfficeModel sls = dgvSalesOffice.SelectedItem as SalesOfficeModel;

            if(sls != null)
            {
                recordID = sls.ID;
                txtOfficeName.Text = sls.OfficeName;
                txtDescription.Text = sls.Description;

                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Visible;

            }

        }
    }
}
