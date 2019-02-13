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
    /// Interaction logic for Courier.xaml
    /// </summary>
    public partial class Courier : UserControl
    {
        public Courier()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        MahApps.Metro.Controls.MetroWindow window;
        Couriers courierToUpdate = new Couriers();

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
            dgvCourier.ItemsSource = await loadDataGridDetails();
            btnUpdate.Visibility = Visibility.Hidden;
        }

        private async Task<List<Couriers>> loadDataGridDetails()
        {
            List<Couriers> lstCouriers = new List<Couriers>();     
            conDB = new ConnectionDB();
            try
            {
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                var collection = db.GetCollection<Couriers>("Couriers");
                var filter = Builders<Couriers>.Filter.And(
        Builders<Couriers>.Filter.Where(p => p.isDeleted == false));
                lstCouriers = collection.Find(filter).ToList();
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Caused by: " + ex.StackTrace);
            }
            return lstCouriers;
        }

        private async void saveRecord()
        {
            try
            {
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                Couriers cr = new Couriers();
                cr.CourierName = txtCourierName.Text;
                cr.Description = txtDescription.Text;

                var collection = db.GetCollection<Couriers>("Couriers");
                collection.InsertOne(cr);
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

                courierToUpdate.CourierName = txtCourierName.Text;
                courierToUpdate.Description = txtDescription.Text;

                var filter = Builders<Couriers>.Filter.And(
            Builders<Couriers>.Filter.Where(p => p.Id == courierToUpdate.Id));
                var updte = Builders<Couriers>.Update.Set("CourierName", courierToUpdate.CourierName)
                    .Set("Description", courierToUpdate.Description);

                var collection = db.GetCollection<Couriers>("Couriers");
                collection.UpdateOne(filter, updte);
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Caused by: " + ex.StackTrace);
            }

        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool bl = await checkFields();
            if (bl)
            {
                saveRecord();
                await window.ShowMessageAsync("SAVE RECORD", "Record saved successfully!");
                txtCourierName.Text = "";
                txtDescription.Text = "";
                dgvCourier.ItemsSource = await loadDataGridDetails();
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            updateRecord();
            await window.ShowMessageAsync("UPDATE RECORD", "Record updated successfully!");
            dgvCourier.ItemsSource = await loadDataGridDetails();
            txtCourierName.Text = "";
            txtDescription.Text = "";
            btnUpdate.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;

        }

        private void btnEditDirectSales_Click(object sender, RoutedEventArgs e)
        {
            courierToUpdate = dgvCourier.SelectedItem as Couriers;

            if (courierToUpdate != null)
            {
                txtCourierName.Text = courierToUpdate.CourierName;
                txtDescription.Text = courierToUpdate.Description;
                
                btnUpdate.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Hidden;

            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            txtCourierName.Text = "";
            txtDescription.Text = "";
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


        #region MYSQL CODES
        //private List<CourierModel> loadDataGridDetails()
        //{
        //    conDB = new ConnectionDB();

        //    string queryString = "SELECT ID, couriername, description FROM dbfh.tblcourier WHERE isDeleted = 0";

        //    CourierModel courier = new CourierModel();
        //    List<CourierModel> lstCourier = new List<CourierModel>();

        //    MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

        //    while (reader.Read())
        //    {
        //        courier.ID = reader["ID"].ToString();
        //        courier.CourierName = reader["couriername"].ToString();
        //        courier.Description = reader["description"].ToString();
        //        lstCourier.Add(courier);
        //        courier = new CourierModel();
        //    }

        //    conDB.closeConnection();

        //    return lstCourier;
        //}

        //private void saveRecord()
        //{
        //    conDB = new ConnectionDB();

        //    string queryString = "INSERT INTO dbfh.tblcourier (couriername, description, isDeleted) VALUES (?,?,0)";
        //    List<string> parameters = new List<string>();
        //    parameters.Add(txtCourierName.Text);
        //    parameters.Add(txtDescription.Text);

        //    conDB.AddRecordToDatabase(queryString, parameters);

        //    conDB.closeConnection();
        //}

        //private void update(string strID)
        //{
        //    conDB = new ConnectionDB();

        //    string queryString = "UPDATE dbfh.tblcourier SET couriername = ?, description = ? WHERE ID = ?";
        //    List<string> parameters = new List<string>();
        //    parameters.Add(txtCourierName.Text);
        //    parameters.Add(txtDescription.Text);
        //    parameters.Add(strID);

        //    conDB.AddRecordToDatabase(queryString, parameters);
        //    conDB.closeConnection();

        //}

        #endregion
    }
}
