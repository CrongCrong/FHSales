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
    /// Interaction logic for Products.xaml
    /// </summary>
    public partial class Products : UserControl
    {
        public Products()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        string recordID = "";

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgvProducts.ItemsSource = loadProductsOnDataGrid();
            btnUpdate.Visibility = Visibility.Hidden;
            if ((Convert.ToInt32(UserModel.UserType) == (int)UserTypeEnum.DIRECTSALES_ADMIN) ||
                (Convert.ToInt32(UserModel.UserType) == (int)UserTypeEnum.ADMIN))
            {
                btnEdit.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Visible;
                btnUpdate.Visibility = Visibility.Visible;
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

        private List<ProductModel> loadProductsOnDataGrid()
        {
            conDB = new ConnectionDB();
            ProductModel product = new ProductModel();
            List<ProductModel> lstProducts = new List<ProductModel>();

            string queryString = "SELECT ID, productname, description FROM dbfh.tblproducts WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                product.ID = reader["ID"].ToString();
                product.ProductName = reader["productname"].ToString();
                product.Description = reader["description"].ToString();
                lstProducts.Add(product);
                product = new ProductModel();
            }

            conDB.closeConnection();

            return lstProducts;
        }

        private void btnEditDirectSales_Click(object sender, RoutedEventArgs e)
        {
            ProductModel selectedProd = dgvProducts.SelectedItem as ProductModel;

            if (selectedProd != null)
            {
                recordID = selectedProd.ID;
                txtProductName.Text = selectedProd.ProductName;
                txtDescription.Text = selectedProd.Description;
                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Visible;
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            updateRecord(recordID);
            await window.ShowMessageAsync("UPDATE RECORD", "Record updated successfully!");
            dgvProducts.ItemsSource = loadProductsOnDataGrid();
            txtProductName.Text = "";
            txtDescription.Text = "";
            recordID = "";
            btnUpdate.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool bl = await checkFields();
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (bl)
            {
                saveRecord();
                txtProductName.Text = "";
                txtDescription.Text = "";
                dgvProducts.ItemsSource = loadProductsOnDataGrid();
                await window.ShowMessageAsync("SAVE RECORD", "Record saved successfully!");
            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            txtProductName.Text = "";
            txtDescription.Text = "";
            recordID = "";
            btnUpdate.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;
        }

        private void saveRecord()
        {
            conDB = new ConnectionDB();

            string queryString = "INSERT INTO dbfh.tblproducts (productname, description, isDeleted) VALUES (?,?,0)";
            List<string> parameters = new List<string>();
            parameters.Add(txtProductName.Text);
            parameters.Add(txtDescription.Text);

            conDB.AddRecordToDatabase(queryString, parameters);

            conDB.closeConnection();
        }

        private void updateRecord(string strID)
        {
            conDB = new ConnectionDB();
            string queryString = "UPDATE dbfh.tblproducts SET productname = ?, description = ? WHERE ID = ?";

            List<string> parameters = new List<string>();
            parameters.Add(txtProductName.Text);
            parameters.Add(txtDescription.Text);
            parameters.Add(strID);

            conDB.AddRecordToDatabase(queryString, parameters);

            conDB.closeConnection();
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
            else
            {
                ifAllCorrect = true;
            }

            return ifAllCorrect;
        }
    }
}
