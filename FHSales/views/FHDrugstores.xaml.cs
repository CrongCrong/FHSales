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
        MahApps.Metro.Controls.MetroWindow window;
        DrugstoresSales drugstoreSaleToUpdate;
        public List<ProductsOrdered> lstProductsOrdered = new List<ProductsOrdered>();

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!UserModel.isPOAdmin)
            {
                btnAdd.Visibility = Visibility.Hidden;
                btnCancel.Visibility = Visibility.Hidden;
                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Hidden;
                btnSearch.Visibility = Visibility.Hidden;
                btnEdit.Visibility = Visibility.Hidden;
                btnReset.Visibility = Visibility.Hidden;
                dgvSales.IsEnabled = false;
            }

            window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
            searchDateFrom.IsEnabled = false;
            searchDateTo.IsEnabled = false;
            searchProduct.IsEnabled = false;

            btnUpdate.Visibility = Visibility.Hidden;

            dgvSales.ItemsSource = await loadDataGridDetails();
            //loadDrugstoresOnCombo();
            loadAreaOnCombo();
            loadAgentOnCombo();
        }

        private async Task<List<DrugstoresSales>> loadDataGridDetails()
        {
            List<DrugstoresSales> lstDrgs = new List<DrugstoresSales>();
            conDB = new ConnectionDB();
            try
            {
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                var collection = db.GetCollection<DrugstoresSales>("DrugstoresSales");
                var filter = Builders<DrugstoresSales>.Filter.And(
        Builders<DrugstoresSales>.Filter.Where(p => p.isDeleted == false));
                lstDrgs = collection.Find(filter).ToList();

                foreach (DrugstoresSales ds in lstDrgs)
                {
                    ds.strDate = ds.DeliveryDate.ToShortDateString();
                }
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Caused by: " + ex.StackTrace);
            }
            return lstDrgs;
        }

        //private async void loadDrugstoresOnCombo()
        //{
        //    try
        //    {
        //        searchDrugstore.Items.Clear();

        //        conDB = new ConnectionDB();
        //        MongoClient client = conDB.initializeMongoDB();
        //        var db = client.GetDatabase("DBFH");
        //        var collection = db.GetCollection<Drugstores>("Drugstores");
        //        var filter = Builders<Drugstores>.Filter.And(
        //Builders<Drugstores>.Filter.Where(p => p.isDeleted == false));
        //        List<Drugstores> lstPayments = collection.Find(filter).ToList();
        //        foreach (Drugstores p in lstPayments)
        //        {
        //            searchDrugstore.Items.Add(p);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await window.ShowMessageAsync("EXCEPTION", "ERROR LOADING DATA " + ex.Message);
        //    }
        //}

        private async void loadAreaOnCombo()
        {
            try
            {
                cmbArea.Items.Clear();
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                var collection = db.GetCollection<Area>("Area");
                var filter = Builders<Area>.Filter.And(
        Builders<Area>.Filter.Where(p => p.isDeleted == false));
                List<Area> lstPayments = collection.Find(filter).ToList();
                foreach (Area p in lstPayments)
                {
                    cmbArea.Items.Add(p);
                }
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("EXCEPTION", "ERROR LOADING DATA " + ex.Message);
            }
        }

        private async void loadAgentOnCombo()
        {
            try
            {
                cmbAgent.Items.Clear();
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();

                var db = client.GetDatabase("DBFH");

                var collection = db.GetCollection<Agents>("Agents");

                var filter = Builders<Agents>.Filter.And(
        Builders<Agents>.Filter.Where(p => p.isDeleted == false));
                List<Agents> lstPayments = collection.Find(filter).ToList();

                foreach (Agents p in lstPayments)
                {
                    cmbAgent.Items.Add(p);
                }
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("EXCEPTION", "ERROR LOADING DATA " + ex.Message);
            }
        }

        private async void saveRecord()
        {
            Agents slctdAgent = cmbAgent.SelectedItem as Agents;
            Area slctdArea = cmbArea.SelectedItem as Area;

            try
            {
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                DrugstoresSales ds = new DrugstoresSales();
                DateTime dte = DateTime.Parse(deliveryDate.Text);
                ds.DeliveryDate = DateTime.Parse(dte.ToLocalTime().ToShortDateString());
                ds.Drugstorename = txtSmallDrugstore.Text;
                ds.Agent = slctdAgent;
                ds.Areas = slctdArea;
                ds.SubArea = cmbSubarea.SelectedValue.ToString();
                ds.Total = Convert.ToDouble( txtTotal.Text);
                ds.ProductsOrdered = lstProductsOrdered;

                var collection = db.GetCollection<DrugstoresSales>("DrugstoresSales");
                collection.InsertOne(ds);
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Cause by: " + ex.StackTrace);
            }
        }

        private async void updateRecord()
        {
            try
            {
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");

                DateTime dte = DateTime.Parse(deliveryDate.Text);
                drugstoreSaleToUpdate.DeliveryDate = DateTime.Parse(dte.ToLocalTime().ToShortDateString());
                drugstoreSaleToUpdate.Drugstorename = txtSmallDrugstore.Text;

                var filter = Builders<DrugstoresSales>.Filter.And(
            Builders<DrugstoresSales>.Filter.Where(p => p.Id == drugstoreSaleToUpdate.Id));
                var updte = Builders<DrugstoresSales>.Update.Set("DeliveryDate", drugstoreSaleToUpdate.DeliveryDate)
                    .Set("Drugstorename", drugstoreSaleToUpdate.Drugstorename)
                    .Set("Agent", drugstoreSaleToUpdate.Agent)
                    .Set("Areas", drugstoreSaleToUpdate.Areas)
                    .Set("SubArea", drugstoreSaleToUpdate.SubArea)
                    .Set("Total", drugstoreSaleToUpdate.Total)
                    .Set("ProductsOrdered", drugstoreSaleToUpdate.ProductsOrdered);

                var collection = db.GetCollection<DrugstoresSales>("DrugstoresSales");
                collection.UpdateOne(filter, updte);
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Cause by: " + ex.StackTrace);
            }
        }

        private async Task<List<DrugstoresSales>> search()
        {
            List<DrugstoresSales> lstDrgSales = new List<DrugstoresSales>();
            DateTime dteNow = DateTime.Parse(searchDateFrom.Text);
            DateTime dteFirstDay = DateTime.Parse(searchDateTo.Text);

            try
            {
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                var collection = db.GetCollection<DrugstoresSales>("DrugstoresSales");

                var filter = Builders<DrugstoresSales>.Filter.And(
        Builders<DrugstoresSales>.Filter.Where(p => p.isDeleted == false));

                if (checkDate.IsChecked.Value)
                {
                    filter = Builders<DrugstoresSales>.Filter.And(
          Builders<DrugstoresSales>.Filter.Gte("DeliveryDate", dteNow),
         Builders<DrugstoresSales>.Filter.Lte("DeliveryDate", dteFirstDay));
                }

                if (checkCategory.IsChecked.Value)
                {
                    Products dd = searchProduct.SelectedItem as Products;
                    filter = Builders<DrugstoresSales>.Filter.And(
         Builders<DrugstoresSales>.Filter.Eq("Products", dd));
                }

                lstDrgSales = collection.Find(filter).ToList();


            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Cause by: " + ex.StackTrace);
            }

            return lstDrgSales;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            drugstoreSaleToUpdate = dgvSales.SelectedItem as DrugstoresSales;

            if (drugstoreSaleToUpdate != null)
            {
                dgvSales.IsEnabled = false;
                btnUpdate.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Hidden;

                deliveryDate.Text = drugstoreSaleToUpdate.DeliveryDate.ToShortDateString();
                txtSmallDrugstore.Text = drugstoreSaleToUpdate.Drugstorename;
                txtTotal.Text = drugstoreSaleToUpdate.Total.ToString();
                lstProductsOrdered = drugstoreSaleToUpdate.ProductsOrdered;

                foreach(Area aa in cmbArea.Items)
                {
                    if (aa.Id.Equals(drugstoreSaleToUpdate.Areas.Id))
                    {
                        cmbArea.SelectedItem = aa;
                    }
                }

                foreach (SubArea sa in cmbSubarea.Items)
                {
                    if (sa.SubAreaName.Equals(drugstoreSaleToUpdate.SubArea))
                    {
                        cmbSubarea.SelectedItem = sa;
                    }
                }

                foreach(Agents ag in cmbAgent.Items)
                {
                    if (ag.Id.Equals(drugstoreSaleToUpdate.Agent.Id))
                    {
                        cmbAgent.SelectedItem = ag;
                    }
                }
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool bl = await checkFields();

            if (bl)
            {
                saveRecord();
                clearFields();
                await window.ShowMessageAsync("SAVE RECORD", "Record saved successfully!");
                dgvSales.ItemsSource = await loadDataGridDetails();
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool bl = await checkFields();
            if (bl)
            {
                updateRecord();
                clearFields();
                await window.ShowMessageAsync("UPDATE RECORD", "Record updated successfully!");
                dgvSales.ItemsSource = await loadDataGridDetails();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            clearFields();
            btnUpdate.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;
            dgvSales.IsEnabled = true;
            lstProductsOrdered = new List<ProductsOrdered>();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Drugstores_Report drugReport = new Drugstores_Report();
            drugReport.ShowDialog();
        }

        private void clearFields()
        {
            deliveryDate.Text = "";
            txtSmallDrugstore.Text = "";
            cmbAgent.SelectedItem = null;
            cmbArea.SelectedItem = null;
            cmbSubarea.SelectedItem = null;
            txtTotal.Text = "";
        }

        private async Task<bool> checkFields()
        {
            bool ifAllCorrect = false;
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (string.IsNullOrEmpty(deliveryDate.Text))
            {
                await window.ShowMessageAsync("DELIVERY DATE", "Please select date.");
            }
            else if (string.IsNullOrEmpty(txtSmallDrugstore.Text))
            {
                await window.ShowMessageAsync("DRUG STORE", "Please select drug store.");
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

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {

            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;


            if (checkDate.IsChecked == true || checkCategory.IsChecked == true)
            {
                if ((string.IsNullOrEmpty(searchDateFrom.Text) || string.IsNullOrEmpty(searchDateTo.Text)) && checkDate.IsChecked == true)
                {
                    await window.ShowMessageAsync("SEARCH", "Please complete value to search.");
                }
                else if (checkCategory.IsChecked == true && searchProduct.SelectedItem == null)
                {
                    await window.ShowMessageAsync("SEARCH", "Please complete value to search.");
                }
                else
                {
                    dgvSales.ItemsSource = await search();
                }
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            searchDateFrom.Text = "";
            searchDateTo.Text = "";
            searchProduct.SelectedItem = null;
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

        #region MYSQL CODES
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
        //private List<DrugstoresSalesModel> loadDataGridDetails()
        //{
        //    conDB = new ConnectionDB();

        //    List<DrugstoresSalesModel> lstSales = new List<DrugstoresSalesModel>();
        //    DrugstoresSalesModel sales = new DrugstoresSalesModel();

        //    string queryString = "SELECT dbfh.tblsales.ID, dbfh.tblsales.deliverydate, dbfh.tblsales.quantity, dbfh.tblsales.drugstoreID, "
        //        + "dbfh.tbldrugstores.drugstore, dbfh.tblproducts.description AS product, dbfh.tblsales.productID as prodID "
        //        + "FROM((dbfh.tblsales INNER JOIN dbfh.tbldrugstores ON dbfh.tblsales.drugstoreID = dbfh.tbldrugstores.ID) " +
        //        "INNER JOIN dbfh.tblproducts ON dbfh.tblsales.productID = dbfh.tblproducts.ID) " +
        //        "WHERE dbfh.tblsales.isDeleted = 0 ORDER BY dbfh.tblsales.deliverydate DESC";

        //    MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

        //    while (reader.Read())
        //    {
        //        sales.ID = reader["ID"].ToString();
        //        DateTime dte = DateTime.Parse(reader["deliveryDate"].ToString());
        //        sales.DeliveryDate = dte.ToShortDateString();
        //        sales.Quantity = Convert.ToInt32(reader["quantity"].ToString());
        //        sales.DrugstoreID = reader["drugstoreID"].ToString();
        //        sales.DrugstoreName = reader["drugstore"].ToString();
        //        sales.ProductName = reader["product"].ToString();
        //        sales.ProductID = reader["prodID"].ToString();

        //        lstSales.Add(sales);
        //        sales = new DrugstoresSalesModel();
        //    }

        //    conDB.closeConnection();

        //    return lstSales;
        //}

        //private void loadDrugstoreOnCombo()
        //{
        //    conDB = new ConnectionDB();

        //    string queryString = "SELECT ID, drugstore, description FROM dbfh.tbldrugstores WHERE isDeleted = 0";

        //    MySqlDataReader reader = conDB.getSelectConnection(queryString, null);
        //    comboDrugstore.Items.Clear();
        //    searchDrugstore.Items.Clear();
        //    while (reader.Read())
        //    {
        //        DrugstoreModel drg = new DrugstoreModel();
        //        drg.ID = reader["ID"].ToString();
        //        drg.DrugstoreName = reader["drugstore"].ToString();
        //        drg.Description = reader["description"].ToString();

        //        comboDrugstore.Items.Add(drg);
        //        searchDrugstore.Items.Add(drg);
        //    }
        //    conDB.closeConnection();
        //}

        //private void updateRecord(DrugstoresSalesModel dsm)
        //{
        //    conDB = new ConnectionDB();

        //    string queryString = "UPDATE dbfh.tblsales SET deliverydate = ?, quantity = ?, drugstoreID = ?, productID = ? WHERE ID = ?";

        //    List<string> parameters = new List<string>();

        //    DateTime date = DateTime.Parse(deliveryDate.Text);
        //    parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
        //    parameters.Add(txtQuantity.Text);
        //    parameters.Add(comboDrugstore.SelectedValue.ToString());
        //    parameters.Add(comboProduct.SelectedValue.ToString());
        //    parameters.Add(dsm.ID);

        //    conDB.AddRecordToDatabase(queryString, parameters);
        //    conDB.closeConnection();
        //}

        //private void saveRecord()
        //{
        //    conDB = new ConnectionDB();
        //    string queryString = "INSERT INTO dbfh.tblsales (deliverydate, quantity, drugstoreID, productID, isDeleted) VALUES " +
        //        "(?,?,?,?,0)";
        //    List<string> parameters = new List<string>();
        //    DateTime date = DateTime.Parse(deliveryDate.Text);
        //    parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
        //    parameters.Add(txtQuantity.Text);
        //    parameters.Add(comboDrugstore.SelectedValue.ToString());
        //    parameters.Add(comboProduct.SelectedValue.ToString());

        //    conDB.AddRecordToDatabase(queryString, parameters);
        //    conDB.closeConnection();

        //}

        //private List<DrugstoresSalesModel> search()
        //{
        //    List<DrugstoresSalesModel> lstDrugstores = new List<DrugstoresSalesModel>();
        //    DrugstoresSalesModel sales = new DrugstoresSalesModel();
        //    conDB = new ConnectionDB();

        //    string queryString = "SELECT dbfh.tblsales.ID, dbfh.tblsales.deliverydate, dbfh.tblsales.quantity, dbfh.tblsales.drugstoreID, "
        //       + "dbfh.tbldrugstores.drugstore, dbfh.tblproducts.description AS product, dbfh.tblsales.productID as prodID "
        //       + "FROM((dbfh.tblsales INNER JOIN dbfh.tbldrugstores ON dbfh.tblsales.drugstoreID = dbfh.tbldrugstores.ID) " +
        //       "INNER JOIN dbfh.tblproducts ON dbfh.tblsales.productID = dbfh.tblproducts.ID) " +
        //       "WHERE dbfh.tblsales.isDeleted = 0";

        //    List<string> parameters = new List<string>();

        //    if (checkDate.IsChecked == true)
        //    {
        //        queryString += " AND (deliverydate BETWEEN ? AND ?)";
        //        DateTime sdate = DateTime.Parse(searchDateFrom.Text);
        //        parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);
        //        sdate = DateTime.Parse(searchDateTo.Text);
        //        parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);
        //    }

        //    if (checkCategory.IsChecked == true)
        //    {
        //        queryString += " AND (dbfh.tblsales.productID = ?)";
        //        parameters.Add(searchProduct.SelectedValue.ToString());
        //    }

        //    if (checkDrugstore.IsChecked == true)
        //    {
        //        queryString += " AND (dbfh.tblsales.drugstoreID = ?)";
        //        parameters.Add(searchDrugstore.SelectedValue.ToString());

        //    }

        //    queryString += " ORDER BY dbfh.tblsales.deliverydate DESC";

        //    MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

        //    while (reader.Read())
        //    {
        //        sales.ID = reader["ID"].ToString();
        //        DateTime dte = DateTime.Parse(reader["deliveryDate"].ToString());
        //        sales.DeliveryDate = dte.ToShortDateString();
        //        sales.Quantity = Convert.ToInt32(reader["quantity"].ToString());
        //        sales.DrugstoreID = reader["drugstoreID"].ToString();
        //        sales.DrugstoreName = reader["drugstore"].ToString();
        //        sales.ProductName = reader["product"].ToString();
        //        sales.ProductID = reader["prodID"].ToString();

        //        lstDrugstores.Add(sales);
        //        sales = new DrugstoresSalesModel();
        //    }
        //    conDB.closeConnection();
        //    return lstDrugstores;
        //}

        #endregion

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddProducts addProducts = new FHSales.AddProducts(this, lstProductsOrdered);
            addProducts.ShowDialog();
        }

        private void cmbArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Area aa = cmbArea.SelectedItem as Area;
            cmbSubarea.Items.Clear();
            if (aa != null)
            {
                foreach(SubArea sa in aa.SubArea)
                {
                    cmbSubarea.Items.Add(sa);
                }
            }
        }
    }
}
