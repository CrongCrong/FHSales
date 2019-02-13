

using FHSales.Classes;
using FHSales.MongoClasses;
using MahApps.Metro.Controls.Dialogs;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FHSales.views
{
    /// <summary>
    /// Interaction logic for DirectSalesDaily.xaml
    /// </summary>
    public partial class DirectSalesDailyView : UserControl
    {
        public DirectSalesDailyView()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        public List<ProductsOrderedDS> lstProductsOrderedDS = new List<ProductsOrderedDS>();
        List<DirectSalesDaily> lstDS = new List<DirectSalesDaily>();
        MahApps.Metro.Controls.MetroWindow window;
        DirectSalesDaily directSalesDailyToUpdate;

        int iTotalExpenses = 0;
        int iTotalAmount = 0;
        int iTotalBoxes = 0;
        int iTotalPads = 0;
        int iTotaljuice = 0;
        int iTotalGuavaBox = 0;
        int iTotalGuyabanoBox = 0;


        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!UserModel.isDSAdmin)
            {
                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Hidden;
                btnSearch.Visibility = Visibility.Hidden;
                btnCancel.Visibility = Visibility.Hidden;
                btnReset.Visibility = Visibility.Hidden;
                btnAddClient.Visibility = Visibility.Hidden;
                dgvDirectSales.IsEnabled = false;

            }

            btnUpdate.Visibility = Visibility.Hidden;
            window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
            cmbClients.IsTextSearchEnabled = true;

            loadPaymentModeOnCombo();
            loadCourierOnCombo();
            loadClientOnCombo();
            dgvDirectSales.ItemsSource = await loadDataGridDetails();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private async Task<List<DirectSalesDaily>> loadDataGridDetails()
        {
            DateTime dteNow = DateTime.Parse(DateTime.Now.ToShortDateString());
            DateTime dteFirstDay = DateTime.Parse(DateTime.Now.ToShortDateString());
            lstDS = new List<DirectSalesDaily>();

            conDB = new ConnectionDB();
            try
            {
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                var collection = db.GetCollection<DirectSalesDaily>("DirectSalesDaily");
                var filter = Builders<DirectSalesDaily>.Filter.And(
        Builders<DirectSalesDaily>.Filter.Where(p => p.isDeleted == false),
        Builders<DirectSalesDaily>.Filter.Gte("DateOrdered", dteNow),
         Builders<DirectSalesDaily>.Filter.Lte("DateOrdered", dteFirstDay));

                lstDS = collection.Find(filter).ToList();
                lstDS = lstDS.OrderByDescending(a => a.DateOrdered).ToList();

                iTotalAmount = 0;
                iTotalExpenses = 0;
                iTotalBoxes = 0;
                iTotalPads = 0;
                iTotaljuice = 0;
                iTotalGuavaBox = 0;
                iTotalGuyabanoBox = 0;

                foreach (DirectSalesDaily ds in lstDS)
                {
                    ds.strClientFullName = ds.Client.LastName + ", " + ds.Client.FirstName;
                    ds.strBankName = ds.Bank.Description;
                    foreach (ProductsOrderedDS pp in ds.ProductsOrdered)
                    {
                        if (pp.Products.Description.Contains("Boxes"))
                        {
                            ds.strTotalBoxes = pp.Qty;
                            iTotalBoxes += Convert.ToInt32(pp.Qty);
                        }
                        else if (pp.Products.Description.Contains("Guava"))
                        {
                            ds.strTotalGuava = pp.Qty;
                            iTotalGuavaBox += Convert.ToInt32(pp.Qty);
                        }
                        if (pp.Products.Description.Contains("Guyabano"))
                        {
                            ds.strTotalGuyabano = pp.Qty;
                            iTotalGuyabanoBox += Convert.ToInt32(pp.Qty);
                        }
                    }
                    iTotalAmount += Convert.ToInt32(ds.Total);
                    iTotalExpenses += Convert.ToInt32(ds.Expenses);
                    iTotalPads += Convert.ToInt32(ds.FreePads);
                    iTotaljuice += Convert.ToInt32(ds.FreeJuice);

                    ds.strDateOrdered = ds.DateOrdered.ToShortDateString();
                    ds.Client.strFullName = ds.Client.LastName + ", " + ds.Client.FirstName;
                }

                lblTotalBoxes.Content = iTotalBoxes;
                lblTotaljuice.Content = iTotaljuice;
                lblTotalPads.Content = iTotalPads;
                lblTotalGuavaBox.Content = iTotalGuavaBox;
                lblTotalGuyabanoBox.Content = iTotalGuyabanoBox;

            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Caused by: " + ex.StackTrace);
            }
            return lstDS;
        }

        private async void saveRecord()
        {
            Banks bank = cmbCashBank.SelectedItem as Banks;
            Couriers courier = cmbCourier.SelectedItem as Couriers;
            Clients cl = cmbClients.SelectedItem as Clients;

            try
            {
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                DirectSalesDaily ds = new DirectSalesDaily();


                DateTime dte = DateTime.Parse(deliveryDateDS.Text);
                ds.DateOrdered = DateTime.Parse(dte.ToLocalTime().ToShortDateString());

                ds.Client = cl;
                ds.Bank = bank;
                ds.Courier = courier;
                ds.Expenses = txtExpenses.Text;
                ds.Total = txtTotalPrice.Text;
                ds.Remarks = txtRemarks.Text;
                ds.FreePads = txtFreePads.Text;
                ds.FreeJuice = txtFreeJuice.Text;
                ds.TrackingNo = txtTrackingNumber.Text;
                ds.ProductsOrdered = lstProductsOrderedDS;

                var collection = db.GetCollection<DirectSalesDaily>("DirectSalesDaily");
                collection.InsertOne(ds);
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Caused by: " + ex.StackTrace);
            }
        }

        private async void updateRecord()
        {
            Clients slctdClients = cmbClients.SelectedItem as Clients;
            Banks slctdBnk = cmbCashBank.SelectedItem as Banks;
            Couriers slctdCourier = cmbCourier.SelectedItem as Couriers;

            try
            {
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");

                var filter = Builders<DirectSalesDaily>.Filter.And(
            Builders<DirectSalesDaily>.Filter.Where(p => p.Id == directSalesDailyToUpdate.Id));
                var updte = Builders<DirectSalesDaily>.Update.Set("Client", slctdClients)
                    .Set("Bank", slctdBnk)
                    .Set("Courier", slctdCourier)
                    .Set("Expenses", txtExpenses.Text)
                    .Set("Total", txtTotalPrice.Text)
                    .Set("Remarks", txtRemarks.Text)
                    .Set("TrackingNo", txtTrackingNumber.Text)
                    .Set("DateOrdered", DateTime.Parse(deliveryDateDS.Text))
                    .Set("FreePads", txtFreePads.Text)
                    .Set("FreeJuice", txtFreeJuice.Text)
                    .Set("isConsolidated", false)
                    .Set("ProductsOrdered", lstProductsOrderedDS);

                var collection = db.GetCollection<DirectSalesDaily>("DirectSalesDaily");
                collection.UpdateOne(filter, updte);
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Cause by: " + ex.StackTrace);
            }
        }

        private async Task<List<DirectSalesDaily>> search()
        {
            Clients cc = cmbSearchClient.SelectedItem as Clients;
            Banks bb = searchBank.SelectedItem as Banks;
            lstDS = new List<DirectSalesDaily>();
            try
            {
                conDB = new ConnectionDB();

                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                var collection = db.GetCollection<DirectSalesDaily>("DirectSalesDaily");

                var filter = Builders<DirectSalesDaily>.Filter.And(
        Builders<DirectSalesDaily>.Filter.Where(p => p.isDeleted == false));

                iTotalAmount = 0;
                iTotalExpenses = 0;
                iTotalBoxes = 0;
                iTotalPads = 0;
                iTotaljuice = 0;
                iTotalGuavaBox = 0;
                iTotalGuyabanoBox = 0;

                if (checkDate.IsChecked.Value)
                {
                    DateTime dteNow = (!string.IsNullOrEmpty(searchDateFrom.Text)) ? DateTime.Parse(searchDateFrom.Text) : DateTime.Now;
                    DateTime dteFirstDay = (!string.IsNullOrEmpty(searchDateTo.Text)) ? DateTime.Parse(searchDateTo.Text) : DateTime.Now;

                    filter = filter & Builders<DirectSalesDaily>.Filter.And(
          Builders<DirectSalesDaily>.Filter.Gte("DateOrdered", dteNow),
         Builders<DirectSalesDaily>.Filter.Lte("DateOrdered", dteFirstDay));
                }

                if (checkClient.IsChecked.Value)
                {
                    filter = filter & Builders<DirectSalesDaily>.Filter.And(
          Builders<DirectSalesDaily>.Filter.Eq("Client", cc));
                }

                if (checkBank.IsChecked.Value)
                {
                    filter = filter & Builders<DirectSalesDaily>.Filter.And(
          Builders<DirectSalesDaily>.Filter.Eq("Bank", bb));
                }

                lstDS = collection.Find(filter).ToList();

                foreach (DirectSalesDaily ds in lstDS)
                {
                    ds.strDateOrdered = ds.DateOrdered.ToShortDateString();
                    ds.Client.strFullName = ds.Client.LastName + ", " + ds.Client.FirstName;
                }
                lstDS = lstDS.OrderByDescending(a => a.DateOrdered).ToList();


                foreach (DirectSalesDaily ds in lstDS)
                {
                    ds.strClientFullName = ds.Client.LastName + ", " + ds.Client.FirstName;
                    ds.strBankName = ds.Bank.Description;

                    foreach (ProductsOrderedDS pp in ds.ProductsOrdered)
                    {
                        if (pp.Products.Description.Contains("Boxes"))
                        {
                            ds.strTotalBoxes = pp.Qty;
                            iTotalBoxes += Convert.ToInt32(pp.Qty);
                        }
                        else if (pp.Products.Description.Contains("Guava"))
                        {
                            ds.strTotalGuava = pp.Qty;
                            iTotalGuavaBox += Convert.ToInt32(pp.Qty);
                        }
                        if (pp.Products.Description.Contains("Guyabano"))
                        {
                            ds.strTotalGuyabano = pp.Qty;
                            iTotalGuyabanoBox += Convert.ToInt32(pp.Qty);
                        }
                    }

                    iTotalAmount += Convert.ToInt32(ds.Total);
                    iTotalExpenses += Convert.ToInt32(ds.Expenses);
                    iTotalPads += Convert.ToInt32(ds.FreePads);
                    iTotaljuice += Convert.ToInt32(ds.FreeJuice);

                    ds.strDateOrdered = ds.DateOrdered.ToShortDateString();
                    ds.Client.strFullName = ds.Client.LastName + ", " + ds.Client.FirstName;

                }
                lblTotalBoxes.Content = iTotalBoxes;
                lblTotaljuice.Content = iTotaljuice;
                lblTotalPads.Content = iTotalPads;
                lblTotalGuavaBox.Content = iTotalGuavaBox;
                lblTotalGuyabanoBox.Content = iTotalGuyabanoBox;
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Caused by: " + ex.StackTrace);
            }

            return lstDS;
        }

        private async void loadPaymentModeOnCombo()
        {
            try
            {
                cmbCashBank.Items.Clear();
                searchBank.Items.Clear();
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();

                var db = client.GetDatabase("DBFH");

                var collection = db.GetCollection<Banks>("Banks");

                var filter = Builders<Banks>.Filter.And(
        Builders<Banks>.Filter.Where(p => p.isDeleted == false));
                List<Banks> lstPayments = collection.Find(filter).ToList();
                foreach (Banks p in lstPayments)
                {
                    cmbCashBank.Items.Add(p);
                    searchBank.Items.Add(p);
                }
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("EXCEPTION", "ERROR LOADING DATA " + ex.Message);
            }
        }

        private async void loadCourierOnCombo()
        {
            try
            {
                cmbCourier.Items.Clear();

                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                var collection = db.GetCollection<Couriers>("Couriers");
                var filter = Builders<Couriers>.Filter.And(
        Builders<Couriers>.Filter.Where(p => p.isDeleted == false));
                List<Couriers> lstPayments = collection.Find(filter).ToList();
                foreach (Couriers p in lstPayments)
                {
                    cmbCourier.Items.Add(p);
                }
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("EXCEPTION", "ERROR LOADING DATA " + ex.Message);
            }
        }

        private async void loadClientOnCombo()
        {
            try
            {
                cmbClients.Items.Clear();
                cmbSearchClient.Items.Clear();

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
                    cmbClients.Items.Add(p);
                    cmbSearchClient.Items.Add(p);
                }
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("EXCEPTION", "ERROR LOADING DATA " + ex.Message);
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
                dgvDirectSales.ItemsSource = await loadDataGridDetails();
                lstProductsOrderedDS = new List<ProductsOrderedDS>();
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();

            if (x)
            {
                updateRecord();
                await window.ShowMessageAsync("UPDATE RECORD", "Record updated successfully!");
                clearFields();
                dgvDirectSales.ItemsSource = await loadDataGridDetails();
                lstProductsOrderedDS = new List<ProductsOrderedDS>();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            clearFields();
            lstProductsOrderedDS = new List<ProductsOrderedDS>();
        }

        private void EditRecord_Click(object sender, RoutedEventArgs e)
        {
            directSalesDailyToUpdate = dgvDirectSales.SelectedItem as DirectSalesDaily;

            if (directSalesDailyToUpdate != null)
            {
                deliveryDateDS.Text = directSalesDailyToUpdate.DateOrdered.ToShortDateString();
                foreach (Clients c in cmbClients.Items)
                {
                    if (c.Id.Equals(directSalesDailyToUpdate.Client.Id))
                    {
                        cmbClients.SelectedItem = c;
                    }
                }

                foreach (Banks b in cmbCashBank.Items)
                {
                    if (b.Id.Equals(directSalesDailyToUpdate.Bank.Id))
                    {
                        cmbCashBank.SelectedItem = b;
                    }
                }

                foreach (Couriers cc in cmbCourier.Items)
                {
                    if (cc.Id.Equals(directSalesDailyToUpdate.Courier.Id))
                    {
                        cmbCourier.SelectedItem = cc;
                    }
                }

                txtExpenses.Text = directSalesDailyToUpdate.Expenses;
                txtTotalPrice.Text = directSalesDailyToUpdate.Total;
                txtRemarks.Text = directSalesDailyToUpdate.Remarks;
                txtTrackingNumber.Text = directSalesDailyToUpdate.TrackingNo;
                txtFreePads.Text = directSalesDailyToUpdate.FreePads;
                txtFreeJuice.Text = directSalesDailyToUpdate.FreeJuice;
                lstProductsOrderedDS = directSalesDailyToUpdate.ProductsOrdered;
            }

            btnUpdate.Visibility = Visibility.Visible;
            btnSave.Visibility = Visibility.Hidden;
        }

        private void DeleteRecord_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (checkDate.IsChecked.Value && string.IsNullOrEmpty(searchDateFrom.Text) && string.IsNullOrEmpty(searchDateTo.Text))
            {
                await window.ShowMessageAsync("SEARCH", "Please complete value to search.");
            }
            else if (checkBank.IsChecked.Value && searchBank.SelectedItem == null)
            {
                await window.ShowMessageAsync("SEARCH", "Please complete value to search.");
            }
            else if (checkClient.IsChecked.Value && cmbSearchClient.SelectedItem == null)
            {
                await window.ShowMessageAsync("SEARCH", "Please complete value to search.");
            }
            else
            {
                dgvDirectSales.ItemsSource = await search();
            }
        }

        private async void btnReset_Click(object sender, RoutedEventArgs e)
        {
            checkBank.IsChecked = false;
            checkClient.IsChecked = false;
            checkDate.IsChecked = false;
            dgvDirectSales.ItemsSource = await loadDataGridDetails();
        }

        private void checkDate_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkDate_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void checkClient_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkClient_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBank_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBank_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void checkPaidSearch_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkPaidSearch_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void btnProducts_Click(object sender, RoutedEventArgs e)
        {
            AddProductDirectDaily addSalesDaily = new AddProductDirectDaily(this, lstProductsOrderedDS);
            addSalesDaily.ShowDialog();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ReportForm rf = new ReportForm(lstDS, iTotalAmount, iTotalExpenses, iTotaljuice, iTotalPads, iTotalGuavaBox, iTotalGuyabanoBox);
            rf.ShowDialog();
        }

        private void txtExpenses_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private async Task<bool> checkFields()
        {
            bool ifCorrect = false;

            if (string.IsNullOrEmpty(deliveryDateDS.Text))
            {
                await window.ShowMessageAsync("DATE ORDERED", "Please select date");
            }
            else if (cmbCashBank.SelectedItem == null)
            {
                await window.ShowMessageAsync("Payment Mode", "Please select value");
            }
            else if (cmbCourier.SelectedItem == null)
            {
                await window.ShowMessageAsync("Courier", "Please select value");
            }
            else if (string.IsNullOrEmpty(txtExpenses.Text))
            {
                await window.ShowMessageAsync("Expenses", "Please input value");
            }
            else if (string.IsNullOrEmpty(txtRemarks.Text))
            {
                await window.ShowMessageAsync("Remarks", "Please input value");
            }
            else if (string.IsNullOrEmpty(txtTrackingNumber.Text))
            {
                await window.ShowMessageAsync("Tracking Number", "Please input value");
            }
            else if (string.IsNullOrEmpty(txtFreePads.Text))
            {
                await window.ShowMessageAsync("Free pads", "Please input value");
            }
            else if (string.IsNullOrEmpty(txtFreeJuice.Text))
            {
                await window.ShowMessageAsync("Free juice", "Please input value");
            }
            else
            {
                ifCorrect = true;
            }

            return ifCorrect;
        }

        private void clearFields()
        {
            deliveryDateDS.Text = "";
            cmbClients.SelectedItem = null;
            cmbCashBank.SelectedItem = null;
            cmbCourier.SelectedItem = null;
            txtExpenses.Text = "0";
            txtTotalPrice.Text = "0";
            txtRemarks.Text = "";
            txtTrackingNumber.Text = "0";
            txtFreePads.Text = "0";
            txtFreeJuice.Text = "0";
            btnUpdate.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;
        }

        private void btnAddClient_Click(object sender, RoutedEventArgs e)
        {
            AddClientFH af = new AddClientFH(this);
            af.ShowDialog();
        }

        private void CheckIsNumeric(TextCompositionEventArgs e)
        {
            int result;

            if (!(int.TryParse(e.Text, out result) || e.Text == "."))
            {
                e.Handled = true;
            }
        }

        private void txtFreePads_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtFreeJuice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }
    }
}
