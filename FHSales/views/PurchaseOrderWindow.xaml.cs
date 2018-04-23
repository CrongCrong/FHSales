using FHSales.Classes;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using System.Data.SqlTypes;
using Microsoft.Reporting.WinForms;

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
        PurchaseOrderModel purchaseOrderModel;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            searchDateFrom.IsEnabled = false;
            searchDateTo.IsEnabled = false;
            searchProduct.IsEnabled = false;
            searchDrugstore.IsEnabled = false;
            searchDR.IsEnabled = false;
            searchPO.IsEnabled = false;
            searchSI.IsEnabled = false;
            dgvPO.ItemsSource = loadDataGridDetails();
            loadDrugstoreOnCombo();
            loadProductOnCombo();
            btnUpdate.Visibility = Visibility.Hidden;
            paymentDueDate.IsEnabled = false;
            if ((Convert.ToInt32(UserModel.UserType) == (int)UserTypeEnum.PO_VIEW) ||
                (Convert.ToInt32(UserModel.UserType) == (int)UserTypeEnum.ADMIN))
            {
                btnEdit.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Visible;
                btnUpdate.Visibility = Visibility.Hidden;
                btnCancel.Visibility = Visibility.Visible;
            }
            else
            {
                btnEdit.Visibility = Visibility.Hidden;
                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Hidden;
                btnCancel.Visibility = Visibility.Hidden;
            }
        }

        private List<PurchaseOrderModel> loadDataGridDetails()
        {
            conDB = new ConnectionDB();
            List<PurchaseOrderModel> lstPurchaseOrder = new List<PurchaseOrderModel>();
            PurchaseOrderModel purchase = new PurchaseOrderModel();

            string queryString = "SELECT dbfh.tblpurchaseorder.ID, ponumber, sinumber, drnumber, paymentdate, deliverydate, " +
                "quantity, drugstoreID, productID, isPaid, amount, dbfh.tbldrugstores.description, dbfh.tblpurchaseorder.paymentduedate FROM " +
                "((dbfh.tblpurchaseorder INNER JOIN dbfh.tbldrugstores ON tblpurchaseorder.drugstoreID = tbldrugstores.ID)" +
                " INNER JOIN dbfh.tblproducts ON dbfh.tblpurchaseorder.productID = tblproducts.ID) " +
                "WHERE dbfh.tblpurchaseorder.isDeleted = 0 order by deliverydate ASC";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                purchase.ID = reader["ID"].ToString();
                purchase.PONumber = reader["ponumber"].ToString();
                purchase.SINumber = reader["sinumber"].ToString();
                purchase.DRNumber = reader["drnumber"].ToString();
                purchase.DrugstoreName = reader["description"].ToString();
                string temp = reader["paymentdate"].ToString();
                if (!string.IsNullOrEmpty(temp))
                {
                    DateTime dte = DateTime.Parse(temp);
                    if(dte.Year == 0001)
                    {
                        purchase.PaymentDate = "";
                    }else
                    {
                        purchase.PaymentDate = dte.ToShortDateString();
                    }                    
                }
                temp = reader["deliverydate"].ToString();
                if (!string.IsNullOrEmpty(temp))
                {
                    DateTime dte = DateTime.Parse(temp);
                    if (dte.Year == 0001)
                    {
                        purchase.DeliveryDate = "";
                    }
                    else
                    {
                        purchase.DeliveryDate = dte.ToShortDateString();
                    }
                }
                temp = reader["paymentduedate"].ToString();
                if (!string.IsNullOrEmpty(temp))
                {
                    DateTime dte = DateTime.Parse(temp);
                    if (dte.Year == 0001)
                    {
                        purchase.PaymentDueDate = "";
                    }
                    else
                    {
                        purchase.PaymentDueDate = dte.ToShortDateString();
                    }
                }else
                {
                    purchase.PaymentDueDate = "";
                }
                purchase.Quantity = reader["quantity"].ToString();
                purchase.DrugstoreID = reader["drugstoreID"].ToString();
                purchase.ProductID = reader["productID"].ToString();
                purchase.isPaid = reader["isPaid"].ToString();
                purchase.Amount = Convert.ToDouble(reader["amount"].ToString()).ToString("N0");
                if (purchase.isPaid.Equals("1"))
                {
                    purchase.boolPaid = true;
                }else
                {
                    purchase.boolPaid = false;
                }
                lstPurchaseOrder.Add(purchase);
                purchase = new PurchaseOrderModel();
            }
            conDB.closeConnection();
            return lstPurchaseOrder;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            purchaseOrderModel = dgvPO.SelectedItem as PurchaseOrderModel;
            enableDisableControls(true);
            if (purchaseOrderModel != null)
            {
                btnUpdate.Visibility = Visibility.Visible;
                btnCancel.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Hidden;

                txtPO.Text = purchaseOrderModel.PONumber;
                txtSI.Text = purchaseOrderModel.SINumber;
                txtDR.Text = purchaseOrderModel.DRNumber;
                paymentDate.Text = purchaseOrderModel.PaymentDate;
                deliveryDate.Text = purchaseOrderModel.DeliveryDate;
                txtQuantity.Text = purchaseOrderModel.Quantity;
                chkPaid.IsChecked = purchaseOrderModel.boolPaid;
                txtAmount.Text = purchaseOrderModel.Amount;
                paymentDueDate.Text = purchaseOrderModel.PaymentDueDate;

                foreach (DrugstoreModel dsm in comboDrugstore.Items)
                {
                    if (dsm.ID.Equals(purchaseOrderModel.DrugstoreID))
                    {
                        comboDrugstore.SelectedItem = dsm;
                    }
                }

                foreach (ProductModel prM in comboProduct.Items)
                {
                    if (prM.ID.Equals(purchaseOrderModel.ProductID))
                    {
                        comboProduct.SelectedItem = prM;
                    }
                }

            }
        }

        private void saveRecord()
        {
            conDB = new ConnectionDB();

            string queryString = "INSERT INTO dbfh.tblpurchaseorder (ponumber, sinumber, drnumber, paymentdate, deliverydate," +
                " quantity, drugstoreID, productID, amount, isPaid, paymentduedate, isDeleted) VALUES (?,?,?,?,?,?,?,?,?,?,?,0)";

            List<string> parameters = new List<string>();

            parameters.Add(txtPO.Text);
            parameters.Add(txtSI.Text);
            parameters.Add(txtDR.Text);
            if (!string.IsNullOrEmpty(paymentDate.Text))
            {
                DateTime date = DateTime.Parse(paymentDate.Text);
                parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
            }
            else
            {
                parameters.Add("0000-00-00");
            }

            if (!string.IsNullOrEmpty(deliveryDate.Text))
            {
                DateTime date = DateTime.Parse(deliveryDate.Text);
                parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
            }
            else
            {
                parameters.Add("0000-00-00");
            }

            parameters.Add(txtQuantity.Text);
            parameters.Add(comboDrugstore.SelectedValue.ToString());
            parameters.Add(comboProduct.SelectedValue.ToString());
            parameters.Add(txtAmount.Text);
            if (chkPaid.IsChecked.Value)
            {
                parameters.Add("1");
            }
            else
            {
                parameters.Add("0");
            }

            if (!string.IsNullOrEmpty(paymentDueDate.Text))
            {
                DateTime date = DateTime.Parse(paymentDueDate.Text);
                parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
            }
            else
            {
                parameters.Add("0000-00-00");
            }

            conDB.AddRecordToDatabase(queryString, parameters);

            conDB.closeConnection();
        }

        private void updateRecord(PurchaseOrderModel pom)
        {
            conDB = new ConnectionDB();

            string queryString = "UPDATE dbfh.tblpurchaseorder SET ponumber = ?, sinumber = ?, drnumber = ?, paymentdate = ?" +
                ", deliverydate = ?, quantity = ?, drugstoreID = ?, productID = ? , isPaid = ?, amount = ?, paymentduedate = ? WHERE ID = ?";

            List<string> parameters = new List<string>();
            parameters.Add(txtPO.Text);
            parameters.Add(txtSI.Text);
            parameters.Add(txtDR.Text);
            if (!string.IsNullOrEmpty(paymentDate.Text))
            {
                DateTime date = DateTime.Parse(paymentDate.Text);
                parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
            }
            else
            {
                parameters.Add("0000-00-00");
            }
            if (!string.IsNullOrEmpty(deliveryDate.Text))
            {
                DateTime date = DateTime.Parse(deliveryDate.Text);
                parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
            }
            else
            {
                parameters.Add("0000-00-00");
            }
            parameters.Add(txtQuantity.Text);
            parameters.Add(comboDrugstore.SelectedValue.ToString());
            parameters.Add(comboProduct.SelectedValue.ToString());
            
            if (chkPaid.IsChecked.Value)
            {
                parameters.Add("1");
            }else
            {
                parameters.Add("0");
            }
            parameters.Add(txtAmount.Text);

            if (!string.IsNullOrEmpty(paymentDueDate.Text))
            {
                DateTime date = DateTime.Parse(paymentDueDate.Text);
                parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
            }
            else
            {
                parameters.Add("0000-00-00");
            }

            parameters.Add(pom.ID);
            conDB.AddRecordToDatabase(queryString, parameters);

            conDB.closeConnection();
        }

        private bool checkIfExistingPO()
        {
            conDB = new ConnectionDB();
            bool ifExist = false;
            string queryString = "SELECT ponumber FROM dbfh.tblpurchaseorder WHERE ponumber = ?";
            List<string> parameters = new List<string>();

            parameters.Add(txtPO.Text);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);
            string t = "";
            while (reader.Read())
            {
                t = reader["ponumber"].ToString();
                ifExist = true;
                if (t.Equals("0"))
                {
                    ifExist = false;
                }
            }

            return ifExist;
        }

        private bool checkIfExistingSI()
        {
            conDB = new ConnectionDB();
            bool ifExist = false;
            string queryString = "SELECT sinumber FROM dbfh.tblpurchaseorder WHERE sinumber = ?";
            List<string> parameters = new List<string>();

            parameters.Add(txtSI.Text);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);
            string t = "";
            while (reader.Read())
            {
                t = reader["sinumber"].ToString();
                ifExist = true;
                if (t.Equals("0"))
                {
                    ifExist = false;
                }
            }

            return ifExist;
        }

        private List<PurchaseOrderModel> search()
        {
            conDB = new ConnectionDB();
            List<PurchaseOrderModel> lstPurchase = new List<PurchaseOrderModel>();
            PurchaseOrderModel purchaseOrderMod = new PurchaseOrderModel();

            string queryString = "SELECT dbfh.tblpurchaseorder.ID, ponumber, sinumber, drnumber, paymentdate, deliverydate, " +
                "quantity, drugstoreID, productID, dbfh.tbldrugstores.description, dbfh.tbldrugstores.description as 'drugstorename', dbfh.tblproducts.description, isPaid, amount FROM " +
                "((dbfh.tblpurchaseorder INNER JOIN dbfh.tbldrugstores ON dbfh.tblpurchaseorder.drugstoreID = dbfh.tbldrugstores.ID)" +
                " INNER JOIN dbfh.tblproducts ON dbfh.tblpurchaseorder.productID = dbfh.tblproducts.ID) WHERE dbfh.tblpurchaseorder.isDeleted = 0";

            List<string> parameters = new List<string>();

            if (checkDate.IsChecked.Value)
            {
                queryString += " AND (deliverydate BETWEEN ? AND ?)";
                DateTime sdate = DateTime.Parse(searchDateFrom.Text);
                parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);
                sdate = DateTime.Parse(searchDateTo.Text);
                parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);
            }

            if (checkCategory.IsChecked.Value)
            {
                queryString += " AND (dbfh.tblpurchaseorder.productID = ?)";
                parameters.Add(searchProduct.SelectedValue.ToString());
            }

            if (checkDrugstore.IsChecked.Value)
            {
                queryString += " AND (dbfh.tblpurchaseorder.drugstoreID = ?)";
                parameters.Add(searchDrugstore.SelectedValue.ToString());
            }

            if (chkDRNumber.IsChecked.Value)
            {
                queryString += " AND (drnumber = ?)";
                parameters.Add(searchDR.Text);
            }
            if (chkPONumber.IsChecked.Value)
            {
                queryString += " AND (ponumber = ?)";
                parameters.Add(searchPO.Text);
            }
            if (chkSINumber.IsChecked.Value)
            {
                queryString += " AND (sinumber = ?)";
                parameters.Add(searchSI.Text);
            }

            if (chkPaymentDate.IsChecked.Value)
            {
                queryString += " AND (paymentdate BETWEEN ? AND ?)";
                DateTime sdate = DateTime.Parse(searchPayDateFrom.Text);
                parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);
                sdate = DateTime.Parse(searchPayDateTo.Text);
                parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);
            }

            queryString += " ORDER BY dbfh.tblpurchaseorder.deliverydate DESC";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                purchaseOrderMod.ID = reader["ID"].ToString();
                purchaseOrderMod.DRNumber = reader["drnumber"].ToString();
                purchaseOrderMod.SINumber = reader["sinumber"].ToString();
                purchaseOrderMod.PONumber = reader["ponumber"].ToString();
                purchaseOrderMod.DrugstoreName = reader["drugstorename"].ToString();
                string temp = reader["paymentdate"].ToString();
                if (!string.IsNullOrEmpty(temp))
                {
                    DateTime dte = DateTime.Parse(temp);
                    if (dte.Year == 0001)
                    {
                        purchaseOrderMod.PaymentDate = "";
                    }
                    else
                    {
                        purchaseOrderMod.PaymentDate = dte.ToShortDateString();
                    }

                }
                temp = reader["deliverydate"].ToString();
                if (!string.IsNullOrEmpty(temp))
                {
                    DateTime dte = DateTime.Parse(temp);
                    if (dte.Year == 0001)
                    {
                        purchaseOrderMod.DeliveryDate = "";
                    }
                    else
                    {
                        purchaseOrderMod.DeliveryDate = dte.ToShortDateString();
                    }
                }
                purchaseOrderMod.DrugstoreID = reader["drugstoreID"].ToString();
                purchaseOrderMod.ProductID = reader["productID"].ToString();
                purchaseOrderMod.Quantity = reader["quantity"].ToString();
                purchaseOrderMod.isPaid = reader["isPaid"].ToString();
                purchaseOrderMod.Amount = Convert.ToDouble(reader["amount"].ToString()).ToString("N0");
                if (purchaseOrderMod.isPaid.Equals("1"))
                {
                    purchaseOrderMod.boolPaid = true;
                }
                else
                {
                    purchaseOrderMod.boolPaid = false;
                }
                lstPurchase.Add(purchaseOrderMod);
                purchaseOrderMod = new PurchaseOrderModel();
            }

            return lstPurchase;
        }

        private async Task<bool> checkFields()
        {
            bool ifAllCorrect = false;
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (string.IsNullOrEmpty(txtPO.Text))
            {
                await window.ShowMessageAsync("PO Number", "Please input PO number.");
            }
            else if (string.IsNullOrEmpty(txtSI.Text))
            {
                await window.ShowMessageAsync("SI Number", "Please provide SI number.");
            }
            else if (string.IsNullOrEmpty(txtDR.Text))
            {
                await window.ShowMessageAsync("DR Number", "Please select DR number.");
            }else if (string.IsNullOrEmpty(txtAmount.Text))
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
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (checkDate.IsChecked.Value || checkCategory.IsChecked.Value || checkDrugstore.IsChecked.Value ||
                chkDRNumber.IsChecked.Value || chkSINumber.IsChecked.Value || chkPONumber.IsChecked.Value ||
                chkPaymentDate.IsChecked.Value)
            {
                if (string.IsNullOrEmpty(searchDateFrom.Text) && string.IsNullOrEmpty(searchDateTo.Text) && checkDate.IsChecked.Value)
                {
                    await window.ShowMessageAsync("SEARCH", "Please complete value to search.");
                }else if(checkCategory.IsChecked.Value && searchProduct.SelectedItem == null)
                {
                    await window.ShowMessageAsync("SEARCH", "Please complete value to search.");
                }else if(checkDrugstore.IsChecked.Value && searchDrugstore.SelectedItem == null)
                {
                    await window.ShowMessageAsync("SEARCH", "Please complete value to search.");
                }else if(chkDRNumber.IsChecked.Value && string.IsNullOrEmpty(searchDR.Text))
                {
                    await window.ShowMessageAsync("SEARCH", "Please complete value to search.");
                }else if(chkPONumber.IsChecked.Value && string.IsNullOrEmpty(searchPO.Text))
                {
                    await window.ShowMessageAsync("SEARCH", "Please complete value to search.");
                }else if(chkSINumber.IsChecked.Value && string.IsNullOrEmpty(searchSI.Text))
                {
                    await window.ShowMessageAsync("SEARCH", "Please complete value to search.");
                }else if (chkPaymentDate.IsChecked.Value && (string.IsNullOrEmpty(searchPayDateFrom.Text))
                    && (string.IsNullOrEmpty(searchDateTo.Text)))
                {
                    await window.ShowMessageAsync("SEARCH", "Please complete value to search.");
                }
                else
                {
                    dgvPO.ItemsSource = search();
                }
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            checkDate.IsChecked = false;
            checkCategory.IsChecked = false;
            checkDrugstore.IsChecked = false;
            chkDRNumber.IsChecked = false;
            chkSINumber.IsChecked = false;
            chkPONumber.IsChecked = false;
            chkPaymentDate.IsChecked = false;
            dgvPO.ItemsSource = loadDataGridDetails();

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
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
            if (bl)
            {
                MessageDialogResult result;

                if (chkPaid.IsChecked.Value)
                {
                    result = await window.ShowMessageAsync("Purchase Order", "Is this transaction ALREADY PAID?", MessageDialogStyle.AffirmativeAndNegative);

                }else
                {
                    result = await window.ShowMessageAsync("Purchase Order", "Is this transaction STILL UNPAID?", MessageDialogStyle.AffirmativeAndNegative);
                }

                if (result == MessageDialogResult.Affirmative)
                {
                    updateRecord(purchaseOrderModel);
                    clearFields();
                    await window.ShowMessageAsync("UPDATE RECORD", "Record updated successfully!");
                    dgvPO.ItemsSource = loadDataGridDetails();
                }                
            }
        }
       
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool bl = await checkFields();
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
            if (bl)
            {
                if(checkIfExistingPO() || checkIfExistingSI())
                {
                    await window.ShowMessageAsync("SAVE RECORD", "PO or SI already exist. Please check/search record.");
                }else
                {
                    saveRecord();
                    clearFields();
                    await window.ShowMessageAsync("SAVE RECORD", "Record saved successfully!");
                    dgvPO.ItemsSource = loadDataGridDetails();
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
            ReportForm rf = new ReportForm(lstPurchaseOrderReports());
            rf.ShowDialog();
        }

        private void loadDrugstoreOnCombo()
        {
            conDB = new ConnectionDB();

            string queryString = "SELECT ID, drugstore, description, paymentdue FROM dbfh.tbldrugstores WHERE isDeleted = 0 order by description asc";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);
            comboDrugstore.Items.Clear();
            searchDrugstore.Items.Clear();
            while (reader.Read())
            {
                DrugstoreModel drg = new DrugstoreModel();
                drg.ID = reader["ID"].ToString();
                drg.DrugstoreName = reader["drugstore"].ToString();
                drg.Description = reader["description"].ToString();
                drg.PaymentDue = reader["paymentdue"].ToString();
                comboDrugstore.Items.Add(drg);
                searchDrugstore.Items.Add(drg);
            }
            conDB.closeConnection();
        }

        private void loadProductOnCombo()
        {
            conDB = new ConnectionDB();
            ProductModel prod = new ProductModel();

            string queryString = "SELECT ID, productname, description FROM dbfh.tblproducts WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            comboProduct.Items.Clear();
            searchProduct.Items.Clear();

            while (reader.Read())
            {
                prod.ID = reader["ID"].ToString();
                prod.ProductName = reader["productname"].ToString();
                prod.Description = reader["description"].ToString();

                comboProduct.Items.Add(prod);
                searchProduct.Items.Add(prod);
                prod = new ProductModel();
            }

            conDB.closeConnection();
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
        }

        private List<PurchaseOrderModel> lstPurchaseOrderReports()
        {
            conDB = new ConnectionDB();
            List<PurchaseOrderModel> lstPurchaseOrderModel = new List<PurchaseOrderModel>();
            PurchaseOrderModel pom = new PurchaseOrderModel();
            string queryString = "SELECT dbfh.tblpurchaseorder.ID, ponumber, sinumber, drnumber, paymentdate, " +
                "deliverydate, quantity, drugstoreID, productID, dbfh.tblproducts.productname, isPaid, amount," +
                " dbfh.tbldrugstores.description, dbfh.tbldrugstores.paymentdue AS 'terms', " +
                "dbfh.tblpurchaseorder.paymentduedate FROM ((dbfh.tblpurchaseorder INNER JOIN " +
                "dbfh.tbldrugstores ON tblpurchaseorder.drugstoreID = tbldrugstores.ID) INNER JOIN " +
                "dbfh.tblproducts ON dbfh.tblpurchaseorder.productID = tblproducts.ID) " +
                "WHERE dbfh.tblpurchaseorder.isDeleted = 0 order by deliverydate ASC";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);
            string temp = "";
            while (reader.Read())
            {
                pom.ID = reader["ID"].ToString();
                pom.DrugstoreName = reader["description"].ToString();
                pom.Quantity = reader["quantity"].ToString();
                pom.ProductName = reader["productname"].ToString();
                pom.Amount = reader["amount"].ToString();
                pom.SINumber = reader["sinumber"].ToString();
                temp = reader["deliverydate"].ToString();
                DateTime d1;
                if (!string.IsNullOrEmpty(temp) && !temp.Equals("0000-00-00"))
                {
                    d1 = DateTime.Parse(temp);
                    pom.DeliveryDate = d1.ToShortDateString();
                }else
                {
                    pom.DeliveryDate = "";
                }
               
                pom.Terms = reader["terms"].ToString();
                temp = reader["paymentduedate"].ToString();
                if (!string.IsNullOrEmpty(temp) && !temp.Equals("0000-00-00"))
                {
                    d1 = DateTime.Parse(temp);
                    pom.PaymentDueDate = d1.ToShortDateString();

                    //CHECK IF DATE IS BEYOND DUE DATE
                    DateTime dtePaymentDue = DateTime.Parse(pom.PaymentDueDate);
                    DateTime dteNow = DateTime.Now;

                    if (dteNow.Date >= dtePaymentDue)
                    {
                        pom.boolPaid = true;
                    }
                    else
                    {
                        pom.boolPaid = false;
                    }
                }
                else
                {
                    pom.PaymentDueDate = "";
                }
                pom.PONumber = reader["ponumber"].ToString();
                lstPurchaseOrderModel.Add(pom);
                pom = new PurchaseOrderModel();
            }

            conDB.closeConnection();

            return lstPurchaseOrderModel;
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            purchaseOrderModel = dgvPO.SelectedItem as PurchaseOrderModel;
            
            if(purchaseOrderModel != null)
            {
                txtDR.Text = purchaseOrderModel.DRNumber;
                txtSI.Text = purchaseOrderModel.SINumber;
                txtPO.Text = purchaseOrderModel.PONumber;
                txtQuantity.Text = purchaseOrderModel.Quantity;
                deliveryDate.Text = purchaseOrderModel.DeliveryDate;
                paymentDate.Text = purchaseOrderModel.PaymentDate;
                txtAmount.Text = purchaseOrderModel.Amount;
                paymentDueDate.Text = purchaseOrderModel.PaymentDueDate;
                if (purchaseOrderModel.boolPaid)
                {
                    chkPaid.IsChecked = true;
                }else
                {
                    chkPaid.IsChecked = false;
                }

                foreach (DrugstoreModel dsm in comboDrugstore.Items)
                {
                    if (dsm.ID.Equals(purchaseOrderModel.DrugstoreID))
                    {
                        comboDrugstore.SelectedItem = dsm;
                    }
                }

                foreach (ProductModel prM in comboProduct.Items)
                {
                    if (prM.ID.Equals(purchaseOrderModel.ProductID))
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

            DrugstoreModel dm = comboDrugstore.SelectedItem as DrugstoreModel;
            if (dm != null)
            {
                if (!string.IsNullOrEmpty(deliveryDate.Text))
                {
                    DateTime d1 = DateTime.Parse(deliveryDate.Text);

                    DateTime d2 = d1.AddDays(Convert.ToInt32(dm.PaymentDue));

                    paymentDueDate.Text = d2.ToShortDateString();
                }
            }
        }
    }
}
