using FHSales.Classes;
using FHSales.MongoClasses;
using MahApps.Metro.Controls.Dialogs;
using MongoDB.Driver;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FHSales.views
{
    /// <summary>
    /// Interaction logic for Bank.xaml
    /// </summary>
    public partial class Bank : UserControl
    {
        public Bank()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        string recordID = "";
        MahApps.Metro.Controls.MetroWindow window;
        Banks bankToUpdate = new Banks();

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
            dgvBanks.ItemsSource = await loadDataGridDetails();
            btnUpdate.Visibility = Visibility.Hidden;
        }

        private async Task<List<Banks>> loadDataGridDetails()
        {
            conDB = new ConnectionDB();
            List<Banks> lstBanks = new List<Banks>();
            try
            {
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                var collection = db.GetCollection<Banks>("Banks");
                var filter = Builders<Banks>.Filter.And(
        Builders<Banks>.Filter.Where(p => p.isDeleted == false));
                lstBanks = collection.Find(filter).ToList();
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Caused by: " + ex.StackTrace);
            }
            return lstBanks;
        }

        private async void saveRecord()
        {
            try
            {
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                Banks bnk = new Banks();
                bnk.BankName = txtBankName.Text;
                bnk.Description = txtDescription.Text;

                var collection = db.GetCollection<Banks>("Banks");
                collection.InsertOne(bnk);
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

                bankToUpdate.BankName = txtBankName.Text;
                bankToUpdate.Description = txtDescription.Text;


                var filter = Builders<Banks>.Filter.And(
            Builders<Banks>.Filter.Where(p => p.Id == bankToUpdate.Id));
                var updte = Builders<Banks>.Update.Set("BankName", bankToUpdate.BankName)
                    .Set("Description", bankToUpdate.Description);

                var collection = db.GetCollection<Banks>("Banks");
                collection.UpdateOne(filter, updte);
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Caused by: " + ex.StackTrace);
            }
        }

        private void btnEditDirectSales_Click(object sender, RoutedEventArgs e)
        {
            bankToUpdate = dgvBanks.SelectedItem as Banks;

            if (bankToUpdate != null)
            {
                txtBankName.Text = bankToUpdate.BankName;
                txtDescription.Text = bankToUpdate.Description;
                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Visible;
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
            bool x = await checkFields();
            if (x)
            {
                updateRecord();
                await window.ShowMessageAsync("UPDATE RECORD", "Record updated successfully!");
                dgvBanks.ItemsSource = await loadDataGridDetails();
                txtBankName.Text = "";
                txtDescription.Text = "";
                recordID = "";
                btnUpdate.Visibility = Visibility.Hidden;
                btnSave.Visibility = Visibility.Visible;
            }

        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool bl = await checkFields();
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (bl)
            {
                saveRecord();
                txtBankName.Text = "";
                txtDescription.Text = "";
                dgvBanks.ItemsSource = await loadDataGridDetails();
                await window.ShowMessageAsync("SAVE RECORD", "Record saved successfully!");
            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            txtBankName.Text = "";
            txtDescription.Text = "";
            recordID = "";
            btnUpdate.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;
        }

        private async Task<bool> checkFields()
        {
            bool ifAllCorrect = false;
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (string.IsNullOrEmpty(txtBankName.Text))
            {
                await window.ShowMessageAsync("BANK NAME", "Please provide bank name.");
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


        #region MYSQLCODES
        //private List<BankModel> loadBankOnDataGrid()
        //{
        //    conDB = new ConnectionDB();
        //    BankModel bank = new BankModel();
        //    List<BankModel> lstBanks = new List<BankModel>();

        //    string queryString = "SELECT ID, bankname, description FROM dbfh.tblbank WHERE isDeleted = 0";

        //    MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

        //    while (reader.Read())
        //    {
        //        bank.ID = reader["ID"].ToString();
        //        bank.BankName = reader["bankname"].ToString();
        //        bank.Description = reader["description"].ToString();
        //        lstBanks.Add(bank);
        //        bank = new BankModel();
        //    }

        //    conDB.closeConnection();

        //    return lstBanks;
        //}

        //private void updateRecord(string strID)
        //{
        //    conDB = new ConnectionDB();
        //    string queryString = "UPDATE dbfh.tblbank SET bankname = ?, description = ? WHERE ID = ?";

        //    List<string> parameters = new List<string>();
        //    parameters.Add(txtBankName.Text);
        //    parameters.Add(txtDescription.Text);
        //    parameters.Add(strID);

        //    conDB.AddRecordToDatabase(queryString, parameters);

        //    conDB.closeConnection();
        //}

        //private void saveRecord()
        //{
        //    conDB = new ConnectionDB();

        //    string queryString = "INSERT INTO dbfh.tblbank (bankname, description, isDeleted) VALUES (?,?,0)";
        //    List<string> parameters = new List<string>();
        //    parameters.Add(txtBankName.Text);
        //    parameters.Add(txtDescription.Text);

        //    conDB.AddRecordToDatabase(queryString, parameters);

        //    conDB.closeConnection();
        //}


        #endregion


    }
}
