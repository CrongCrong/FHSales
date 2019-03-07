using FHSales.Classes;
using FHSales.MongoClasses;
using FHSales.views;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MongoDB.Driver;
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
using System.Windows.Shapes;

namespace FHSales
{
    /// <summary>
    /// Interaction logic for AddClientFH.xaml
    /// </summary>
    public partial class AddClientFH : MetroWindow
    {
        public AddClientFH()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        MahApps.Metro.Controls.MetroWindow window;
        Clients clientToUpdate;
        DirectSalesDailyView directSalesView;


        public AddClientFH(DirectSalesDailyView dsdv)
        {
            directSalesView = dsdv;
            InitializeComponent();
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
            dgvClients.ItemsSource = await loadDataGridDetails();
            btnUpdate.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;
        }


        private async Task<List<Clients>> loadDataGridDetails()
        {
            List<Clients> lstDS = new List<Clients>();
            conDB = new ConnectionDB();
            try
            {
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                var collection = db.GetCollection<Clients>("Clients");
                var filter = Builders<Clients>.Filter.And(
        Builders<Clients>.Filter.Where(p => p.isDeleted == false));
                lstDS = collection.Find(filter).ToList();

                lstDS = lstDS.OrderBy(cc => cc.LastName).ToList();
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Caused by: " + ex.StackTrace);
            }
            return lstDS;
        }

        private async void saveRecord()
        {
            try
            {
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                Clients cc = new Clients();

                cc.FirstName = txtFirstName.Text;
                cc.LastName = txtLastName.Text;
                cc.ContactNo = txtContactNo.Text;
                cc.Address = txtAddress.Text;

                var collection = db.GetCollection<Clients>("Clients");
                collection.InsertOne(cc);
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

                clientToUpdate.FirstName = txtFirstName.Text;
                clientToUpdate.LastName = txtLastName.Text;
                clientToUpdate.ContactNo = txtContactNo.Text;
                clientToUpdate.Address = txtAddress.Text;

                var filter = Builders<Clients>.Filter.And(
            Builders<Clients>.Filter.Where(p => p.Id == clientToUpdate.Id));

                var updte = Builders<Clients>.Update.Set("FirstName", clientToUpdate.FirstName)
                    .Set("LastName", clientToUpdate.LastName)
                    .Set("ContactNo", clientToUpdate.ContactNo)
                    .Set("Address", clientToUpdate.Address);

                var collection = db.GetCollection<Clients>("Clients");
                collection.UpdateOne(filter, updte);
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Caused by: " + ex.StackTrace);
            }
        }

        private async void loadClientOnCombo()
        {
            try
            {
                directSalesView.cmbClients.Items.Clear();
                directSalesView.cmbSearchClient.Items.Clear();

                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                var collection = db.GetCollection<Clients>("Clients");
                var filter = Builders<Clients>.Filter.And(
        Builders<Clients>.Filter.Where(p => p.isDeleted == false));
                List<Clients> lstPayments = collection.Find(filter).ToList();
                lstPayments = lstPayments.OrderBy(a => a.LastName).ToList();
                foreach (Clients p in lstPayments)
                {
                    p.strFullName = p.LastName + ", " + p.FirstName;
                    directSalesView.cmbClients.Items.Add(p);
                    directSalesView.cmbSearchClient.Items.Add(p);
                }
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("EXCEPTION", "ERROR LOADING DATA " + ex.Message);
            }
        }

        private void UpdateClient_Click(object sender, RoutedEventArgs e)
        {
            clientToUpdate = dgvClients.SelectedItem as Clients;

            if (clientToUpdate != null)
            {
                txtFirstName.Text = clientToUpdate.FirstName;
                txtLastName.Text = clientToUpdate.LastName;
                txtContactNo.Text = clientToUpdate.ContactNo;
                txtAddress.Text = clientToUpdate.Address;
                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Visible;
            }
        }

        private void DeleteClient_Click(object sender, RoutedEventArgs e)
        {
            Clients cc = dgvClients.SelectedItem as Clients;

            if (cc != null)
            {

            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            loadClientOnCombo();
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();

            if (x)
            {
                updateRecord();
                await window.ShowMessageAsync("UPDATE RECORD", "Record updated successfully!");
                clearFields();
                dgvClients.ItemsSource = await loadDataGridDetails();
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();

            if (x)
            {
                saveRecord();
                await window.ShowMessageAsync("SAVE RECORD", "Record saved successfully!");
                clearFields();
                dgvClients.ItemsSource = await loadDataGridDetails();
            }

        }

        private async Task<bool> checkFields()
        {
            bool ifCorrect = false;

            if (string.IsNullOrEmpty(txtFirstName.Text))
            {
                await window.ShowMessageAsync("First name", "Please input value");
            }
            else if (string.IsNullOrEmpty(txtLastName.Text))
            {
                await window.ShowMessageAsync("Last name", "Please input value");
            }
            else if (string.IsNullOrEmpty(txtContactNo.Text))
            {
                await window.ShowMessageAsync("Contact No.", "Please input value");
            }
            else if (string.IsNullOrEmpty(txtAddress.Text))
            {
                await window.ShowMessageAsync("Address", "Please input value");
            }else
            {
                ifCorrect = true;
            }
            return ifCorrect;
        }

        private void clearFields()
        {
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtContactNo.Text = "";
            txtAddress.Text = "";
            btnUpdate.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;
        }
    }
}
