
using MySql.Data.MySqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using System.Data.SqlTypes;
using Microsoft.Reporting.WinForms;
using FHSales.MongoClasses;
using MongoDB.Driver;
using FHSales.Classes;
using System.Linq;

namespace FHSales.views
{
    /// <summary>
    /// Interaction logic for PurchaseOrderWindow.xaml
    /// </summary>
    public partial class PurchaseOrderWindow : UserControl
    {
        public PurchaseOrderWindow()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        MahApps.Metro.Controls.MetroWindow window;
        PurchaseOrders purchaseOrderToUpdate;

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
            searchDateFrom.IsEnabled = false;
            searchDateTo.IsEnabled = false;
            searchProduct.IsEnabled = false;
            searchDrugstore.IsEnabled = false;
            searchDR.IsEnabled = false;
            searchPO.IsEnabled = false;
            searchSI.IsEnabled = false;
            dgvPO.ItemsSource = await loadDataGridDetails();
            loadDrugstoreOnCombo();
            loadProductsOnCombo();

            btnUpdate.Visibility = Visibility.Hidden;

            paymentDueDate.IsEnabled = false;

            if (!UserModel.isPOAdmin)
            {
                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Hidden;
                btnEdit.Visibility = Visibility.Hidden;
                btnCancel.Visibility = Visibility.Hidden;
                btnView.Visibility = Visibility.Hidden;
                btnSearch.Visibility = Visibility.Hidden;
                btnReset.Visibility = Visibility.Hidden;
                dgvPO.IsEnabled = false;
            }

        }

        private async Task<List<PurchaseOrders>> loadDataGridDetails()
        {
            List<PurchaseOrders> lstPurchaseOrd = new List<PurchaseOrders>();
            conDB = new ConnectionDB();
            try
            {
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                var collection = db.GetCollection<PurchaseOrders>("PurchaseOrders");
                var filter = Builders<PurchaseOrders>.Filter.And(
        Builders<PurchaseOrders>.Filter.Where(p => p.isDeleted == false));
                lstPurchaseOrd = collection.Find(filter).ToList();

                foreach (PurchaseOrders ds in lstPurchaseOrd)
                {
                    ds.strDeliveryDate = ds.DeliveryDate.ToShortDateString();
                    ds.strPaymentDate = ds.PaymentDate.ToShortDateString();
                    ds.strPaymentDueDate = ds.PaymentDueDate.ToShortDateString();
                }
                lstPurchaseOrd = lstPurchaseOrd.OrderByDescending(a => a.DeliveryDate).ToList();
                
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Caused by: " + ex.StackTrace);
            }
            return lstPurchaseOrd;
        }

        private async void loadDrugstoreOnCombo()
        {
            try
            {
                comboDrugstore.Items.Clear();
                searchDrugstore.Items.Clear();
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                var collection = db.GetCollection<Drugstores>("Drugstores");
                var filter = Builders<Drugstores>.Filter.And(
        Builders<Drugstores>.Filter.Where(p => p.isDeleted == false));
                List<Drugstores> lstPayments = collection.Find(filter).ToList();
                foreach (Drugstores p in lstPayments)
                {
                    searchDrugstore.Items.Add(p);
                    comboDrugstore.Items.Add(p);
                }
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("EXCEPTION", "ERROR LOADING DATA " + ex.Message);
            }
        }

        private async void loadProductsOnCombo()
        {
            try
            {
                comboProduct.Items.Clear();
                searchProduct.Items.Clear();
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                var collection = db.GetCollection<Products>("Products");
                var filter = Builders<Products>.Filter.And(
        Builders<Products>.Filter.Where(p => p.isDeleted == false));
                List<Products> lstPayments = collection.Find(filter).ToList();

                foreach (Products p in lstPayments)
                {
                    comboProduct.Items.Add(p);
                    searchProduct.Items.Add(p);
                }
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("EXCEPTION", "ERROR LOADING DATA " + ex.Message);
            }
        }

        private async void updateRecord()
        {
            Drugstores slctdDrg = comboDrugstore.SelectedItem as Drugstores;
            Products slctdPrd = comboProduct.SelectedItem as Products;
            try
            {
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");

                purchaseOrderToUpdate.PONumber = txtPO.Text;

                DateTime dte = DateTime.Parse(deliveryDate.Text);
                purchaseOrderToUpdate.DeliveryDate = DateTime.Parse(dte.ToLocalTime().ToShortDateString());

                dte = DateTime.Parse(paymentDueDate.Text);
                purchaseOrderToUpdate.PaymentDueDate = DateTime.Parse(dte.ToLocalTime().ToShortDateString());

                dte = DateTime.Parse(expiryDate.Text);
                purchaseOrderToUpdate.ExpiryDate = DateTime.Parse(dte.ToLocalTime().ToShortDateString());

                DateTime validValue;
                DateTime? testdte;
                testdte = DateTime.TryParse(paymentDate.Text, out validValue)
                    ? validValue
                    : (DateTime?)null;
                purchaseOrderToUpdate.PaymentDate = DateTime.Parse(testdte.GetValueOrDefault().ToShortDateString());

                purchaseOrderToUpdate.Qty = txtQuantity.Text;
                purchaseOrderToUpdate.Drugstore = slctdDrg;
                purchaseOrderToUpdate.Product = slctdPrd;
                purchaseOrderToUpdate.DRNumber = txtDR.Text;
                purchaseOrderToUpdate.PONumber = txtPO.Text;
                purchaseOrderToUpdate.SINumber = txtSI.Text;
                purchaseOrderToUpdate.Amount = Convert.ToDouble(txtAmount.Text);
                purchaseOrderToUpdate.isPaid = (chkPaid.IsChecked.Value) ? true : false;
                purchaseOrderToUpdate.BatchNo = txtBatchNo.Text;

                var filter = Builders<PurchaseOrders>.Filter.And(
            Builders<PurchaseOrders>.Filter.Where(p => p.Id == purchaseOrderToUpdate.Id));
                var updte = Builders<PurchaseOrders>.Update.Set("PONumber", purchaseOrderToUpdate.PONumber)
                    .Set("SINumber", purchaseOrderToUpdate.SINumber)
                    .Set("DRNumber", purchaseOrderToUpdate.DRNumber)
                    .Set("PaymentDueDate", purchaseOrderToUpdate.PaymentDueDate)
                    .Set("PaymentDate", purchaseOrderToUpdate.PaymentDate)
                    .Set("DeliveryDate", purchaseOrderToUpdate.DeliveryDate)
                    .Set("Qty", purchaseOrderToUpdate.Qty)
                    .Set("Drugstore", purchaseOrderToUpdate.Drugstore)
                    .Set("Product", purchaseOrderToUpdate.Product)
                    .Set("Amount", purchaseOrderToUpdate.Amount)
                    .Set("isPaid", purchaseOrderToUpdate.isPaid)
                    .Set("ExpiryDate", purchaseOrderToUpdate.ExpiryDate)
                    .Set("BatchNo", purchaseOrderToUpdate.BatchNo);

                var collection = db.GetCollection<PurchaseOrders>("PurchaseOrders");
                collection.UpdateOne(filter, updte);
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Cause by: " + ex.StackTrace);
            }
        }

        private async void saveRecord()
        {
            Drugstores slctdDrg = comboDrugstore.SelectedItem as Drugstores;
            Products slctdPrd = comboProduct.SelectedItem as Products;
            PurchaseOrders purchaseOrder = new PurchaseOrders();

            try
            {
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");

                purchaseOrder.PONumber = txtPO.Text;

                DateTime dte = DateTime.Parse(deliveryDate.Text);
                purchaseOrder.DeliveryDate = DateTime.Parse(dte.ToLocalTime().ToShortDateString());

                dte = DateTime.Parse(paymentDueDate.Text);
                purchaseOrder.PaymentDueDate = DateTime.Parse(dte.ToLocalTime().ToShortDateString());

                dte = DateTime.Parse(expiryDate.Text);
                purchaseOrder.ExpiryDate = DateTime.Parse(dte.ToLocalTime().ToShortDateString());

                DateTime validValue;
                DateTime? testdte;
                testdte = DateTime.TryParse(paymentDate.Text, out validValue)
                    ? validValue
                    : (DateTime?)null;
                purchaseOrder.PaymentDate = DateTime.Parse(testdte.GetValueOrDefault().ToShortDateString());


                purchaseOrder.Qty = txtQuantity.Text;
                purchaseOrder.Drugstore = slctdDrg;
                purchaseOrder.Product = slctdPrd;
                purchaseOrder.DRNumber = txtDR.Text;
                purchaseOrder.PONumber = txtPO.Text;
                purchaseOrder.SINumber = txtSI.Text;
                purchaseOrder.Amount = Convert.ToDouble(txtAmount.Text);
                purchaseOrder.isPaid = (chkPaid.IsChecked.Value) ? true : false;
                purchaseOrder.BatchNo = txtBatchNo.Text;

                var collection = db.GetCollection<PurchaseOrders>("PurchaseOrders");
                collection.InsertOne(purchaseOrder);
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Cause by: " + ex.StackTrace);
            }
        }

        private async Task<bool> checkSIandPONumber()
        {
            bool ifCorrect = false;
            List<PurchaseOrders> lstPurchaseOrd = new List<PurchaseOrders>();
            conDB = new ConnectionDB();
            try
            {
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                var collection = db.GetCollection<PurchaseOrders>("PurchaseOrders");
                var filter = Builders<PurchaseOrders>.Filter.Or(
        Builders<PurchaseOrders>.Filter.Where(p => p.PONumber == txtPO.Text),
        Builders<PurchaseOrders>.Filter.Where(p => p.SINumber == txtSI.Text));
                lstPurchaseOrd = collection.Find(filter).ToList();

                foreach (PurchaseOrders ds in lstPurchaseOrd)
                {
                    ds.strDeliveryDate = ds.DeliveryDate.ToShortDateString();
                    ds.strPaymentDate = ds.PaymentDate.ToShortDateString();
                    ds.strPaymentDueDate = ds.PaymentDueDate.ToShortDateString();
                }
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Caused by: " + ex.StackTrace);
            }
            return ifCorrect;
        }

        private async Task<List<PurchaseOrders>> search()
        {
            List<PurchaseOrders> lstDrgSales = new List<PurchaseOrders>();
            DateTime dteNow = (!string.IsNullOrEmpty(searchDateFrom.Text)) ? DateTime.Parse(searchDateFrom.Text) :  DateTime.Now;
            DateTime dteFirstDay = (!string.IsNullOrEmpty(searchDateTo.Text)) ? DateTime.Parse(searchDateTo.Text) : DateTime.Now;

            DateTime dteNowPayment = (!string.IsNullOrEmpty(searchPayDateFrom.Text)) ? DateTime.Parse(searchPayDateFrom.Text) : DateTime.Now;
            DateTime dteFirstDayPayment = (!string.IsNullOrEmpty(searchPayDateTo.Text)) ? DateTime.Parse(searchPayDateTo.Text) : DateTime.Now;

            try
            {
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                var collection = db.GetCollection<PurchaseOrders>("PurchaseOrders");

                var filter = Builders<PurchaseOrders>.Filter.And(
        Builders<PurchaseOrders>.Filter.Where(p => p.isDeleted == false));

                if (checkDate.IsChecked.Value)
                {
                    filter = filter & Builders<PurchaseOrders>.Filter.And(
          Builders<PurchaseOrders>.Filter.Gte("DeliveryDate", dteNow),
         Builders<PurchaseOrders>.Filter.Lte("DeliveryDate", dteFirstDay));
                }

                if (checkDrugstore.IsChecked.Value)
                {
                    Drugstores dd = searchDrugstore.SelectedItem as Drugstores;
                    filter = filter & Builders<PurchaseOrders>.Filter.And(
         Builders<PurchaseOrders>.Filter.Eq("Drugstore", dd));
                }

                if (checkCategory.IsChecked.Value)
                {
                    Products dd = searchDrugstore.SelectedItem as Products;
                    filter = filter & Builders<PurchaseOrders>.Filter.And(
         Builders<PurchaseOrders>.Filter.Eq("Products", dd));
                }

                if (chkPaymentDate.IsChecked.Value)
                {
                    filter = filter & Builders<PurchaseOrders>.Filter.And(
          Builders<PurchaseOrders>.Filter.Gte("PaymentDate", dteNowPayment),
         Builders<PurchaseOrders>.Filter.Lte("PaymentDate", dteFirstDayPayment));
                }

                if (chkPONumber.IsChecked.Value)
                {
                    filter = filter & Builders<PurchaseOrders>.Filter.And(
          Builders<PurchaseOrders>.Filter.Gte("PONumber", searchPO));
                }

                if (chkSINumber.IsChecked.Value)
                {
                    filter = filter & Builders<PurchaseOrders>.Filter.And(
          Builders<PurchaseOrders>.Filter.Gte("SINumber", searchSI));
                }

                lstDrgSales = collection.Find(filter).ToList();

            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Caused by: " + ex.StackTrace);
            }

            return lstDrgSales;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            purchaseOrderToUpdate = dgvPO.SelectedItem as PurchaseOrders;
            enableDisableControls(true);
            dgvPO.IsEnabled = false; 
            if (purchaseOrderToUpdate != null)
            {
                btnUpdate.Visibility = Visibility.Visible;
                btnCancel.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Hidden;

                txtPO.Text = purchaseOrderToUpdate.PONumber;
                txtSI.Text = purchaseOrderToUpdate.SINumber;
                txtDR.Text = purchaseOrderToUpdate.DRNumber;
                txtBatchNo.Text = purchaseOrderToUpdate.BatchNo;

                //paymentDate.Text = purchaseOrderToUpdate.PaymentDate.ToShortDateString();
                DateTime dTest = DateTime.Parse(purchaseOrderToUpdate.PaymentDate.ToShortDateString());
                paymentDate.Text = (dTest.Year.Equals(0001)) ? "" : dTest.ToShortDateString();

                deliveryDate.Text = purchaseOrderToUpdate.DeliveryDate.ToShortDateString();
                expiryDate.Text = purchaseOrderToUpdate.ExpiryDate.ToShortDateString();
                txtQuantity.Text = purchaseOrderToUpdate.Qty.ToString();
                chkPaid.IsChecked = purchaseOrderToUpdate.isPaid;
                txtAmount.Text = purchaseOrderToUpdate.Amount.ToString("N0");
                paymentDueDate.Text = purchaseOrderToUpdate.PaymentDueDate.ToShortDateString();

                foreach (Drugstores dsm in comboDrugstore.Items)
                {
                    if (dsm.Id.Equals(purchaseOrderToUpdate.Drugstore.Id))
                    {
                        comboDrugstore.SelectedItem = dsm;
                    }
                }

                foreach (Products prM in comboProduct.Items)
                {
                    if (prM.Id.Equals(purchaseOrderToUpdate.Product.Id))
                    {
                        comboProduct.SelectedItem = prM;
                    }
                }

            }
        }

        private async Task<bool> checkFields()
        {
            bool ifAllCorrect = false;

            if (string.IsNullOrEmpty(txtPO.Text))
            {
                await window.ShowMessageAsync("PO Number", "Please input PO number.");
            }
            else if (string.IsNullOrEmpty(txtBatchNo.Text))
            {
                await window.ShowMessageAsync("Batch No.", "Please provide Lot/Batch Number.");
            }
            else if (string.IsNullOrEmpty(txtSI.Text))
            {
                await window.ShowMessageAsync("SI Number", "Please provide SI number.");
            }
            else if (string.IsNullOrEmpty(txtDR.Text))
            {
                await window.ShowMessageAsync("DR Number", "Please select DR number.");
            }
            else if (string.IsNullOrEmpty(txtAmount.Text))
            {
                await window.ShowMessageAsync("Amount", "Please enter amount.");
            }
            else
            {
                ifAllCorrect = true;
            }

            return ifAllCorrect;
        }

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {

            if (checkDate.IsChecked.Value || checkCategory.IsChecked.Value || checkDrugstore.IsChecked.Value ||
                chkDRNumber.IsChecked.Value || chkSINumber.IsChecked.Value || chkPONumber.IsChecked.Value ||
                chkPaymentDate.IsChecked.Value)
            {
                if (string.IsNullOrEmpty(searchDateFrom.Text) && string.IsNullOrEmpty(searchDateTo.Text) && checkDate.IsChecked.Value)
                {
                    await window.ShowMessageAsync("SEARCH", "Please complete value to search.");
                }
                else if (checkCategory.IsChecked.Value && searchProduct.SelectedItem == null)
                {
                    await window.ShowMessageAsync("SEARCH", "Please complete value to search.");
                }
                else if (checkDrugstore.IsChecked.Value && searchDrugstore.SelectedItem == null)
                {
                    await window.ShowMessageAsync("SEARCH", "Please complete value to search.");
                }
                else if (chkDRNumber.IsChecked.Value && string.IsNullOrEmpty(searchDR.Text))
                {
                    await window.ShowMessageAsync("SEARCH", "Please complete value to search.");
                }
                else if (chkPONumber.IsChecked.Value && string.IsNullOrEmpty(searchPO.Text))
                {
                    await window.ShowMessageAsync("SEARCH", "Please complete value to search.");
                }
                else if (chkSINumber.IsChecked.Value && string.IsNullOrEmpty(searchSI.Text))
                {
                    await window.ShowMessageAsync("SEARCH", "Please complete value to search.");
                }
                else if (chkPaymentDate.IsChecked.Value && (string.IsNullOrEmpty(searchPayDateFrom.Text))
                   && (string.IsNullOrEmpty(searchDateTo.Text)))
                {
                    await window.ShowMessageAsync("SEARCH", "Please complete value to search.");
                }
                else
                {
                    dgvPO.ItemsSource = await search();
                }
            }
        }

        private async void btnReset_Click(object sender, RoutedEventArgs e)
        {
            checkDate.IsChecked = false;
            checkCategory.IsChecked = false;
            checkDrugstore.IsChecked = false;
            chkDRNumber.IsChecked = false;
            chkSINumber.IsChecked = false;
            chkPONumber.IsChecked = false;
            chkPaymentDate.IsChecked = false;
            dgvPO.ItemsSource = await loadDataGridDetails();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            clearFields();
            btnUpdate.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;
            chkPaid.IsChecked = false;
            chkPaid.IsEnabled = true;
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool bl = await checkFields();

            if (bl)
            {
                MessageDialogResult result;

                if (chkPaid.IsChecked.Value)
                {
                    result = await window.ShowMessageAsync("Purchase Order", "Is this transaction ALREADY PAID?", MessageDialogStyle.AffirmativeAndNegative);

                }
                else
                {
                    result = await window.ShowMessageAsync("Purchase Order", "Is this transaction STILL UNPAID?", MessageDialogStyle.AffirmativeAndNegative);
                }

                if (result == MessageDialogResult.Affirmative)
                {
                    updateRecord();
                    clearFields();
                    await window.ShowMessageAsync("UPDATE RECORD", "Record updated successfully!");
                    dgvPO.ItemsSource = await loadDataGridDetails();
                }
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool bl = await checkFields();
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
            if (bl)
            {
                bool blSIPO = await checkSIandPONumber();
                if (blSIPO)
                {
                    await window.ShowMessageAsync("SAVE RECORD", "PO or SI already exist. Please check/search record.");
                }
                else
                {
                    saveRecord();
                    clearFields();
                    await window.ShowMessageAsync("SAVE RECORD", "Record saved successfully!");
                    dgvPO.ItemsSource = await loadDataGridDetails();
                }

            }
        }

        private void checkDate_Checked(object sender, RoutedEventArgs e)
        {
            searchDateFrom.IsEnabled = true;
            searchDateTo.IsEnabled = true;
        }

        private void checkCategory_Checked(object sender, RoutedEventArgs e)
        {
            searchProduct.IsEnabled = true;
        }

        private void checkDrugstore_Checked(object sender, RoutedEventArgs e)
        {
            searchDrugstore.IsEnabled = true;
        }

        private void checkDate_Unchecked(object sender, RoutedEventArgs e)
        {
            searchDateFrom.IsEnabled = false;
            searchDateTo.IsEnabled = false;
        }

        private void checkCategory_Unchecked(object sender, RoutedEventArgs e)
        {
            searchProduct.IsEnabled = false;
        }

        private void checkDrugstore_Unchecked(object sender, RoutedEventArgs e)
        {
            searchDrugstore.IsEnabled = false;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            POReport rep = new POReport();
            rep.ShowDialog();
            //ReportForm rf = new ReportForm(lstPurchaseOrderReports());
            // rf.ShowDialog();
        }

        private void MenuStocks_Click(object sender, RoutedEventArgs e)
        {
            POReport rep = new POReport();
            rep.ShowDialog();
            //ReportForm rf = new ReportForm(lstPurchaseOrderReports());
            // rf.ShowDialog();
        }

        private void clearFields()
        {
            txtPO.Text = "";
            txtSI.Text = "";
            txtDR.Text = "";
            paymentDate.Text = "";
            deliveryDate.Text = "";
            txtQuantity.Text = "";
            txtAmount.Text = "";
            paymentDueDate.Text = "";
            txtPO.IsEnabled = true;
            txtSI.IsEnabled = true;
            txtDR.IsEnabled = true;
            txtAmount.IsEnabled = true;
            paymentDate.IsEnabled = true;
            deliveryDate.IsEnabled = true;
            txtQuantity.IsEnabled = true;
            comboDrugstore.IsEnabled = true;
            comboProduct.IsEnabled = true;
            comboDrugstore.SelectedItem = null;
            comboProduct.SelectedItem = null;
            chkPaid.IsChecked = false;
            chkPaid.IsEnabled = true;
            btnUpdate.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;
            dgvPO.IsEnabled = true;
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            purchaseOrderToUpdate = dgvPO.SelectedItem as PurchaseOrders;

            if (purchaseOrderToUpdate != null)
            {
                    
                txtDR.Text = purchaseOrderToUpdate.DRNumber;
                txtSI.Text = purchaseOrderToUpdate.SINumber;
                txtPO.Text = purchaseOrderToUpdate.PONumber;
                txtQuantity.Text = purchaseOrderToUpdate.Qty;
                deliveryDate.Text = purchaseOrderToUpdate.DeliveryDate.ToShortDateString();
                paymentDate.Text = purchaseOrderToUpdate.PaymentDate.ToShortDateString();
                txtAmount.Text = purchaseOrderToUpdate.Amount.ToString("0.##");
                paymentDueDate.Text = purchaseOrderToUpdate.PaymentDueDate.ToShortDateString();
                chkPaid.IsChecked = purchaseOrderToUpdate.isPaid;
              
                foreach (Drugstores dsm in comboDrugstore.Items)
                {
                    if (dsm.Id.Equals(purchaseOrderToUpdate.Drugstore.Id))
                    {
                        comboDrugstore.SelectedItem = dsm;
                    }
                }

                foreach (Products prM in comboProduct.Items)
                {
                    if (prM.Id.Equals(purchaseOrderToUpdate.Product.Id))
                    {
                        comboProduct.SelectedItem = prM;
                    }
                }
                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Hidden;
                //btnCancel.Visibility = Visibility.Hidden;

                enableDisableControls(false);
            }
        }

        private void enableDisableControls(bool b)
        {
            txtDR.IsEnabled = b;
            txtSI.IsEnabled = b;
            txtPO.IsEnabled = b;
            txtQuantity.IsEnabled = b;
            paymentDate.IsEnabled = b;
            deliveryDate.IsEnabled = b;
            comboDrugstore.IsEnabled = b;
            comboProduct.IsEnabled = b;
            chkPaid.IsEnabled = b;
            txtAmount.IsEnabled = b;
        }

        private void chkSINumber_Checked(object sender, RoutedEventArgs e)
        {
            searchSI.IsEnabled = true;
        }

        private void chkPONumber_Checked(object sender, RoutedEventArgs e)
        {
            searchPO.IsEnabled = true;
        }

        private void chkSINumber_Unchecked(object sender, RoutedEventArgs e)
        {
            searchSI.IsEnabled = false;
        }

        private void chkPONumber_Unchecked(object sender, RoutedEventArgs e)
        {
            searchPO.IsEnabled = false;
        }

        private void chkDRNumber_Checked(object sender, RoutedEventArgs e)
        {
            searchDR.IsEnabled = true;
        }

        private void chkDRNumber_Unchecked(object sender, RoutedEventArgs e)
        {
            searchDR.IsEnabled = false;
        }

        private void chkPaymentDate_Checked(object sender, RoutedEventArgs e)
        {
            searchPayDateFrom.IsEnabled = true;
            searchPayDateTo.IsEnabled = true;
        }

        private void chkPaymentDate_Unchecked(object sender, RoutedEventArgs e)
        {
            searchPayDateFrom.IsEnabled = false;
            searchPayDateTo.IsEnabled = false;
        }

        private void deliveryDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            Drugstores dm = comboDrugstore.SelectedItem as Drugstores;
            if (dm != null)
            {
                if (!string.IsNullOrEmpty(deliveryDate.Text))
                {
                    DateTime d1 = DateTime.Parse(deliveryDate.Text);

                    DateTime d2 = d1.AddDays(Convert.ToInt32(dm.Terms));

                    paymentDueDate.Text = d2.ToShortDateString();
                }
            }
        }

        #region MYSQL CODES

        //private void loadDrugstoreOnCombo()
        //{
        //    conDB = new ConnectionDB();

        //    string queryString = "SELECT ID, drugstore, description, paymentdue FROM dbfh.tbldrugstores WHERE isDeleted = 0 order by description asc";

        //    MySqlDataReader reader = conDB.getSelectConnection(queryString, null);
        //    comboDrugstore.Items.Clear();
        //    searchDrugstore.Items.Clear();
        //    while (reader.Read())
        //    {
        //        DrugstoreModel drg = new DrugstoreModel();
        //        drg.ID = reader["ID"].ToString();
        //        drg.DrugstoreName = reader["drugstore"].ToString();
        //        drg.Description = reader["description"].ToString();
        //        drg.PaymentDue = reader["paymentdue"].ToString();
        //        comboDrugstore.Items.Add(drg);
        //        searchDrugstore.Items.Add(drg);
        //    }
        //    conDB.closeConnection();
        //}

        //private void loadProductOnCombo()
        //{
        //    conDB = new ConnectionDB();
        //    ProductModel prod = new ProductModel();

        //    string queryString = "SELECT ID, productname, description FROM dbfh.tblproducts WHERE isDeleted = 0";

        //    MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

        //    comboProduct.Items.Clear();
        //    searchProduct.Items.Clear();

        //    while (reader.Read())
        //    {
        //        prod.ID = reader["ID"].ToString();
        //        prod.ProductName = reader["productname"].ToString();
        //        prod.Description = reader["description"].ToString();

        //        comboProduct.Items.Add(prod);
        //        searchProduct.Items.Add(prod);
        //        prod = new ProductModel();
        //    }

        //    conDB.closeConnection();
        //}

        //private List<PurchaseOrderModel> lstPurchaseOrderReports()
        //{
        //    conDB = new ConnectionDB();
        //    List<PurchaseOrderModel> lstPurchaseOrderModel = new List<PurchaseOrderModel>();
        //    PurchaseOrderModel pom = new PurchaseOrderModel();
        //    string queryString = "SELECT dbfh.tblpurchaseorder.ID, ponumber, sinumber, drnumber, paymentdate, " +
        //        "deliverydate, quantity, drugstoreID, productID, dbfh.tblproducts.productname, isPaid, amount," +
        //        " dbfh.tbldrugstores.description, dbfh.tbldrugstores.paymentdue AS 'terms', " +
        //        "dbfh.tblpurchaseorder.paymentduedate FROM ((dbfh.tblpurchaseorder INNER JOIN " +
        //        "dbfh.tbldrugstores ON tblpurchaseorder.drugstoreID = tbldrugstores.ID) INNER JOIN " +
        //        "dbfh.tblproducts ON dbfh.tblpurchaseorder.productID = tblproducts.ID) " +
        //        "WHERE dbfh.tblpurchaseorder.isDeleted = 0  AND dbfh.tblpurchaseorder.isPaid = 0 order by deliverydate ASC";

        //    MySqlDataReader reader = conDB.getSelectConnection(queryString, null);
        //    string temp = "";
        //    while (reader.Read())
        //    {
        //        pom.ID = reader["ID"].ToString();
        //        pom.DrugstoreName = reader["description"].ToString();
        //        pom.Quantity = reader["quantity"].ToString();
        //        pom.ProductName = reader["productname"].ToString();
        //        pom.Amount = reader["amount"].ToString();
        //        pom.SINumber = reader["sinumber"].ToString();
        //        temp = reader["deliverydate"].ToString();
        //        DateTime d1;
        //        if (!string.IsNullOrEmpty(temp) && !temp.Equals("0000-00-00"))
        //        {
        //            d1 = DateTime.Parse(temp);
        //            pom.DeliveryDate = d1.ToShortDateString();
        //        }
        //        else
        //        {
        //            pom.DeliveryDate = "";
        //        }

        //        pom.Terms = reader["terms"].ToString();
        //        temp = reader["paymentduedate"].ToString();
        //        if (!string.IsNullOrEmpty(temp) && !temp.Equals("0000-00-00"))
        //        {
        //            d1 = DateTime.Parse(temp);
        //            pom.PaymentDueDate = d1.ToShortDateString();

        //            //CHECK IF DATE IS BEYOND DUE DATE
        //            DateTime dtePaymentDue = DateTime.Parse(pom.PaymentDueDate);
        //            DateTime dteNow = DateTime.Now;

        //            if (dteNow.Date >= dtePaymentDue)
        //            {
        //                pom.boolPaid = true;
        //            }
        //            else
        //            {
        //                pom.boolPaid = false;
        //            }
        //        }
        //        else
        //        {
        //            pom.PaymentDueDate = "";
        //        }
        //        pom.PONumber = reader["ponumber"].ToString();
        //        lstPurchaseOrderModel.Add(pom);
        //        pom = new PurchaseOrderModel();
        //    }

        //    conDB.closeConnection();

        //    return lstPurchaseOrderModel;
        //}

        //private List<PurchaseOrderModel> loadDataGridDetails()
        //{
        //    conDB = new ConnectionDB();
        //    List<PurchaseOrderModel> lstPurchaseOrder = new List<PurchaseOrderModel>();
        //    PurchaseOrderModel purchase = new PurchaseOrderModel();

        //    string queryString = "SELECT dbfh.tblpurchaseorder.ID, ponumber, sinumber, drnumber, paymentdate, deliverydate, " +
        //        "quantity, drugstoreID, productID, isPaid, amount, dbfh.tbldrugstores.description, dbfh.tblpurchaseorder.paymentduedate FROM " +
        //        "((dbfh.tblpurchaseorder INNER JOIN dbfh.tbldrugstores ON tblpurchaseorder.drugstoreID = tbldrugstores.ID)" +
        //        " INNER JOIN dbfh.tblproducts ON dbfh.tblpurchaseorder.productID = tblproducts.ID) " +
        //        "WHERE dbfh.tblpurchaseorder.isDeleted = 0 order by deliverydate ASC";

        //    MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

        //    while (reader.Read())
        //    {
        //        purchase.ID = reader["ID"].ToString();
        //        purchase.PONumber = reader["ponumber"].ToString();
        //        purchase.SINumber = reader["sinumber"].ToString();
        //        purchase.DRNumber = reader["drnumber"].ToString();
        //        purchase.DrugstoreName = reader["description"].ToString();
        //        string temp = reader["paymentdate"].ToString();
        //        if (!string.IsNullOrEmpty(temp))
        //        {
        //            DateTime dte = DateTime.Parse(temp);
        //            if (dte.Year == 0001)
        //            {
        //                purchase.PaymentDate = "";
        //            }
        //            else
        //            {
        //                purchase.PaymentDate = dte.ToShortDateString();
        //            }
        //        }
        //        temp = reader["deliverydate"].ToString();
        //        if (!string.IsNullOrEmpty(temp))
        //        {
        //            DateTime dte = DateTime.Parse(temp);
        //            if (dte.Year == 0001)
        //            {
        //                purchase.DeliveryDate = "";
        //            }
        //            else
        //            {
        //                purchase.DeliveryDate = dte.ToShortDateString();
        //            }
        //        }
        //        temp = reader["paymentduedate"].ToString();
        //        if (!string.IsNullOrEmpty(temp))
        //        {
        //            DateTime dte = DateTime.Parse(temp);
        //            if (dte.Year == 0001)
        //            {
        //                purchase.PaymentDueDate = "";
        //            }
        //            else
        //            {
        //                purchase.PaymentDueDate = dte.ToShortDateString();
        //            }
        //        }
        //        else
        //        {
        //            purchase.PaymentDueDate = "";
        //        }
        //        purchase.Quantity = reader["quantity"].ToString();
        //        purchase.DrugstoreID = reader["drugstoreID"].ToString();
        //        purchase.ProductID = reader["productID"].ToString();
        //        purchase.isPaid = reader["isPaid"].ToString();
        //        double tempAmt = Convert.ToDouble(reader["amount"].ToString());
        //        purchase.Amount = tempAmt.ToString("N2");

        //        if (purchase.isPaid.Equals("1"))
        //        {
        //            purchase.boolPaid = true;
        //        }
        //        else
        //        {
        //            purchase.boolPaid = false;
        //        }
        //        lstPurchaseOrder.Add(purchase);
        //        purchase = new PurchaseOrderModel();
        //    }
        //    conDB.closeConnection();
        //    return lstPurchaseOrder;
        //}

        //private void saveRecord()
        //{
        //    conDB = new ConnectionDB();

        //    string queryString = "INSERT INTO dbfh.tblpurchaseorder (ponumber, sinumber, drnumber, paymentdate, deliverydate," +
        //        " quantity, drugstoreID, productID, amount, isPaid, paymentduedate, isDeleted) VALUES (?,?,?,?,?,?,?,?,?,?,?,0)";

        //    List<string> parameters = new List<string>();

        //    parameters.Add(txtPO.Text);
        //    parameters.Add(txtSI.Text);
        //    parameters.Add(txtDR.Text);
        //    if (!string.IsNullOrEmpty(paymentDate.Text))
        //    {
        //        DateTime date = DateTime.Parse(paymentDate.Text);
        //        parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
        //    }
        //    else
        //    {
        //        parameters.Add("0000-00-00");
        //    }

        //    if (!string.IsNullOrEmpty(deliveryDate.Text))
        //    {
        //        DateTime date = DateTime.Parse(deliveryDate.Text);
        //        parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
        //    }
        //    else
        //    {
        //        parameters.Add("0000-00-00");
        //    }

        //    parameters.Add(txtQuantity.Text);
        //    parameters.Add(comboDrugstore.SelectedValue.ToString());
        //    parameters.Add(comboProduct.SelectedValue.ToString());
        //    parameters.Add(txtAmount.Text);
        //    if (chkPaid.IsChecked.Value)
        //    {
        //        parameters.Add("1");
        //    }
        //    else
        //    {
        //        parameters.Add("0");
        //    }

        //    if (!string.IsNullOrEmpty(paymentDueDate.Text))
        //    {
        //        DateTime date = DateTime.Parse(paymentDueDate.Text);
        //        parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
        //    }
        //    else
        //    {
        //        parameters.Add("0000-00-00");
        //    }

        //    conDB.AddRecordToDatabase(queryString, parameters);

        //    conDB.closeConnection();
        //}

        //private void updateRecord(PurchaseOrderModel pom)
        //{
        //    conDB = new ConnectionDB();

        //    string queryString = "UPDATE dbfh.tblpurchaseorder SET ponumber = ?, sinumber = ?, drnumber = ?, paymentdate = ?" +
        //        ", deliverydate = ?, quantity = ?, drugstoreID = ?, productID = ? , isPaid = ?, amount = ?, paymentduedate = ? WHERE ID = ?";

        //    List<string> parameters = new List<string>();
        //    parameters.Add(txtPO.Text);
        //    parameters.Add(txtSI.Text);
        //    parameters.Add(txtDR.Text);
        //    if (!string.IsNullOrEmpty(paymentDate.Text))
        //    {
        //        DateTime date = DateTime.Parse(paymentDate.Text);
        //        parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
        //    }
        //    else
        //    {
        //        parameters.Add("0000-00-00");
        //    }
        //    if (!string.IsNullOrEmpty(deliveryDate.Text))
        //    {
        //        DateTime date = DateTime.Parse(deliveryDate.Text);
        //        parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
        //    }
        //    else
        //    {
        //        parameters.Add("0000-00-00");
        //    }
        //    parameters.Add(txtQuantity.Text);
        //    parameters.Add(comboDrugstore.SelectedValue.ToString());
        //    parameters.Add(comboProduct.SelectedValue.ToString());

        //    if (chkPaid.IsChecked.Value)
        //    {
        //        parameters.Add("1");
        //    }
        //    else
        //    {
        //        parameters.Add("0");
        //    }
        //    parameters.Add(txtAmount.Text);

        //    if (!string.IsNullOrEmpty(paymentDueDate.Text))
        //    {
        //        DateTime date = DateTime.Parse(paymentDueDate.Text);
        //        parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
        //    }
        //    else
        //    {
        //        parameters.Add("0000-00-00");
        //    }

        //    parameters.Add(pom.ID);
        //    conDB.AddRecordToDatabase(queryString, parameters);

        //    conDB.closeConnection();
        //}

        //private bool checkIfExistingPO()
        //{
        //    conDB = new ConnectionDB();
        //    bool ifExist = false;
        //    string queryString = "SELECT ponumber FROM dbfh.tblpurchaseorder WHERE ponumber = ?";
        //    List<string> parameters = new List<string>();

        //    parameters.Add(txtPO.Text);

        //    MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);
        //    string t = "";
        //    while (reader.Read())
        //    {
        //        t = reader["ponumber"].ToString();
        //        ifExist = true;
        //        if (t.Equals("0"))
        //        {
        //            ifExist = false;
        //        }
        //    }

        //    return ifExist;
        //}

        //private bool checkIfExistingSI()
        //{
        //    conDB = new ConnectionDB();
        //    bool ifExist = false;
        //    string queryString = "SELECT sinumber FROM dbfh.tblpurchaseorder WHERE sinumber = ?";
        //    List<string> parameters = new List<string>();

        //    parameters.Add(txtSI.Text);

        //    MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);
        //    string t = "";
        //    while (reader.Read())
        //    {
        //        t = reader["sinumber"].ToString();
        //        ifExist = true;
        //        if (t.Equals("0"))
        //        {
        //            ifExist = false;
        //        }
        //    }

        //    return ifExist;
        //}

        //private List<PurchaseOrderModel> search()
        //{
        //    conDB = new ConnectionDB();
        //    List<PurchaseOrderModel> lstPurchase = new List<PurchaseOrderModel>();
        //    PurchaseOrderModel purchaseOrderMod = new PurchaseOrderModel();

        //    string queryString = "SELECT dbfh.tblpurchaseorder.ID, ponumber, sinumber, drnumber, paymentdate, deliverydate, " +
        //        "quantity, drugstoreID, productID, dbfh.tbldrugstores.description, dbfh.tbldrugstores.description as 'drugstorename', dbfh.tblproducts.description, isPaid, amount FROM " +
        //        "((dbfh.tblpurchaseorder INNER JOIN dbfh.tbldrugstores ON dbfh.tblpurchaseorder.drugstoreID = dbfh.tbldrugstores.ID)" +
        //        " INNER JOIN dbfh.tblproducts ON dbfh.tblpurchaseorder.productID = dbfh.tblproducts.ID) WHERE dbfh.tblpurchaseorder.isDeleted = 0";

        //    List<string> parameters = new List<string>();

        //    if (checkDate.IsChecked.Value)
        //    {
        //        queryString += " AND (deliverydate BETWEEN ? AND ?)";
        //        DateTime sdate = DateTime.Parse(searchDateFrom.Text);
        //        parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);
        //        sdate = DateTime.Parse(searchDateTo.Text);
        //        parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);
        //    }

        //    if (checkCategory.IsChecked.Value)
        //    {
        //        queryString += " AND (dbfh.tblpurchaseorder.productID = ?)";
        //        parameters.Add(searchProduct.SelectedValue.ToString());
        //    }

        //    if (checkDrugstore.IsChecked.Value)
        //    {
        //        queryString += " AND (dbfh.tblpurchaseorder.drugstoreID = ?)";
        //        parameters.Add(searchDrugstore.SelectedValue.ToString());
        //    }

        //    if (chkDRNumber.IsChecked.Value)
        //    {
        //        queryString += " AND (drnumber = ?)";
        //        parameters.Add(searchDR.Text);
        //    }
        //    if (chkPONumber.IsChecked.Value)
        //    {
        //        queryString += " AND (ponumber = ?)";
        //        parameters.Add(searchPO.Text);
        //    }
        //    if (chkSINumber.IsChecked.Value)
        //    {
        //        queryString += " AND (sinumber = ?)";
        //        parameters.Add(searchSI.Text);
        //    }

        //    if (chkPaymentDate.IsChecked.Value)
        //    {
        //        queryString += " AND (paymentdate BETWEEN ? AND ?)";
        //        DateTime sdate = DateTime.Parse(searchPayDateFrom.Text);
        //        parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);
        //        sdate = DateTime.Parse(searchPayDateTo.Text);
        //        parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);
        //    }

        //    queryString += " ORDER BY dbfh.tblpurchaseorder.deliverydate DESC";

        //    MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

        //    while (reader.Read())
        //    {
        //        purchaseOrderMod.ID = reader["ID"].ToString();
        //        purchaseOrderMod.DRNumber = reader["drnumber"].ToString();
        //        purchaseOrderMod.SINumber = reader["sinumber"].ToString();
        //        purchaseOrderMod.PONumber = reader["ponumber"].ToString();
        //        purchaseOrderMod.DrugstoreName = reader["drugstorename"].ToString();
        //        string temp = reader["paymentdate"].ToString();
        //        if (!string.IsNullOrEmpty(temp))
        //        {
        //            DateTime dte = DateTime.Parse(temp);
        //            if (dte.Year == 0001)
        //            {
        //                purchaseOrderMod.PaymentDate = "";
        //            }
        //            else
        //            {
        //                purchaseOrderMod.PaymentDate = dte.ToShortDateString();
        //            }

        //        }
        //        temp = reader["deliverydate"].ToString();
        //        if (!string.IsNullOrEmpty(temp))
        //        {
        //            DateTime dte = DateTime.Parse(temp);
        //            if (dte.Year == 0001)
        //            {
        //                purchaseOrderMod.DeliveryDate = "";
        //            }
        //            else
        //            {
        //                purchaseOrderMod.DeliveryDate = dte.ToShortDateString();
        //            }
        //        }
        //        purchaseOrderMod.DrugstoreID = reader["drugstoreID"].ToString();
        //        purchaseOrderMod.ProductID = reader["productID"].ToString();
        //        purchaseOrderMod.Quantity = reader["quantity"].ToString();
        //        purchaseOrderMod.isPaid = reader["isPaid"].ToString();
        //        purchaseOrderMod.Amount = Convert.ToDouble(reader["amount"].ToString()).ToString("N0");
        //        if (purchaseOrderMod.isPaid.Equals("1"))
        //        {
        //            purchaseOrderMod.boolPaid = true;
        //        }
        //        else
        //        {
        //            purchaseOrderMod.boolPaid = false;
        //        }
        //        lstPurchase.Add(purchaseOrderMod);
        //        purchaseOrderMod = new PurchaseOrderModel();
        //    }

        //    return lstPurchase;
        //}


        #endregion


    }
}
