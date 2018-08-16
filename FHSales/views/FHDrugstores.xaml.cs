using FHSales.Classes;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FHSales.views
{
    /// <summary>
    /// Interaction logic for FHDrugstores.xaml
    /// </summary>
    public partial class FHDrugstores : UserControl
    {
        public FHDrugstores()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        DrugstoresSalesModel drugstoresSalesMod;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            searchDateFrom.IsEnabled = false;
            searchDateTo.IsEnabled = false;
            searchProduct.IsEnabled = false;
            searchDrugstore.IsEnabled = false;

            btnUpdate.Visibility = Visibility.Hidden;

            dgvSales.ItemsSource = loadDataGridDetails();
            loadDrugstoreOnCombo();
            loadProductOnCombo();

            if ((Convert.ToInt32(UserModel.UserType) == (int) UserTypeEnum.DIRECTSALES_ADMIN) || 
                (Convert.ToInt32(UserModel.UserType) == (int) UserTypeEnum.ADMIN))
            {
                btnEdit.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Visible;
                btnUpdate.Visibility = Visibility.Visible;
                btnCancel.Visibility = Visibility.Visible;
            }else
            {
                btnEdit.Visibility = Visibility.Hidden;
                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Hidden;
                btnCancel.Visibility = Visibility.Hidden;
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            drugstoresSalesMod = dgvSales.SelectedItem as DrugstoresSalesModel;

            if (drugstoresSalesMod != null)
            {
                btnUpdate.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Hidden;

                deliveryDate.Text = drugstoresSalesMod.DeliveryDate;
                txtQuantity.Text = drugstoresSalesMod.Quantity.ToString();
                foreach (DrugstoreModel dsm in comboDrugstore.Items)
                {
                    if (dsm.ID.Equals(drugstoresSalesMod.DrugstoreID))
                    {
                        comboDrugstore.SelectedItem = dsm;
                    }
                }

                foreach (ProductModel prM in comboProduct.Items)
                {
                    if (prM.ID.Equals(drugstoresSalesMod.ProductID))
                    {
                        comboProduct.SelectedItem = prM;
                    }
                }
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool bl = await checkFields();
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (bl)
            {
                saveRecord();
                clearFields();
                await window.ShowMessageAsync("SAVE RECORD", "Record saved successfully!");
                dgvSales.ItemsSource = loadDataGridDetails();
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool bl = await checkFields();
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (bl)
            {
                updateRecord(drugstoresSalesMod);
                clearFields();
                await window.ShowMessageAsync("UPDATE RECORD", "Record updated successfully!");
                dgvSales.ItemsSource = loadDataGridDetails();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            clearFields();
            btnUpdate.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Drugstores_Report drugReport = new Drugstores_Report();
            drugReport.ShowDialog();
        }

        private void clearFields()
        {
            deliveryDate.Text = "";
            txtQuantity.Text = "";
            comboDrugstore.SelectedItem = null;
            comboProduct.SelectedItem = null;
        }

        private async Task<bool> checkFields()
        {
            bool ifAllCorrect = false;
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (string.IsNullOrEmpty(deliveryDate.Text))
            {
                await window.ShowMessageAsync("DELIVERY DATE", "Please select date.");
            }
            else if (string.IsNullOrEmpty(txtQuantity.Text))
            {
                await window.ShowMessageAsync("QUANTITY", "Please provide quantity.");
            }
            else if (comboDrugstore.SelectedItem == null)
            {
                await window.ShowMessageAsync("DRUG STORE", "Please select drug store.");
            }
            else
            {
                ifAllCorrect = true;
            }

            return ifAllCorrect;
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

        private List<DrugstoresSalesModel> loadDataGridDetails()
        {
            conDB = new ConnectionDB();

            List<DrugstoresSalesModel> lstSales = new List<DrugstoresSalesModel>();
            DrugstoresSalesModel sales = new DrugstoresSalesModel();

            string queryString = "SELECT dbfh.tblsales.ID, dbfh.tblsales.deliverydate, dbfh.tblsales.quantity, dbfh.tblsales.drugstoreID, "
                + "dbfh.tbldrugstores.drugstore, dbfh.tblproducts.description AS product, dbfh.tblsales.productID as prodID "
                + "FROM((dbfh.tblsales INNER JOIN dbfh.tbldrugstores ON dbfh.tblsales.drugstoreID = dbfh.tbldrugstores.ID) " +
                "INNER JOIN dbfh.tblproducts ON dbfh.tblsales.productID = dbfh.tblproducts.ID) " +
                "WHERE dbfh.tblsales.isDeleted = 0 ORDER BY dbfh.tblsales.deliverydate DESC";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                sales.ID = reader["ID"].ToString();
                DateTime dte = DateTime.Parse(reader["deliveryDate"].ToString());
                sales.DeliveryDate = dte.ToShortDateString();
                sales.Quantity = Convert.ToInt32(reader["quantity"].ToString());
                sales.DrugstoreID = reader["drugstoreID"].ToString();
                sales.DrugstoreName = reader["drugstore"].ToString();
                sales.ProductName = reader["product"].ToString();
                sales.ProductID = reader["prodID"].ToString();

                lstSales.Add(sales);
                sales = new DrugstoresSalesModel();
            }

            conDB.closeConnection();

            return lstSales;
        }

        private void loadDrugstoreOnCombo()
        {
            conDB = new ConnectionDB();

            string queryString = "SELECT ID, drugstore, description FROM dbfh.tbldrugstores WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);
            comboDrugstore.Items.Clear();
            searchDrugstore.Items.Clear();
            while (reader.Read())
            {
                DrugstoreModel drg = new DrugstoreModel();
                drg.ID = reader["ID"].ToString();
                drg.DrugstoreName = reader["drugstore"].ToString();
                drg.Description = reader["description"].ToString();

                comboDrugstore.Items.Add(drg);
                searchDrugstore.Items.Add(drg);
            }
            conDB.closeConnection();
        }

        private void updateRecord(DrugstoresSalesModel dsm)
        {
            conDB = new ConnectionDB();

            string queryString = "UPDATE dbfh.tblsales SET deliverydate = ?, quantity = ?, drugstoreID = ?, productID = ? WHERE ID = ?";

            List<string> parameters = new List<string>();

            DateTime date = DateTime.Parse(deliveryDate.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
            parameters.Add(txtQuantity.Text);
            parameters.Add(comboDrugstore.SelectedValue.ToString());
            parameters.Add(comboProduct.SelectedValue.ToString());
            parameters.Add(dsm.ID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
        }

        private void saveRecord()
        {
            conDB = new ConnectionDB();
            string queryString = "INSERT INTO dbfh.tblsales (deliverydate, quantity, drugstoreID, productID, isDeleted) VALUES " +
                "(?,?,?,?,0)";
            List<string> parameters = new List<string>();
            DateTime date = DateTime.Parse(deliveryDate.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
            parameters.Add(txtQuantity.Text);
            parameters.Add(comboDrugstore.SelectedValue.ToString());
            parameters.Add(comboProduct.SelectedValue.ToString());

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();

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
            searchDateFrom.Text = "";
            searchDateTo.Text = "";
        }

        private void checkCategory_Unchecked(object sender, RoutedEventArgs e)
        {
            searchProduct.IsEnabled = false;
            searchProduct.SelectedItem = null;

        }

        private void checkCategory_Checked(object sender, RoutedEventArgs e)
        {
            searchProduct.IsEnabled = true;
            searchProduct.SelectedItem = null;
        }

        private void checkDrugstore_Checked(object sender, RoutedEventArgs e)
        {
            searchDrugstore.IsEnabled = true;
            searchDrugstore.SelectedItem = null;
        }

        private void checkDrugstore_Unchecked(object sender, RoutedEventArgs e)
        {
            searchDrugstore.IsEnabled = false;
            searchDrugstore.SelectedItem = null;
        }

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {

            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;


            if (checkDate.IsChecked == true || checkCategory.IsChecked == true || checkDrugstore.IsChecked == true)
            {
                if ((string.IsNullOrEmpty(searchDateFrom.Text) || string.IsNullOrEmpty(searchDateTo.Text)) && checkDate.IsChecked == true)
                {
                    await window.ShowMessageAsync("SEARCH", "Please complete value to search.");
                }
                else if (checkCategory.IsChecked == true && searchProduct.SelectedItem == null)
                {
                    await window.ShowMessageAsync("SEARCH", "Please complete value to search.");
                }
                else if (checkDrugstore.IsChecked == true && searchDrugstore.SelectedItem == null)
                {
                    await window.ShowMessageAsync("SEARCH", "Please complete value to search.");
                }
                else
                {
                    dgvSales.ItemsSource = search();
                }
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            searchDateFrom.Text = "";
            searchDateTo.Text = "";
            searchProduct.SelectedItem = null;
            searchDrugstore.SelectedItem = null;
        }

        private List<DrugstoresSalesModel> search()
        {
            List<DrugstoresSalesModel> lstDrugstores = new List<DrugstoresSalesModel>();
            DrugstoresSalesModel sales = new DrugstoresSalesModel();
            conDB = new ConnectionDB();

            string queryString = "SELECT dbfh.tblsales.ID, dbfh.tblsales.deliverydate, dbfh.tblsales.quantity, dbfh.tblsales.drugstoreID, "
               + "dbfh.tbldrugstores.drugstore, dbfh.tblproducts.description AS product, dbfh.tblsales.productID as prodID "
               + "FROM((dbfh.tblsales INNER JOIN dbfh.tbldrugstores ON dbfh.tblsales.drugstoreID = dbfh.tbldrugstores.ID) " +
               "INNER JOIN dbfh.tblproducts ON dbfh.tblsales.productID = dbfh.tblproducts.ID) " +
               "WHERE dbfh.tblsales.isDeleted = 0";

            List<string> parameters = new List<string>();

            if (checkDate.IsChecked == true)
            {
                queryString += " AND (deliverydate BETWEEN ? AND ?)";
                DateTime sdate = DateTime.Parse(searchDateFrom.Text);
                parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);
                sdate = DateTime.Parse(searchDateTo.Text);
                parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);
            }

            if (checkCategory.IsChecked == true)
            {
                queryString += " AND (dbfh.tblsales.productID = ?)";
                parameters.Add(searchProduct.SelectedValue.ToString());
            }

            if (checkDrugstore.IsChecked == true)
            {
                queryString += " AND (dbfh.tblsales.drugstoreID = ?)";
                parameters.Add(searchDrugstore.SelectedValue.ToString());

            }

            queryString += " ORDER BY dbfh.tblsales.deliverydate DESC";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                sales.ID = reader["ID"].ToString();
                DateTime dte = DateTime.Parse(reader["deliveryDate"].ToString());
                sales.DeliveryDate = dte.ToShortDateString();
                sales.Quantity = Convert.ToInt32(reader["quantity"].ToString());
                sales.DrugstoreID = reader["drugstoreID"].ToString();
                sales.DrugstoreName = reader["drugstore"].ToString();
                sales.ProductName = reader["product"].ToString();
                sales.ProductID = reader["prodID"].ToString();

                lstDrugstores.Add(sales);
                sales = new DrugstoresSalesModel();
            }
            conDB.closeConnection();
            return lstDrugstores;
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

    }
}
