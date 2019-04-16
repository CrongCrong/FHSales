using FHSales.Classes;
using FHSales.MongoClasses;
using MahApps.Metro.Controls.Dialogs;
using MongoDB.Driver;
using System;
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
        MahApps.Metro.Controls.MetroWindow window;
        Drugstores drugstoreToUpdate = new Drugstores();

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
            dgvDrugstore.ItemsSource = await loadDataGridDetails();
            btnUpdate.Visibility = Visibility.Hidden;
        }

        private async Task<List<Drugstores>> loadDataGridDetails()
        {
            List<Drugstores> lstDrgs = new List<Drugstores>();
            conDB = new ConnectionDB();
            try
            {
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                var collection = db.GetCollection<Drugstores>("Drugstores");

                var filter = Builders<Drugstores>.Filter.And(
        Builders<Drugstores>.Filter.Where(p => p.isDeleted == false));
                lstDrgs = collection.Find(filter).ToList();
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Caused by: " + ex.StackTrace);
            }
            return lstDrgs;
        }

        private async void saveRecord()
        {
            try
            {
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                Drugstores ds = new Drugstores();
                ds.DrugstoreName = txtDrugstore.Text;
                ds.Description = txtDescription.Text;
                ds.Terms = txtdues.Text;
                var collection = db.GetCollection<Drugstores>("Drugstores");
                collection.InsertOne(ds);
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

                drugstoreToUpdate.DrugstoreName = txtDrugstore.Text;
                drugstoreToUpdate.Description = txtDescription.Text;
                drugstoreToUpdate.Terms = txtdues.Text;

                var filter = Builders<Drugstores>.Filter.And(
                Builders<Drugstores>.Filter.Where(p => p.Id == drugstoreToUpdate.Id));

                var updte = Builders<Drugstores>.Update.Set("DrugstoreName", drugstoreToUpdate.DrugstoreName)
                    .Set("Description", drugstoreToUpdate.Description)
                    .Set("Terms", drugstoreToUpdate.Terms);

                var collection = db.GetCollection<Drugstores>("Drugstores");
                collection.UpdateOne(filter, updte);
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Cause by: " + ex.StackTrace);
            }

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
                dgvDrugstore.ItemsSource = await loadDataGridDetails();
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            updateRecord();
            await window.ShowMessageAsync("UPDATE RECORD", "Record updated successfully!");
            dgvDrugstore.ItemsSource = await loadDataGridDetails();
            txtDrugstore.Text = "";
            txtDescription.Text = "";
            txtdues.Text = "";
            btnUpdate.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            txtDrugstore.Text = "";
            txtDescription.Text = "";
            btnUpdate.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;
            dgvDrugstore.IsEnabled = true;
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


        #region MYSQL Codes
        //private List<DrugstoreModel> loadDataGridDetails()
        //{
        //    conDB = new ConnectionDB();

        //    string queryString = "SELECT ID, drugstore, description, paymentdue FROM dbfh.tbldrugstores WHERE isDeleted = 0 order by drugstore ASC";

        //    DrugstoreModel drugstore = new DrugstoreModel();
        //    List<DrugstoreModel> lstDrugstore = new List<DrugstoreModel>();

        //    MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

        //    while (reader.Read())
        //    {
        //        drugstore.ID = reader["ID"].ToString();
        //        drugstore.DrugstoreName = reader["drugstore"].ToString();
        //        drugstore.Description = reader["description"].ToString();
        //        drugstore.PaymentDue = reader["paymentdue"].ToString();
        //        lstDrugstore.Add(drugstore);
        //        drugstore = new DrugstoreModel();
        //    }

        //    conDB.closeConnection();

        //    return lstDrugstore;
        //}

        //private void saveRecord()
        //{
        //    conDB = new ConnectionDB();

        //    string queryString = "INSERT INTO dbfh.tbldrugstores (drugstore, description, paymentdue, isDeleted) VALUES (?,?,?,0)";
        //    List<string> parameters = new List<string>();
        //    parameters.Add(txtDrugstore.Text);
        //    parameters.Add(txtDescription.Text);
        //    parameters.Add(txtdues.Text);
        //    conDB.AddRecordToDatabase(queryString, parameters);

        //    conDB.closeConnection();
        //}

        //private void update(string strID)
        //{
        //    conDB = new ConnectionDB();

        //    string queryString = "UPDATE dbfh.tbldrugstores SET drugstore = ?, description = ?, paymentdue = ? WHERE ID = ?";
        //    List<string> parameters = new List<string>();
        //    parameters.Add(txtDrugstore.Text);
        //    parameters.Add(txtDescription.Text);
        //    parameters.Add(txtdues.Text);
        //    parameters.Add(strID);

        //    conDB.AddRecordToDatabase(queryString, parameters);
        //    conDB.closeConnection();

        //}

        #endregion

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            drugstoreToUpdate = dgvDrugstore.SelectedItem as Drugstores;

            if (drugstoreToUpdate != null)
            {
                dgvDrugstore.IsEnabled = false;
                txtDescription.Text = drugstoreToUpdate.Description;
                txtDrugstore.Text = drugstoreToUpdate.DrugstoreName;
                txtdues.Text = drugstoreToUpdate.Terms;
                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Visible;
            }
        }
    }
}
