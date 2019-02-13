using FHSales.Classes;
using FHSales.MongoClasses;
using MahApps.Metro.Controls.Dialogs;
using MongoDB.Driver;
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
        MahApps.Metro.Controls.MetroWindow window;
        SalesOffices salesOfficeToUpdate;


        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
            btnUpdate.Visibility = Visibility.Hidden;
            dgvSalesOffice.ItemsSource = await loadDataGridDetails();
        }

        private async Task<List<SalesOffices>> loadDataGridDetails()
        {
            List<SalesOffices> lstAgnts = new List<SalesOffices>();
            conDB = new ConnectionDB();
            try
            {
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                var collection = db.GetCollection<SalesOffices>("SalesOffices");
                var filter = Builders<SalesOffices>.Filter.And(
        Builders<SalesOffices>.Filter.Where(p => p.isDeleted == false));
                lstAgnts = collection.Find(filter).ToList();
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Caused by: " + ex.StackTrace);
            }
            return lstAgnts;
        }

        private async void saveRecord()
        {
            try
            {
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                SalesOffices so = new SalesOffices();
                so.OfficeName = txtOfficeName.Text;
                so.Description = txtDescription.Text;

                var collection = db.GetCollection<SalesOffices>("SalesOffices");
                collection.InsertOne(so);
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Caused by: " + ex.StackTrace);
            }
        }

        private async void updateRecord()
        {
            try
            {
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");

                salesOfficeToUpdate.OfficeName = txtOfficeName.Text;
                salesOfficeToUpdate.Description = txtDescription.Text;

                var filter = Builders<SalesOffices>.Filter.And(
            Builders<SalesOffices>.Filter.Where(p => p.Id == salesOfficeToUpdate.Id));
                var updte = Builders<SalesOffices>.Update.Set("OfficeName", salesOfficeToUpdate.OfficeName)
                    .Set("Description", salesOfficeToUpdate.Description);

                var collection = db.GetCollection<SalesOffices>("SalesOffices");
                collection.UpdateOne(filter, updte);
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Caused by: " + ex.StackTrace);
            }
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
                dgvSalesOffice.ItemsSource = await loadDataGridDetails();
                await window.ShowMessageAsync("SAVE RECORD", "Record saved successfully!");
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
            bool x = await checkFields();
            if (x)
            {
                updateRecord();
                await window.ShowMessageAsync("UPDATE RECORD", "Record updated successfully!");
                dgvSalesOffice.ItemsSource = await loadDataGridDetails();
                txtOfficeName.Text = "";
                txtDescription.Text = "";
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
            
            salesOfficeToUpdate = dgvSalesOffice.SelectedItem as SalesOffices;

            if(salesOfficeToUpdate != null)
            {
                txtOfficeName.Text = salesOfficeToUpdate.OfficeName;
                txtDescription.Text = salesOfficeToUpdate.Description;

                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Visible;

            }

        }

        #region
        //private List<SalesOfficeModel> loadDataGridDetails()
        //{
        //    conDB = new ConnectionDB();
        //    List<SalesOfficeModel> lstSalesOfficeModel = new List<SalesOfficeModel>();
        //    SalesOfficeModel som = new SalesOfficeModel();

        //    string queryString = "SELECT ID, officename, description FROM dbfh.tblsalesoffice WHERE isDeleted = 0";

        //    MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

        //    while (reader.Read())
        //    {
        //        som.ID = reader["ID"].ToString();
        //        som.OfficeName = reader["officename"].ToString();
        //        som.Description = reader["description"].ToString();

        //        lstSalesOfficeModel.Add(som);
        //        som = new SalesOfficeModel();
        //    }

        //    conDB.closeConnection();

        //    return lstSalesOfficeModel;
        //}

        //private void saveRecord()
        //{
        //    conDB = new ConnectionDB();

        //    string queryString = "INSERT INTO dbfh.tblsalesoffice (officename, description, isDeleted) VALUES (?,?,0)";

        //    List<string> parameters = new List<string>();

        //    parameters.Add(txtOfficeName.Text);
        //    parameters.Add(txtDescription.Text);

        //    conDB.AddRecordToDatabase(queryString, parameters);

        //    conDB.closeConnection();
        //}

        //private void updateRecord(string strID)
        //{
        //    conDB = new ConnectionDB();
        //    string queryString = "UPDATE dbfh.tblsalesoffice SET officename = ?, description = ? WHERE ID = ?";

        //    List<string> parameters = new List<string>();
        //    parameters.Add(txtOfficeName.Text);
        //    parameters.Add(txtDescription.Text);
        //    parameters.Add(strID);

        //    conDB.AddRecordToDatabase(queryString, parameters);

        //    conDB.closeConnection();
        //}


        #endregion
    }
}
