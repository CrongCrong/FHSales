

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
        List<Products> lstProds = new List<Products>();
        MahApps.Metro.Controls.MetroWindow window;
        DirectSalesDaily directSalesDailyToUpdate;
        DateTime dteNow = DateTime.Parse(DateTime.Now.ToShortDateString());

        DateTime dteFirstDay;


        int iTotalExpenses = 0;
        int iTotalAmount = 0;
        int iTotalBoxes = 0;
        int iTotalPads = 0;
        int iTotaljuice = 0;
        int iTotalGuavaBox = 0;
        int iTotalGuyabanoBox = 0;


        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
            var x = await window.ShowProgressAsync("LOADING", "Please wait...", false) as ProgressDialogController;

            string dtee = "1" + "/" + dteNow.Month + "/" + dteNow.Year;
            dteFirstDay = DateTime.Parse(dtee);

            if (!UserModel.isDSAdmin)
            {
                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Hidden;
                btnSearch.Visibility = Visibility.Hidden;
                btnCancel.Visibility = Visibility.Hidden;
                btnReset.Visibility = Visibility.Hidden;
                dgvDirectSales.IsEnabled = false;
            }

            btnUpdate.Visibility = Visibility.Hidden;

            cmbClients.IsTextSearchEnabled = true;
            loadProducts();
            loadPaymentModeOnCombo();
            loadCourierOnCombo();
            loadClientOnCombo();
            loadExpense(dteNow, dteFirstDay);
            dgvDirectSales.ItemsSource = await loadDataGridDetails();

            await x.CloseAsync();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private async Task<List<DirectSalesDaily>> loadDataGridDetails()
        {
            lblDateToday.Content = "RECORDED SALES FOR " + DateTime.Now.ToShortDateString();
            DateTime dteNow = DateTime.Parse(DateTime.Now.ToShortDateString());
            DateTime dteFirstDay = DateTime.Parse(DateTime.Now.ToShortDateString());
            lstDS = new List<DirectSalesDaily>();
            conDB = new ConnectionDB();
            lstProds = new List<Products>();
            try
            {
                loadProducts();
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
                        foreach (Products p in lstProds)
                        {
                            if (pp.Products != null && pp.Products.Id.Equals(p.Id))
                            {
                                p.Qty = p.Qty + Convert.ToInt32(pp.Qty);
                            }
                        }
                    }
                    iTotalAmount += Convert.ToInt32(ds.Total);
                    iTotalExpenses += Convert.ToInt32(ds.Expenses);
                    iTotalPads += Convert.ToInt32(ds.FreePads);
                    iTotaljuice += Convert.ToInt32(ds.FreeJuice);

                    ds.strDateOrdered = ds.DateOrdered.ToShortDateString();
                    ds.Client.strFullName = ds.Client.LastName + ", " + ds.Client.FirstName;
                }

                dgvProdSum.Items.Refresh();
                dgvProdSum.ItemsSource = lstProds;
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
            var x = await window.ShowProgressAsync("LOADING", "Please wait...", false) as ProgressDialogController;

            Clients cc = cmbSearchClient.SelectedItem as Clients;
            Banks bb = searchBank.SelectedItem as Banks;
            lstDS = new List<DirectSalesDaily>();
            lstProds = new List<Products>();
            try
            {
                conDB = new ConnectionDB();
                loadProducts();
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
                        foreach (Products p in lstProds)
                        {
                            if (pp.Products != null && pp.Products.Id.Equals(p.Id))
                            {
                                p.Qty = p.Qty + Convert.ToInt32(pp.Qty);
                            }
                        }
                    }

                    iTotalAmount += Convert.ToInt32(ds.Total);
                    iTotalExpenses += Convert.ToInt32(ds.Expenses);
                    iTotalPads += Convert.ToInt32(ds.FreePads);
                    iTotaljuice += Convert.ToInt32(ds.FreeJuice);

                    ds.strDateOrdered = ds.DateOrdered.ToShortDateString();
                    ds.Client.strFullName = ds.Client.LastName + ", " + ds.Client.FirstName;

                }
                await x.CloseAsync();
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

        private async void loadProducts()
        {
            try
            {
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                var collection = db.GetCollection<Products>("Products");

                var filter = Builders<Products>.Filter.And(
        Builders<Products>.Filter.Where(p => p.isDeleted == false));
                lstProds = collection.Find(filter).ToList();
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("EXCEPTION", "ERROR LOADING DATA " + ex.Message);
            }
        }

        private async void loadExpense(DateTime dNow, DateTime dFirstDay)
        {

            double sumEX = 0;
            try
            {
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                var collection = db.GetCollection<Expenses>("Expenses");

                var filter = Builders<Expenses>.Filter.And(
        Builders<Expenses>.Filter.Where(p => p.isDeleted == false),
         Builders<Expenses>.Filter.Eq("DateRecorded", dNow));

                List<Expenses> lstee = collection.Find(filter).ToList();

                foreach (Expenses exx in lstee)
                {
                    sumEX = sumEX + Convert.ToDouble(exx.ExpensesValue);
                }
                lblExpenseToday.Content = "For this day: Php " + sumEX.ToString("0.##");

                filter = Builders<Expenses>.Filter.And(
       Builders<Expenses>.Filter.Where(p => p.isDeleted == false),
        Builders<Expenses>.Filter.Lte("DateRecorded", dNow),
        Builders<Expenses>.Filter.Gte("DateRecorded", dFirstDay));

                lstee = collection.Find(filter).ToList();
                sumEX = 0;
                foreach (Expenses exx in lstee)
                {
                    sumEX = sumEX + Convert.ToDouble(exx.ExpensesValue);
                }
                lblExpenseMonth.Content = "For this Month: Php " + sumEX.ToString("0.##");
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("EXCEPTION", "ERROR LOADING DATA " + ex.Message);
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var y = await window.ShowProgressAsync("LOADING", "Please wait...", false) as ProgressDialogController;
            bool x = await checkFields();

            if (x)
            {
                saveRecord();
                await window.ShowMessageAsync("SAVE RECORD", "Record saved successfully!");
                clearFields();
                dgvDirectSales.ItemsSource = await loadDataGridDetails();
                lstProductsOrderedDS = new List<ProductsOrderedDS>();
            }
            await y.CloseAsync();
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var y = await window.ShowProgressAsync("LOADING", "Please wait...", false) as ProgressDialogController;
            bool x = await checkFields();

            if (x)
            {
                updateRecord();
                await window.ShowMessageAsync("UPDATE RECORD", "Record updated successfully!");
                clearFields();
                dgvDirectSales.ItemsSource = await loadDataGridDetails();
                lstProductsOrderedDS = new List<ProductsOrderedDS>();
            }
            await y.CloseAsync();
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
                lstProductsOrderedDS = directSalesDailyToUpdate.ProductsOrdered;
                dgvDirectSales.IsEnabled = false;
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
                lblDateToday.Content = "";
                dgvDirectSales.ItemsSource = await search();
                dgvProdSum.Items.Refresh();

                if (checkDate.IsChecked.Value)
                    loadExpense(DateTime.Parse(searchDateTo.Text), DateTime.Parse(searchDateFrom.Text));

                dgvProdSum.ItemsSource = lstProds;
            }
        }

        private async void btnReset_Click(object sender, RoutedEventArgs e)
        {
            lstProds = new List<Products>();
            checkBank.IsChecked = false;
            checkClient.IsChecked = false;
            checkDate.IsChecked = false;
            dgvDirectSales.ItemsSource = await loadDataGridDetails();
            loadExpense(dteNow, dteFirstDay);

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
            else if (cmbClients.SelectedItem == null)
            {
                await window.ShowMessageAsync("CLIENT", "Please select client.");
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
            btnUpdate.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;
            dgvDirectSales.IsEnabled = true;
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

        private void btnAdd2_Click(object sender, RoutedEventArgs e)
        {
            AddClientFH af = new AddClientFH(this);
            af.ShowDialog();
        }

        private void cmbClients_GotFocus(object sender, RoutedEventArgs e)
        {
            cmbClients.IsDropDownOpen = true;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            DailyExpenseDS de = new DailyExpenseDS(this);
            de.ShowDialog();
        }
    }
}
