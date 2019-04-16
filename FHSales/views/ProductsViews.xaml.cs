using FHSales.Classes;
using FHSales.MongoClasses;
using MahApps.Metro.Controls.Dialogs;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FHSales.views
{
    /// <summary>
    /// Interaction logic for Products.xaml
    /// </summary>
    public partial class ProductsViews : UserControl
    {
        public ProductsViews()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        MahApps.Metro.Controls.MetroWindow window;
        Products productToUpdate = new Products();

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
            dgvProducts.ItemsSource = await loadDataGridDetails();
            btnUpdate.Visibility = Visibility.Hidden;
            //if ((Convert.ToInt32(UserModel.UserType) == (int)UserTypeEnum.DIRECTSALES_ADMIN) ||
            //    (Convert.ToInt32(UserModel.UserType) == (int)UserTypeEnum.ADMIN))
            //{
            //    btnEdit.Visibility = Visibility.Visible;
            //    btnSave.Visibility = Visibility.Visible;
            //    btnUpdate.Visibility = Visibility.Visible;
            //    btnCancel.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    btnEdit.Visibility = Visibility.Hidden;
            //    btnSave.Visibility = Visibility.Hidden;
            //    btnUpdate.Visibility = Visibility.Hidden;
            //    btnCancel.Visibility = Visibility.Hidden;
            //}
        }

        private async Task<List<Products>> loadDataGridDetails()
        {
            List<Products> lstPrds = new List<Products>();
            conDB = new ConnectionDB();
            try
            {
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                var collection = db.GetCollection<Products>("Products");
                var filter = Builders<Products>.Filter.And(
        Builders<Products>.Filter.Where(p => p.isDeleted == false));
                lstPrds = collection.Find(filter).ToList();
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Cause by: " + ex.StackTrace);
            }
            return lstPrds;
        }


        private async void saveRecord()
        {
            try
            {
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                Products p = new Products();
                p.ProductName = txtProductName.Text;
                p.Description = txtDescription.Text;
                p.Price = Convert.ToDouble(txtPrice.Text);
                var collection = db.GetCollection<Products>("Products");
                collection.InsertOne(p);
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

                productToUpdate.ProductName = txtProductName.Text;
                productToUpdate.Description = txtDescription.Text;
                productToUpdate.Price = Convert.ToDouble(txtPrice.Text);

                var filter = Builders<Products>.Filter.And(
            Builders<Products>.Filter.Where(p => p.Id == productToUpdate.Id));
                var updte = Builders<Products>.Update.Set("ProductName", productToUpdate.ProductName)
                    .Set("Description", productToUpdate.Description)
                    .Set("Price", productToUpdate.Price);

                var collection = db.GetCollection<Products>("Products");
                collection.UpdateOne(filter, updte);
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Cause by: " + ex.StackTrace);
            }
        }

        private void btnEditDirectSales_Click(object sender, RoutedEventArgs e)
        {
            productToUpdate = dgvProducts.SelectedItem as Products;
            if (productToUpdate != null)
            {

                txtProductName.Text = productToUpdate.ProductName;
                txtDescription.Text = productToUpdate.Description;
                txtPrice.Text = productToUpdate.Price.ToString();
                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Visible;
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            updateRecord();
            await window.ShowMessageAsync("UPDATE RECORD", "Record updated successfully!");
            dgvProducts.ItemsSource = await loadDataGridDetails();
            txtProductName.Text = "";
            txtDescription.Text = "";
            txtPrice.Text = "";
            btnUpdate.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool bl = await checkFields();
            if (bl)
            {
                saveRecord();
                txtProductName.Text = "";
                txtDescription.Text = "";
                txtPrice.Text = "";
                dgvProducts.ItemsSource = await loadDataGridDetails();
                await window.ShowMessageAsync("SAVE RECORD", "Record saved successfully!");
            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            txtProductName.Text = "";
            txtDescription.Text = "";
            txtPrice.Text = "";
            btnUpdate.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;
        }

        private async Task<bool> checkFields()
        {
            bool ifAllCorrect = false;
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (string.IsNullOrEmpty(txtProductName.Text))
            {
                await window.ShowMessageAsync("PRODUCT NAME", "Please provide product name.");
            }
            else if (string.IsNullOrEmpty(txtDescription.Text))
            {
                await window.ShowMessageAsync("DESCRIPTION", "Please provide product description");
            }
            else if (string.IsNullOrEmpty(txtPrice.Text))
            {
                await window.ShowMessageAsync("DESCRIPTION", "Please provide product description");
            }
            else
            {
                ifAllCorrect = true;
            }

            return ifAllCorrect;
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
        //private List<ProductModel> loadProductsOnDataGrid()
        //{
        //    conDB = new ConnectionDB();
        //    ProductModel product = new ProductModel();
        //    List<ProductModel> lstProducts = new List<ProductModel>();

        //    string queryString = "SELECT ID, productname, description FROM dbfh.tblproducts WHERE isDeleted = 0";

        //    MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

        //    while (reader.Read())
        //    {
        //        product.ID = reader["ID"].ToString();
        //        product.ProductName = reader["productname"].ToString();
        //        product.Description = reader["description"].ToString();
        //        lstProducts.Add(product);
        //        product = new ProductModel();
        //    }

        //    conDB.closeConnection();

        //    return lstProducts;
        //}

        //private void saveRecord()
        //{
        //    conDB = new ConnectionDB();

        //    string queryString = "INSERT INTO dbfh.tblproducts (productname, description, isDeleted) VALUES (?,?,0)";
        //    List<string> parameters = new List<string>();
        //    parameters.Add(txtProductName.Text);
        //    parameters.Add(txtDescription.Text);

        //    conDB.AddRecordToDatabase(queryString, parameters);

        //    conDB.closeConnection();
        //}

        //private void updateRecord(string strID)
        //{
        //    conDB = new ConnectionDB();
        //    string queryString = "UPDATE dbfh.tblproducts SET productname = ?, description = ? WHERE ID = ?";

        //    List<string> parameters = new List<string>();
        //    parameters.Add(txtProductName.Text);
        //    parameters.Add(txtDescription.Text);
        //    parameters.Add(strID);

        //    conDB.AddRecordToDatabase(queryString, parameters);

        //    conDB.closeConnection();
        //}
        #endregion

        private void txtPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }
    }
}
