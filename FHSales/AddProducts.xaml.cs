using FHSales.Classes;
using FHSales.views;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Windows;

namespace FHSales
{
    /// <summary>
    /// Interaction logic for AddProducts.xaml
    /// </summary>
    public partial class AddProducts : MetroWindow
    {
        public AddProducts()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        List<ProductModel> lstProductModel = new List<ProductModel>();
        FHBoxes FhBoxesViews;

        public AddProducts(FHBoxes fhBoxes)
        {
            FhBoxesViews = fhBoxes;
            InitializeComponent();
        }

        public AddProducts(FHBoxes fhBoxes, List<ProductModel> lpm)
        {
            FhBoxesViews = fhBoxes;
            lstProductModel = lpm;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            loadProductsOnCombo();
            if(lstProductModel.Count > 0)
            {
                dgvProducts.ItemsSource = lstProductModel;
                dgvProducts.Items.Refresh();

                //btnAdd.IsEnabled = false;
                //btnRemove.IsEnabled = false;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ProductModel prodAdd = cmbProducts.SelectedItem as ProductModel;
            
            if (string.IsNullOrEmpty(txtQuantity.Text))
            {
                prodAdd.Quantity = "0";
            }else
            {
                prodAdd.Quantity = txtQuantity.Text;
            }

            lstProductModel.Add(prodAdd);
            dgvProducts.ItemsSource = lstProductModel;
            dgvProducts.Items.Refresh();

        }

        private void loadProductsOnCombo()
        {
            conDB = new ConnectionDB();
            ProductModel prod = new ProductModel();

            string queryString = "SELECT ID, productname, description FROM dbfh.tblproducts WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);
            cmbProducts.Items.Clear();
            while (reader.Read())
            {
                prod.ID = reader["ID"].ToString();
                prod.ProductName = reader["productname"].ToString();
                prod.Description = reader["description"].ToString();

                cmbProducts.Items.Add(prod);

                prod = new ProductModel();
            }

            conDB.closeConnection();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            ProductModel prodMod = dgvProducts.SelectedItem as ProductModel;

            if(prodMod != null)
            {
                lstProductModel.Remove(prodMod);
                dgvProducts.ItemsSource = lstProductModel;
                dgvProducts.Items.Refresh();
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(lstProductModel != null)
            {
                FhBoxesViews.lstProductModel = lstProductModel;
            }
            
        }
    }
}
