using FHSales.Classes;
using FHSales.views;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace FHSales
{
    /// <summary>
    /// Interaction logic for ViewDRJuice.xaml
    /// </summary>
    public partial class ViewDRJuice : MetroWindow
    {
        public ViewDRJuice()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        string queryString = "";
        List<string> parameters;
        StocksInventoryJuice stocksInventoryView;
        public List<FHJuiceProductModel> lstFHProducts;
        AccountOrderModel accountOrder;
        bool ifEditConso = false;

        public ViewDRJuice(StocksInventoryJuice sij, AccountOrderModel ao, List<FHJuiceProductModel> lstFprod)
        {
            stocksInventoryView = sij;
            accountOrder = ao;
            lstFHProducts = lstFprod;
            InitializeComponent();

        }


        public ViewDRJuice(StocksInventoryJuice sij, AccountOrderModel ao, List<FHJuiceProductModel> lstFprod, bool blConso)
        {
            stocksInventoryView = sij;
            accountOrder = ao;
            lstFHProducts = lstFprod;
            ifEditConso = blConso;
            InitializeComponent();

        }


        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            loadClientOnCombo();
            loadPaymentOnCombo();
            loadRepOnCombo();
            if (accountOrder != null && !ifEditConso)
            {
                initiateValues();
                btnUpdate.Visibility = Visibility.Hidden;
            }
            else if (accountOrder != null && ifEditConso)
            {
                initiateValues();
                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Visible;
            }
        }

        private void initiateValues()
        {
            txtDRNo.Text = accountOrder.DrNo;
            deliveryDate.Text = accountOrder.DeliveryDate;
            txtTerms.Text = accountOrder.Terms;
            paymentDueDate.Text = accountOrder.PaymentDueDate;
            txtTotal.Text = accountOrder.Total;

            foreach (ClientModel c in cmbClient.Items)
            {
                if (accountOrder.ClientID.Equals(c.ID))
                {
                    cmbClient.SelectedItem = c;
                }
            }

            foreach (RepresentativeModel r in cmbRep.Items)
            {
                if (accountOrder.RepID.Equals(r.ID))
                {
                    cmbRep.SelectedItem = r;
                }
            }

            foreach (PaymentModel p in cmbPayment.Items)
            {
                if (accountOrder.PaymentModeID.Equals(p.ID))
                {
                    cmbPayment.SelectedItem = p;
                }
            }

            if (accountOrder.isPaid.Equals("1"))
            {
                chkPaid.IsChecked = true;
            }
            else
            {
                chkPaid.IsChecked = false;
            }
        }

        private void loadClientOnCombo()
        {
            conDB = new ConnectionDB();
            queryString = "SELECT ID, accountname, address, contactnumber, contactperson FROM dbjuiceacct.tblaccounts " +
                "WHERE isDeleted = 0";
            ClientModel client = new ClientModel();
            cmbClient.Items.Clear();
            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                client.ID = reader["ID"].ToString();
                client.ClientName = reader["accountname"].ToString();
                client.Address = reader["address"].ToString();
                client.ContactNumber = reader["contactnumber"].ToString();
                client.ContactPerson = reader["contactperson"].ToString();
                cmbClient.Items.Add(client);
                client = new ClientModel();
            }
            conDB.closeConnection();
        }

        private void loadPaymentOnCombo()
        {
            conDB = new ConnectionDB();
            queryString = "SELECT ID, paymentmode, description FROM dbjuiceacct.tblpaymentmode " +
                "WHERE isDeleted = 0";
            PaymentModel payment = new PaymentModel();
            cmbPayment.Items.Clear();
            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                payment.ID = reader["ID"].ToString();
                payment.PaymentMode = reader["paymentmode"].ToString();
                payment.Description = reader["description"].ToString();
                cmbPayment.Items.Add(payment);
                payment = new PaymentModel();
            }
            conDB.closeConnection();
        }

        private void loadRepOnCombo()
        {
            conDB = new ConnectionDB();
            queryString = "SELECT ID, name, telephonenumber FROM dbjuiceacct.tblrepresentative " +
                "WHERE isDeleted = 0";
            RepresentativeModel repp = new RepresentativeModel();
            cmbRep.Items.Clear();
            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                repp.ID = reader["ID"].ToString();
                repp.Name = reader["name"].ToString();
                repp.TelephoneNumber = reader["telephonenumber"].ToString();
                cmbRep.Items.Add(repp);
                repp = new RepresentativeModel();
            }
            conDB.closeConnection();
        }

        private string saveRecordToNewTable()
        {
            string strID = "";

            conDB = new ConnectionDB();
            queryString = "INSERT INTO dbjuiceacct.tblaccountorderconso (clientID, paymentmodeID, drNo, deliverydate, " +
                "terms, paymentduedate, total, isPaid, remainingbalance, repID, actorderID, isDeleted) VALUES (?,?,?,?,?,?,?,?,?,?,?,0)";

            parameters = new List<string>();
            parameters.Add(cmbClient.SelectedValue.ToString());
            parameters.Add(cmbPayment.SelectedValue.ToString());
            parameters.Add(txtDRNo.Text);

            DateTime pdate = DateTime.Parse(deliveryDate.Text);
            parameters.Add(pdate.Year + "/" + pdate.Month + "/" + pdate.Day);

            parameters.Add(txtTerms.Text);

            pdate = DateTime.Parse(paymentDueDate.Text);
            parameters.Add(pdate.Year + "/" + pdate.Month + "/" + pdate.Day);

            parameters.Add(txtTotal.Text);

            if (chkPaid.IsChecked.Value)
            {
                parameters.Add("1");
            }
            else
            {
                parameters.Add("0");
            }

            parameters.Add(txtRemainingBalance.Text);

            parameters.Add(cmbRep.SelectedValue.ToString());
            parameters.Add(accountOrder.ID);

            conDB.AddRecordToDatabase(queryString, parameters);

            MySqlDataReader reader = conDB.getSelectConnection("select ID from dbjuiceacct.tblaccountorderconso order by ID desc limit 1", null);

            while (reader.Read())
            {
                strID = reader["ID"].ToString();

            }

            conDB.closeConnection();
            return strID;
        }

        private async Task<bool> checkFields()
        {
            bool ifCorrect = false;

            if (cmbClient.SelectedItem == null)
            {
                await this.ShowMessageAsync("Client", "Please select value");
            }
            else if (string.IsNullOrEmpty(txtDRNo.Text))
            {
                await this.ShowMessageAsync("DR", "Please input value");
            }
            else if (string.IsNullOrEmpty(deliveryDate.Text))
            {
                await this.ShowMessageAsync("Delivery Date", "Please input value");
            }
            else if (cmbPayment.SelectedItem == null)
            {
                await this.ShowMessageAsync("Payment mode", "Pleasea select value");
            }
            else if (string.IsNullOrEmpty(txtTerms.Text))
            {
                await this.ShowMessageAsync("Terms", "Please input value");
            }
            else if (string.IsNullOrEmpty(paymentDueDate.Text))
            {
                await this.ShowMessageAsync("Payment Date", "Please input value");
            }
            else if (cmbRep.SelectedItem == null)
            {
                await this.ShowMessageAsync("Representative/Distributor", "Pleasea select value");
            }
            else
            {
                ifCorrect = true;
            }

            return ifCorrect;
        }

        private void btnAddProducts_Click(object sender, RoutedEventArgs e)
        {
            if (ifEditConso)
            {
                AddProductJuiceAcct addJuice = new AddProductJuiceAcct(this, lstFHProducts, ifEditConso);
                addJuice.ShowDialog();
            }
            else
            {
                AddProductJuiceAcct addJuice = new AddProductJuiceAcct(this, lstFHProducts);
                addJuice.ShowDialog();
            }

            
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();

            if (x)
            {
                int iID = saveRecord();
                saveProductsOnRecords(iID);
                clearFields();
                updateOriginalRecordtoConsolidated();
                stocksInventoryView.dgvConsolidatedDR.ItemsSource = loadConsolidatedDRS();
                stocksInventoryView.dgvAccounts.ItemsSource = loadDREncoded();
                //dgvAccounts.ItemsSource = loadDatagridDetails();
                await this.ShowMessageAsync("SAVE RECORD", "Record saved successfully!");
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txtTerms_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void txtTerms_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {

        }

        private void txtRemainingBalance_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {

        }

        private int saveRecord()
        {
            int strID = 0;

            conDB = new ConnectionDB();
            queryString = "INSERT INTO dbjuiceacct.tblaccountorderconso (clientID, paymentmodeID, drNo, deliverydate, " +
                "terms, paymentduedate, total, isPaid, remainingbalance, repID, actorderID,  isDeleted) VALUES (?,?,?,?,?,?,?,?,?,?,?,0)";

            parameters = new List<string>();
            parameters.Add(cmbClient.SelectedValue.ToString());
            parameters.Add(cmbPayment.SelectedValue.ToString());
            parameters.Add(txtDRNo.Text);
            DateTime pdate = DateTime.Parse(deliveryDate.Text);
            parameters.Add(pdate.Year + "/" + pdate.Month + "/" + pdate.Day);
            parameters.Add(txtTerms.Text);
            pdate = DateTime.Parse(paymentDueDate.Text);
            parameters.Add(pdate.Year + "/" + pdate.Month + "/" + pdate.Day);
            parameters.Add(txtTotal.Text);

            if (chkPaid.IsChecked.Value)
            {
                parameters.Add("1");
            }
            else
            {
                parameters.Add("0");
            }
            parameters.Add(txtRemainingBalance.Text);
            parameters.Add(cmbRep.SelectedValue.ToString());
            parameters.Add(accountOrder.ID);
            conDB.AddRecordToDatabase(queryString, parameters);

            MySqlDataReader reader = conDB.getSelectConnection("select ID from dbjuiceacct.tblaccountorderconso order by ID desc limit 1", null);

            while (reader.Read())
            {
                strID = Convert.ToInt32(reader["ID"].ToString());

            }
            conDB.closeConnection();

            return strID;
        }

        private void saveProductsOnRecords(int iOrderID)
        {
            conDB = new ConnectionDB();

            foreach (FHJuiceProductModel pp in lstFHProducts)
            {
                queryString = "INSERT INTO dbjuiceacct.tblproductsorderedconso (orderID, productID, qty, price, remarks, isDeleted) " +
                    "VALUES (?,?,?,?,?,0)";
                parameters = new List<string>();
                parameters.Add(iOrderID.ToString());
                parameters.Add(pp.ID);
                parameters.Add(pp.Qty);
                parameters.Add(pp.Price);
                parameters.Add(pp.Remarks);

                conDB.AddRecordToDatabase(queryString, parameters);

            }
            conDB.closeConnection();
        }

        private void updateOriginalRecordtoConsolidated()
        {
            conDB = new ConnectionDB();
            queryString = "UPDATE dbjuiceacct.tblaccountorder SET isConso = 1 WHERE ID = ?";

            parameters = new List<string>();
            parameters.Add(accountOrder.ID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();

        }

        private List<AccountOrderModel> loadConsolidatedDRS()
        {
            conDB = new ConnectionDB();
            queryString = "SELECT dbjuiceacct.tblaccountorderconso.ID, drNo, clientID, dbjuiceacct.tblaccounts.accountname, " +
                "deliverydate, terms, paymentduedate, total, repID, paymentmodeID, isPaid FROM (dbjuiceacct.tblaccountorderconso INNER JOIN " +
                "dbjuiceacct.tblaccounts ON dbjuiceacct.tblaccountorderconso.clientID = dbjuiceacct.tblaccounts.ID) " +
                "WHERE dbjuiceacct.tblaccountorderconso.isDeleted = 0 AND dbjuiceacct.tblaccounts.isDeleted = 0";

            List<AccountOrderModel> lstDRs = new List<AccountOrderModel>();

            AccountOrderModel drs = new AccountOrderModel();

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                drs.ID = reader["ID"].ToString();
                drs.DrNo = reader["drNo"].ToString();
                drs.ClientName = reader["accountname"].ToString();
                drs.ClientID = reader["clientID"].ToString();
                DateTime pdate = DateTime.Parse(reader["deliverydate"].ToString());
                drs.DeliveryDate = pdate.ToShortDateString();

                pdate = DateTime.Parse(reader["paymentduedate"].ToString());
                drs.PaymentDueDate = pdate.ToShortDateString();
                drs.Terms = reader["terms"].ToString();
                drs.Total = reader["total"].ToString();
                drs.RepID = reader["repID"].ToString();
                drs.isPaid = reader["isPaid"].ToString();
                drs.PaymentModeID = reader["paymentmodeID"].ToString();

                lstDRs.Add(drs);
                drs = new AccountOrderModel();
            }

            conDB.closeConnection();

            return lstDRs;
        }

        private List<AccountOrderModel> loadDREncoded()
        {
            conDB = new ConnectionDB();
            queryString = "SELECT dbjuiceacct.tblaccountorder.ID, drNo, clientID, dbjuiceacct.tblaccounts.accountname, " +
                "deliverydate, terms, paymentduedate, total, repID, paymentmodeID, isPaid FROM (dbjuiceacct.tblaccountorder INNER JOIN " +
                "dbjuiceacct.tblaccounts ON dbjuiceacct.tblaccountorder.clientID = dbjuiceacct.tblaccounts.ID) " +
                "WHERE dbjuiceacct.tblaccountorder.isDeleted = 0 AND dbjuiceacct.tblaccounts.isDeleted = 0 AND dbjuiceacct.tblaccountorder.isConso = 0";

            List<AccountOrderModel> lstDRs = new List<AccountOrderModel>();

            AccountOrderModel drs = new AccountOrderModel();
            //parameters = new List<string>();
            // DateTime dteNow = DateTime.Now;
            //DateTime dteFirstDay = new DateTime(dteNow.Year, dteNow.Month, 1);

            // parameters.Add(dteFirstDay.Year + "-" + dteFirstDay.Month + "-" + 1);
            //parameters.Add(dteNow.Year + "-" + dteNow.Month + "-" + dteNow.Day);

            //dateFrom.Text = dteFirstDay.ToShortDateString();
            // dateTo.Text = dteNow.ToShortDateString();

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                drs.ID = reader["ID"].ToString();
                drs.DrNo = reader["drNo"].ToString();
                drs.ClientName = reader["accountname"].ToString();
                drs.ClientID = reader["clientID"].ToString();
                DateTime pdate = DateTime.Parse(reader["deliverydate"].ToString());
                drs.DeliveryDate = pdate.ToShortDateString();

                pdate = DateTime.Parse(reader["paymentduedate"].ToString());
                drs.PaymentDueDate = pdate.ToShortDateString();
                drs.Terms = reader["terms"].ToString();
                drs.Total = reader["total"].ToString();
                drs.RepID = reader["repID"].ToString();
                drs.isPaid = reader["isPaid"].ToString();
                drs.PaymentModeID = reader["paymentmodeID"].ToString();

                lstDRs.Add(drs);
                drs = new AccountOrderModel();
            }

            conDB.closeConnection();

            //lstDRCounts = getDRCountOnClients(dteFirstDay.Year + "-" + dteFirstDay.Month + "-" + 1,
            //    dteNow.Year + "-" + dteNow.Month + "-" + dteNow.Day);

            //foreach (ClientModel cc in lstClients)
            //{
            //    foreach (ClientModel drCC in lstDRCounts)
            //    {
            //        if (cc.ClientID.Equals(drCC.ClientID))
            //        {
            //            cc.DRCount = drCC.DRCount;
            //        }
            //    }
            //}

            return lstDRs;
        }

        private void updateRecordConso(string strRecID)
        {
            conDB = new ConnectionDB();
            queryString = "UPDATE dbjuiceacct.tblaccountorderconso SET clientID = ?, paymentmodeID = ?, drNo = ?, " +
                "deliverydate = ?, terms = ?, paymentduedate = ?, total = ?, repID = ?, isPaid = ?, remainingbalance = ? WHERE " +
                "ID = ?";

            parameters = new List<string>();
            parameters.Add(cmbClient.SelectedValue.ToString());
            parameters.Add(cmbPayment.SelectedValue.ToString());
            parameters.Add(txtDRNo.Text);
            DateTime pdate = DateTime.Parse(deliveryDate.Text);
            parameters.Add(pdate.Year + "/" + pdate.Month + "/" + pdate.Day);
            parameters.Add(txtTerms.Text);
            pdate = DateTime.Parse(paymentDueDate.Text);
            parameters.Add(pdate.Year + "/" + pdate.Month + "/" + pdate.Day);
            parameters.Add(txtTotal.Text);
            parameters.Add(cmbRep.SelectedValue.ToString());
            if (chkPaid.IsChecked.Value)
            {
                parameters.Add("1");
            }
            else
            {
                parameters.Add("0");
            }
            parameters.Add(txtRemainingBalance.Text);
            parameters.Add(strRecID);
            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
        }

        private void updateProductsOnRecordsConso(string iOrderID)
        {
            conDB = new ConnectionDB();
            foreach (FHJuiceProductModel pp in lstFHProducts)
            {
                if (!pp.NewlyAdded)
                {
                    queryString = "UPDATE dbjuiceacct.tblproductsorderedconso SET orderID = ?, productID = ?, qty = ?, price = ?, " +
                 "remarks = ? WHERE ID = ?";
                    parameters = new List<string>();
                    parameters.Add(pp.OrderID);
                    parameters.Add(pp.ID);
                    parameters.Add(pp.Qty);
                    parameters.Add(pp.Price);
                    parameters.Add(pp.Remarks);
                    parameters.Add(pp.RecordID);

                    conDB.AddRecordToDatabase(queryString, parameters);
                }
                else
                {
                    queryString = "INSERT INTO dbjuiceacct.tblproductsorderedconso (orderID, productID, qty, price, remarks, isDeleted) " +
                   "VALUES (?,?,?,?,?,0)";

                    parameters = new List<string>();

                    parameters.Add(iOrderID);
                    parameters.Add(pp.ID);
                    parameters.Add(pp.Qty);
                    parameters.Add(pp.Price);
                    parameters.Add(pp.Remarks);
                    conDB.AddRecordToDatabase(queryString, parameters);
                }

            }
            conDB.closeConnection();

        }


        private void clearFields()
        {
            cmbClient.SelectedItem = null;
            txtDRNo.Text = "";
            deliveryDate.Text = "";
            cmbPayment.SelectedItem = null;
            txtTerms.Text = "";
            paymentDueDate.Text = "";
            txtTotal.Text = "";
            txtRemainingBalance.Text = "";
            //dgvOrderHistory.IsEnabled = true;
            cmbRep.SelectedItem = null;
            lstFHProducts = new List<FHJuiceProductModel>();

        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();

            if (x)
            {
                updateRecordConso(accountOrder.ID);
                updateProductsOnRecordsConso(accountOrder.ID);
                clearFields();
                btnUpdate.Visibility = Visibility.Hidden;
                btnSave.Visibility = Visibility.Visible;
                await this.ShowMessageAsync("UPDATE RECORD", "Record updated successfully!");
                this.Close();
            }
        }


    }
}
