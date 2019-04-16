using FHSales.Classes;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FHSales.views
{
    /// <summary>
    /// Interaction logic for StocksInventory.xaml
    /// </summary>
    public partial class StocksInventory : UserControl
    {
        public StocksInventory()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        List<string> parameters;
        string queryString = "";
        MahApps.Metro.Controls.MetroWindow window;


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgvStocks.ItemsSource = loadDatagridDetails();
            loadTollpackerCombo();
        }

        private List<StocksInventoryModel> loadDatagridDetails()
        {
            conDB = new ConnectionDB();
            List<StocksInventoryModel> lstStocksInventory = new List<StocksInventoryModel>();
            StocksInventoryModel stocksInventory = new StocksInventoryModel();

            queryString = "SELECT tblstockstp.ID, datedelivered, dbfh.tblstockstp.description, tbltollpacker.description as tollpackername, " +
                "qty FROM (dbfh.tblstockstp INNER JOIN dbfh.tbltollpacker ON tblstockstp.tollpackerID = tbltollpacker.ID)" +
                "WHERE tblstockstp.isDeleted = 0 AND tbltollpacker.isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                stocksInventory.ID = reader["ID"].ToString();

                DateTime dte = DateTime.Parse(reader["datedelivered"].ToString());
                stocksInventory.DeliveryDate = dte.ToShortDateString();

                stocksInventory.StocksDescription = reader["description"].ToString();
                stocksInventory.TollpackerName = reader["tollpackername"].ToString();
                stocksInventory.Qty = reader["qty"].ToString();

                lstStocksInventory.Add(stocksInventory);
                stocksInventory = new StocksInventoryModel();
            }
            conDB.closeConnection();

            return lstStocksInventory;
        }

        private void loadTollpackerCombo()
        {
            conDB = new ConnectionDB();
            queryString = "SELECT ID, name, description FROM dbfh.tbltollpacker WHERE isDeleted = 0";
            TollPackerModel tollPacker = new TollPackerModel();

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);
            cmbTollpacker.Items.Clear();
            while (reader.Read())
            {
                tollPacker.ID = reader["ID"].ToString();
                tollPacker.TollpackerName = reader["name"].ToString();
                tollPacker.Description = reader["description"].ToString();
                cmbTollpacker.Items.Add(tollPacker);

                tollPacker = new TollPackerModel();
            }

            conDB.closeConnection();
        }

        private void saveRecord()
        {
            conDB = new ConnectionDB();
            queryString = "INSERT INTO dbfh.tblstockstp (datedelivered, tollpackerID, description, qty, isDeleted) VALUES (?,?,?,?,0)";

            parameters = new List<string>();
            DateTime sdate = DateTime.Parse(dateDelivered.Text);
            parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);
            parameters.Add(cmbTollpacker.SelectedValue.ToString());
            parameters.Add(txtItemDescription.Text);
            parameters.Add(txtQty.Text);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();

        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();
            window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
            if (x)
            {
                saveRecord();
                clearFields();
                dgvStocks.ItemsSource = loadDatagridDetails();
                await window.ShowMessageAsync("Save record", "Records successfully saved.");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private async Task<bool> checkFields()
        {
            bool ifOkay = false;
            window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (string.IsNullOrEmpty(dateDelivered.Text))
            {
                await window.ShowMessageAsync("Date", "Please select date.");
            }
            else if (cmbTollpacker.SelectedItem == null)
            {
                await window.ShowMessageAsync("Toll packer", "Please select toll packer.");
            }
            else if (string.IsNullOrEmpty(txtItemDescription.Text))
            {
                await window.ShowMessageAsync("Description", "Please input value.");
            }
            else if (string.IsNullOrEmpty(txtQty.Text))
            {
                await window.ShowMessageAsync("Quantity", "Please input value.");
            }
            else
            {
                ifOkay = true;
            }

            return ifOkay;
        }

        private List<StocksInventoryModel> getOverallStocks()
        {
            conDB = new ConnectionDB();
            List<StocksInventoryModel> lstStocksInv = new List<StocksInventoryModel>();
            StocksInventoryModel stocks = new StocksInventoryModel();

            queryString = "SELECT datedelivered, qty FROM dbfh.tblstockstp WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                stocks.DeliveryDate = reader["datedelivered"].ToString();
                stocks.Qty = reader["qty"].ToString();
                lstStocksInv.Add(stocks);
                stocks = new StocksInventoryModel();
            }
            conDB.closeConnection();

            return lstStocksInv;
        }

        //private List<StocksInventoryModel> computeRemainingStocks()
        //{

        //}

        private void clearFields()
        {
            dateDelivered.Text = "";
            cmbTollpacker.SelectedItem = null;
            txtItemDescription.Text = "";
            txtQty.Text = "";
        }

        private void btnCheckStocks_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
