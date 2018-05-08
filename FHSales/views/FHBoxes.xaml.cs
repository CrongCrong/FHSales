using FHSales.Classes;
using MahApps.Metro.Controls.Dialogs;
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
    /// Interaction logic for FHBoxes.xaml
    /// </summary>
    public partial class FHBoxes : UserControl
    {
        public FHBoxes()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        DirectSalesModel selected;
        public List<ProductModel> lstProductModel = new List<ProductModel>();


        private void btnEditDirectSales_Click(object sender, RoutedEventArgs e)
        {
            selected = dgvDirectSales.SelectedItem as DirectSalesModel;

            if (selected != null)
            {
                //DirectSalesDetails ds = new DirectSalesDetails(this, selected);
                // ds.ShowDialog();
                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Visible;

                deliveryDateDS.Text = selected.DeliveryDate;
                txtClientName.Text = selected.ClientName;
                txtExpenses.Text = selected.Expenses.ToString();
                txtTotalPrice.Text = selected.TotalPrice.ToString();

                loadCashBankonCombo(selected);
                loadCourierCombo(selected);
                loadProductsOnList(selected.ID);
                loadOfficeSalesCombo(selected);
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            searchDateFrom.IsEnabled = false;
            searchDateTo.IsEnabled = false;
            searchClient.IsEnabled = false;
            searchBank.IsEnabled = false;

            dgvDirectSales.ItemsSource = loadDataGridDetailsDirectSales();
            loadCashBankonCombo();
            loadCourierCombo();
            loadOfficeSalesCombo();
            btnUpdate.Visibility = Visibility.Hidden;
            if ((Convert.ToInt32(UserModel.UserType) == (int)UserTypeEnum.DIRECTSALES_ADMIN) ||
                (Convert.ToInt32(UserModel.UserType) == (int)UserTypeEnum.ADMIN))
            {
                btnEditDirectSales.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Visible;
                btnUpdate.Visibility = Visibility.Hidden;
                btnCancel.Visibility = Visibility.Visible;
            }
            else
            {
                btnEditDirectSales.Visibility = Visibility.Hidden;
                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Hidden;
                btnCancel.Visibility = Visibility.Hidden;
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool bl = await checkFields();
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (bl)
            {
                if(lstProductModel.Count <= 0)
                {
                    await window.ShowMessageAsync("SAVE ERROR", "PRODUCT LIST IS EMPTY.");
                }else
                {
                    int id = saveDirectSalesRecord();
                    saveProductsOnList(id.ToString());
                    await window.ShowMessageAsync("SAVE RECORD", "Record saved successfully!");
                    dgvDirectSales.ItemsSource = loadDataGridDetailsDirectSales();
                    clearFields();
                }
                
            }
        }

        private List<DirectSalesModel> loadDataGridDetailsDirectSales()
        {
            conDB = new ConnectionDB();
            List<DirectSalesModel> lstDirectSales = new List<DirectSalesModel>();
            DirectSalesModel directSales = new DirectSalesModel();

            string queryString = "SELECT dbfh.tbldirectsales.ID, clientname, quantity, cashbankID, bankname, courierID, " +
                "couriername, totalprice, deliverydate, expenses, officeID FROM ((dbfh.tbldirectsales INNER JOIN dbfh.tblbank ON " +
                "dbfh.tbldirectsales.cashbankID = dbfh.tblbank.ID) INNER JOIN dbfh.tblcourier ON " +
                "dbfh.tbldirectsales.courierID = dbfh.tblcourier.ID) WHERE dbfh.tbldirectsales.salestypeID = 1 AND dbfh.tbldirectsales.isDeleted = 0 ORDER BY deliverydate DESC";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                directSales.ID = reader["ID"].ToString();
                directSales.ClientName = reader["clientname"].ToString();
                directSales.Quantity = reader["quantity"].ToString();
                directSales.CashBankID = reader["cashbankID"].ToString();
                directSales.CashBankName = reader["bankname"].ToString();
                directSales.CourierID = reader["courierID"].ToString();
                directSales.CourierName = reader["couriername"].ToString();
                directSales.TotalPrice = Convert.ToInt32(reader["totalprice"].ToString());
                directSales.OfficeSalesID = reader["officeID"].ToString();
                DateTime dte = DateTime.Parse(reader["deliveryDate"].ToString());
                directSales.DeliveryDate = dte.ToShortDateString();

                directSales.Expenses = Convert.ToInt32(reader["expenses"].ToString());

                lstDirectSales.Add(directSales);
                directSales = new DirectSalesModel();
            }
            conDB.closeConnection();
            lblRecords.Content = lstDirectSales.Count + " RECORDS FOUND.";
            return lstDirectSales;

        }

        private void loadOfficeSalesCombo()
        {
            conDB = new ConnectionDB();
            SalesOfficeModel office = new SalesOfficeModel();

            string queryString = "SELECT ID, officename, description FROM dbfh.tblsalesoffice WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);
            cmbSalesOffice.Items.Clear();
            while (reader.Read())
            {
                office.ID = reader["ID"].ToString();
                office.OfficeName = reader["officename"].ToString();
                office.Description = reader["description"].ToString();
                cmbSalesOffice.Items.Add(office);
                office = new SalesOfficeModel();
            }

            conDB.closeConnection();
        }

        private void loadOfficeSalesCombo(DirectSalesModel dsm)
        {
            conDB = new ConnectionDB();
            SalesOfficeModel office = new SalesOfficeModel();

            string queryString = "SELECT ID, officename, description FROM dbfh.tblsalesoffice WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);
            cmbSalesOffice.Items.Clear();
            while (reader.Read())
            {
                office.ID = reader["ID"].ToString();
                office.OfficeName = reader["officename"].ToString();
                office.Description = reader["description"].ToString();

                cmbSalesOffice.Items.Add(office);
                if (dsm.OfficeSalesID.Equals(office.ID))
                {
                    cmbSalesOffice.SelectedItem = office;
                }
                office = new SalesOfficeModel();
            }

            conDB.closeConnection();
        }

        private void loadCashBankonCombo()
        {
            conDB = new ConnectionDB();
            BankModel bankCash = new BankModel();

            string queryString = "SELECT ID, bankname, description FROM dbfh.tblbank WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);
            cmbCashBank.Items.Clear();
            searchBank.Items.Clear();
            while (reader.Read())
            {
                bankCash.ID = reader["ID"].ToString();
                bankCash.BankName = reader["bankname"].ToString();
                bankCash.Description = reader["description"].ToString();

                cmbCashBank.Items.Add(bankCash);
                searchBank.Items.Add(bankCash);
                bankCash = new BankModel();
            }

            conDB.closeConnection();

        }

        private void loadCashBankonCombo(DirectSalesModel directSales)
        {
            conDB = new ConnectionDB();
            BankModel bankCash = new BankModel();

            string queryString = "SELECT ID, bankname, description FROM dbfh.tblbank WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);
            cmbCashBank.Items.Clear();
            while (reader.Read())
            {
                bankCash.ID = reader["ID"].ToString();
                bankCash.BankName = reader["bankname"].ToString();
                bankCash.Description = reader["description"].ToString();

                cmbCashBank.Items.Add(bankCash);
                if (directSales.CashBankID.Equals(bankCash.ID))
                {
                    cmbCashBank.SelectedItem = bankCash;
                }
                bankCash = new BankModel();
            }

            conDB.closeConnection();

        }

        private void loadCourierCombo()
        {
            conDB = new ConnectionDB();
            CourierModel courier = new CourierModel();

            string queryString = "SELECT ID, couriername, description FROM dbfh.tblcourier WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);
            cmbCourier.Items.Clear();
            while (reader.Read())
            {
                courier.ID = reader["ID"].ToString();
                courier.CourierName = reader["couriername"].ToString();
                courier.Description = reader["description"].ToString();

                cmbCourier.Items.Add(courier);

                courier = new CourierModel();
            }

            conDB.closeConnection();
        }

        private void loadCourierCombo(DirectSalesModel directSales)
        {
            conDB = new ConnectionDB();
            CourierModel courier = new CourierModel();

            string queryString = "SELECT ID, couriername, description FROM dbfh.tblcourier WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);
            cmbCourier.Items.Clear();
            while (reader.Read())
            {
                courier.ID = reader["ID"].ToString();
                courier.CourierName = reader["couriername"].ToString();
                courier.Description = reader["description"].ToString();

                cmbCourier.Items.Add(courier);

                if (directSales.CourierID.Equals(courier.ID))
                {
                    cmbCourier.SelectedItem = courier;
                }

                courier = new CourierModel();
            }

            conDB.closeConnection();
        }

        private int saveDirectSalesRecord()
        {
            conDB = new ConnectionDB();
            int x = 0;
            string queryString = "INSERT INTO dbfh.tbldirectsales (clientname, cashbankID, courierID, totalprice, " +
                "deliverydate, expenses, salestypeID, officeID, isDeleted) VALUES (?,?,?,?,?,?,?,?,0)";

            List<string> parameters = new List<string>();
            parameters.Add(txtClientName.Text);
            parameters.Add(cmbCashBank.SelectedValue.ToString());
            parameters.Add(cmbCourier.SelectedValue.ToString());
            parameters.Add(txtTotalPrice.Text);
            DateTime delDate = DateTime.Parse(deliveryDateDS.Text);
            parameters.Add(delDate.Year + "-" + delDate.Month + "-" + delDate.Day);
            parameters.Add(txtExpenses.Text);
            parameters.Add("1");
            parameters.Add(cmbSalesOffice.SelectedValue.ToString());
            conDB.AddRecordToDatabase(queryString, parameters);

            MySqlDataReader reader = conDB.getSelectConnection("select ID from dbfh.tbldirectsales order by ID desc limit 1", null);

            while (reader.Read())
            {
                x = Convert.ToInt32(reader["ID"].ToString());
            }

            conDB.closeConnection();
            return x;
        }

        private void updateDirectSalesRecord(DirectSalesModel directSalesModel)
        {
            conDB = new ConnectionDB();
            string queryString = "UPDATE dbfh.tbldirectsales SET deliverydate = ?, clientname = ?, cashbankID = ?, " +
                "courierID = ?, expenses = ?, totalprice = ?, officeID = ? WHERE ID = ?";

            List<string> parameters = new List<string>();
            DateTime date = DateTime.Parse(deliveryDateDS.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            parameters.Add(txtClientName.Text);
            parameters.Add(cmbCashBank.SelectedValue.ToString());
            parameters.Add(cmbCourier.SelectedValue.ToString());
            parameters.Add(txtExpenses.Text);
            parameters.Add(txtTotalPrice.Text);
            parameters.Add(cmbSalesOffice.SelectedValue.ToString());
            parameters.Add(directSalesModel.ID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();

        }

        private void saveProductsOnList(string ID)
        {
            string query = "";
            conDB = new ConnectionDB();
            foreach(ProductModel p in lstProductModel)
            {
                query = "INSERT INTO dbfh.tblproductsordered (productID, salesID, qty, totalamt, isDeleted) VALUES (?,?,?,?,0)";
                List<string> parameters = new List<string>();

                parameters.Add(p.ID);
                parameters.Add(ID);
                parameters.Add(p.Quantity);
                parameters.Add(p.TotalAmount);
                conDB.AddRecordToDatabase(query, parameters);
            }

        }

        private void updateProductsOnList(string strSalesID)
        {
            string query = "";
            conDB = new ConnectionDB();
            foreach (ProductModel p in lstProductModel)
            {
                if (!p.newlyAdded)
                {
                    query = "UPDATE dbfh.tblproductsordered SET productID = ?, qty = ?, totalamt = ?, isDeleted = ? WHERE ID = ?";
                    List<string> parameters = new List<string>();

                    parameters.Add(p.ProductID);
                    parameters.Add(p.Quantity);
                    parameters.Add(p.TotalAmount);
                    parameters.Add(p.isDeleted);
                    parameters.Add(p.ID);
                    conDB.AddRecordToDatabase(query, parameters);
                }else
                {
                    query = "INSERT INTO dbfh.tblproductsordered (productID, salesID, qty, totalamt, isDeleted) VALUES (?,?,?,?,0)";
                    List<string> parameters = new List<string>();

                    parameters.Add(p.ProductID);
                    parameters.Add(strSalesID);
                    parameters.Add(p.Quantity);
                    parameters.Add(p.TotalAmount);
                    conDB.AddRecordToDatabase(query, parameters);
                }
                
            }
        }

        private void loadProductsOnList(string ID)
        {
            conDB = new ConnectionDB();
            lstProductModel = new List<ProductModel>();
            ProductModel prodM = new ProductModel();

            string queryString = "SELECT dbfh.tblproductsordered.ID, productID, salesID, qty, totalamt, description, dbfh.tblproductsordered.isDeleted " +
                "FROM((dbfh.tblproductsordered INNER JOIN dbfh.tblproducts ON " +
                "dbfh.tblproductsordered.productID = dbfh.tblproducts.ID) " +
                "INNER JOIN dbfh.tbldirectsales ON dbfh.tblproductsordered.salesID = dbfh.tbldirectsales.ID) " +
                "WHERE dbfh.tblproducts.isDeleted = 0 AND dbfh.tbldirectsales.isDeleted = 0 AND dbfh.tblproductsordered.isDeleted = 0" +
                " AND dbfh.tblproductsordered.salesID = ?";

            List<string> parameters = new List<string>();
            parameters.Add(ID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                prodM.ID = reader["ID"].ToString();
                prodM.ProductID = reader["productID"].ToString();
                prodM.SalesID = reader["salesID"].ToString();
                prodM.ProductName = reader["description"].ToString();
                prodM.Quantity = reader["qty"].ToString();
                prodM.TotalAmount = reader["totalamt"].ToString();
                prodM.isDeleted = reader["isDeleted"].ToString();
                lstProductModel.Add(prodM);
                prodM = new ProductModel();
            }
            
            conDB.closeConnection();
        }

        private List<DirectSalesModel> search()
        {
            conDB = new ConnectionDB();

            List<DirectSalesModel> lstDirectSales = new List<DirectSalesModel>();
            DirectSalesModel directSales = new DirectSalesModel();
            string queryString = "SELECT dbfh.tbldirectsales.ID, clientname, quantity, cashbankID, bankname, courierID, " +
                "couriername, totalprice, deliverydate, expenses, officeID FROM ((dbfh.tbldirectsales INNER JOIN dbfh.tblbank ON " +
                "dbfh.tbldirectsales.cashbankID = dbfh.tblbank.ID) INNER JOIN dbfh.tblcourier ON " +
                "dbfh.tbldirectsales.courierID = dbfh.tblcourier.ID) WHERE dbfh.tbldirectsales.isDeleted = 0 AND dbfh.tbldirectsales.salestypeID = 1";
            List<string> parameters = new List<string>();
            if (checkDate.IsChecked == true)
            {
                queryString += " AND (deliverydate BETWEEN ? AND ?)";
                DateTime sdate = DateTime.Parse(searchDateFrom.Text);
                parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);
                sdate = DateTime.Parse(searchDateTo.Text);
                parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);
            }

            if (checkClient.IsChecked == true)
            {
                queryString += " AND (clientname LIKE '%" + searchClient.Text + "%')";
            }

            if (checkBank.IsChecked == true)
            {
                queryString += " AND (dbfh.tbldirectsales.cashbankID = ?)";
                parameters.Add(searchBank.SelectedValue.ToString());
            }

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                directSales.ID = reader["ID"].ToString();
                directSales.ClientName = reader["clientname"].ToString();
                directSales.Quantity = reader["quantity"].ToString();
                directSales.CashBankID = reader["cashbankID"].ToString();
                directSales.CashBankName = reader["bankname"].ToString();
                directSales.CourierID = reader["courierID"].ToString();
                directSales.CourierName = reader["couriername"].ToString();
                directSales.OfficeSalesID = reader["officeID"].ToString();
                directSales.TotalPrice = Convert.ToInt32(reader["totalprice"].ToString());

                DateTime dte = DateTime.Parse(reader["deliveryDate"].ToString());
                directSales.DeliveryDate = dte.ToShortDateString();

                directSales.Expenses = Convert.ToInt32(reader["expenses"].ToString());

                lstDirectSales.Add(directSales);
                directSales = new DirectSalesModel();
            }
            conDB.closeConnection();
            lblRecords.Content = lstDirectSales.Count + " RECORDS FOUND.";
            return lstDirectSales;
        }

        private async Task<bool> checkFields()
        {
            bool ifAllCorrect = false;
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (string.IsNullOrEmpty(deliveryDateDS.Text))
            {
                await window.ShowMessageAsync("Date", "Please select date.");
            }
            else if (string.IsNullOrEmpty(txtClientName.Text))
            {
                await window.ShowMessageAsync("Client Name", "Please provide client name.");
            }
            else if (cmbCashBank.SelectedItem == null)
            {
                await window.ShowMessageAsync("CASH/BANK", "Please select if bank or cash");
            }
            else if (cmbCourier.SelectedItem == null)
            {
                await window.ShowMessageAsync("COURIER", "Please select courier.");
            }
            else if (string.IsNullOrEmpty(txtExpenses.Text))
            {
                await window.ShowMessageAsync("EXPENSES", "Please provide expenses value.");
            }
            else if (string.IsNullOrEmpty(txtTotalPrice.Text))
            {
                await window.ShowMessageAsync("TOTAL PRICE", "Please provide total price value.");
            }
            else
            {

                ifAllCorrect = true;
            }

            return ifAllCorrect;
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool blCorrect = await checkFields();
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (blCorrect)
            {
                updateDirectSalesRecord(selected);
                updateProductsOnList(selected.ID);
                clearFields();
                await window.ShowMessageAsync("UPDATE RECORD", "Record updated successfully!");
                dgvDirectSales.ItemsSource = loadDataGridDetailsDirectSales();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (btnUpdate.Visibility == Visibility.Hidden)
            {
                clearFields();
                lstProductModel = new List<ProductModel>();
            }
            else
            {
                clearFields();

                btnSave.Visibility = Visibility.Visible;
                btnUpdate.Visibility = Visibility.Hidden;
                lstProductModel = new List<ProductModel>();
            }
        }

        private void clearFields()
        {
            deliveryDateDS.Text = "";
            txtClientName.Text = "";
            txtExpenses.Text = "";
            txtTotalPrice.Text = "";

            cmbCashBank.SelectedItem = null;
            cmbCourier.SelectedItem = null;
            cmbSalesOffice.SelectedItem = null;
        }

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (checkDate.IsChecked == true || checkClient.IsChecked == true || checkBank.IsChecked == true)
            {
                if ((string.IsNullOrEmpty(searchDateFrom.Text) || string.IsNullOrEmpty(searchDateTo.Text)) && checkDate.IsChecked == true)
                {
                    await window.ShowMessageAsync("SEARCH", "Please complete value to search.");
                }
                else if (checkClient.IsChecked == true && searchClient.Text == null)
                {
                    await window.ShowMessageAsync("SEARCH", "Please complete value to search.");
                }
                else if (checkBank.IsChecked == true && searchBank.SelectedItem == null)
                {
                    await window.ShowMessageAsync("SEARCH", "Please complete value to search.");
                }
                else
                {
                    dgvDirectSales.ItemsSource = search();
                }
            }
        }

        private bool verifySearchFields()
        {
            bool ifAllCorrect = false;
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (checkDate.IsChecked == true)
            {
                if (string.IsNullOrEmpty(searchDateFrom.Text) && string.IsNullOrEmpty(searchDateTo.Text))
                {
                    window.ShowMessageAsync("Search: Date", "Please select date");
                }
            }
            else if (checkClient.IsChecked == true)
            {
                if (string.IsNullOrEmpty(searchClient.Text))
                {
                    window.ShowMessageAsync("Search: Client", "Please input client");
                }
            }
            else if (checkBank.IsChecked == true)
            {
                if (searchBank.SelectedItem == null)
                {
                    window.ShowMessageAsync("Search: Bank/Cash", "Please select bank or cash");
                }
            }
            else
            {
                ifAllCorrect = true;
            }

            return ifAllCorrect;
        }

        private void checkDate_Checked(object sender, RoutedEventArgs e)
        {
            searchDateFrom.IsEnabled = true;
            searchDateTo.IsEnabled = true;
        }

        private void checkDate_Unchecked(object sender, RoutedEventArgs e)
        {
            searchDateFrom.IsEnabled = false;
            searchDateTo.IsEnabled = false;
        }

        private void checkClient_Checked(object sender, RoutedEventArgs e)
        {
            searchClient.IsEnabled = true;
        }

        private void checkClient_Unchecked(object sender, RoutedEventArgs e)
        {
            searchClient.IsEnabled = false;
        }

        private void checkBank_Checked(object sender, RoutedEventArgs e)
        {
            searchBank.IsEnabled = true;
        }

        private void checkBank_Unchecked(object sender, RoutedEventArgs e)
        {
            searchBank.IsEnabled = false;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            FHBoxes_Report boxesReport = new FHBoxes_Report();
            boxesReport.ShowDialog();
            //POReport rep = new POReport();
            //rep.ShowDialog();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            checkDate.IsChecked = false;
            checkClient.IsChecked = false;
            checkBank.IsChecked = false;
            searchDateFrom.Text = "";
            searchDateTo.Text = "";
            searchClient.Text = "";
            searchBank.SelectedItem = null;
            dgvDirectSales.ItemsSource = loadDataGridDetailsDirectSales();
        }

        private void txtQuantity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void CheckIsNumeric(TextCompositionEventArgs e)
        {
            int result;

            if (!(int.TryParse(e.Text, out result) || e.Text == "."))
            {
                e.Handled = true;
            }
        }

        private void txtExpenses_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if(!UserModel.UserType.Equals(UserTypeEnum.DIRECTSALES_ADMIN) || !UserModel.UserType.Equals(UserTypeEnum.ADMIN))
            {
                btnEditDirectSales.Visibility = Visibility.Hidden;
                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Hidden;
                btnCancel.Visibility = Visibility.Hidden;
            }
        }

        private void btnProducts_Click(object sender, RoutedEventArgs e)
        {
            AddProducts addProd = new AddProducts(this, lstProductModel);
            addProd.ShowDialog();
        }

    }
}
