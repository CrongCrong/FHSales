using FHSales.Classes;
using FHSales.views;
using MahApps.Metro.Controls;
using System.Collections.Generic;
using System.Windows;
using System;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using FHSales.MongoClasses;
using MongoDB.Driver;

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
        List<ProductsOrdered> lstProductForDrugstores = new List<ProductsOrdered>();
        ProductsOrdered productsOrdered = new ProductsOrdered();
        List<ProductModel> lstProdsMods = new List<ProductModel>();
        List<PaymentsDrugstores> lstPaymentDrugstores = new List<PaymentsDrugstores>();

        FHBoxes FhBoxesViews;
        FHDrugstores FhDrugstoreView;
        bool ifEdit = false;
        //string strSalesID = "";

        MahApps.Metro.Controls.MetroWindow window;


        public AddProducts(FHBoxes fhBoxes)
        {
            FhBoxesViews = fhBoxes;
            InitializeComponent();
        }

        public AddProducts(FHBoxes fhBoxes, List<ProductModel> lpm, bool ifTryToEdit)
        {
            ifEdit = ifTryToEdit;
            FhBoxesViews = fhBoxes;
            lstProdsMods = lpm;
            InitializeComponent();
        }

        public AddProducts(FHDrugstores fhDrugs, List<ProductsOrdered> lpmDrg, List<PaymentsDrugstores> lstPdrg)
        {
            FhDrugstoreView = fhDrugs;
            lstProductForDrugstores = lpmDrg;
            lstPaymentDrugstores = lstPdrg;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
            loadProductsOnCombo();
            if (FhBoxesViews != null)
            {
                dgvProducts.ItemsSource = lstProdsMods;
            }

            if (FhDrugstoreView != null)
            {
                dgvProducts.ItemsSource = lstProductForDrugstores;
            }
        }

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Products prodAdd = cmbProducts.SelectedItem as Products;
            bool x = await checkFields();

            if (x)
            {
                productsOrdered.ProductName = prodAdd.ProductName;
                productsOrdered.Qty = Convert.ToInt32(txtQuantity.Text);
                productsOrdered.Price = Convert.ToDouble(txtTotal.Text);
                productsOrdered.Total = Convert.ToDouble(txtTotal.Text);
                productsOrdered.Id = prodAdd.Id;
                lstProductForDrugstores.Add(productsOrdered);
                dgvProducts.ItemsSource = lstProductForDrugstores;
                dgvProducts.Items.Refresh();
                productsOrdered = new ProductsOrdered();
                clearFields();
            }

            //ProductModel proAdd = cmbProducts.SelectedItem as ProductModel;
            //ProductModel pp = new ProductModel();
            //bool x = await checkFields();
            //if (x)
            //{

            //    pp.ID = proAdd.ProductID;
            //    pp.Quantity = txtQuantity.Text;
            //    pp.ProductName = proAdd.ProductName;
            //    pp.TotalAmount = (Convert.ToDouble(pp.Quantity) * Convert.ToDouble(txtPrice.Text)).ToString();
            //    pp.newlyAdded = true;
            //    lstProdsMods.Add(pp);
            //    pp = new ProductModel();
            //}
            //dgvProducts.Items.Refresh();
            //dgvProducts.ItemsSource = lstProdsMods;
        }

        private async void loadProductsOnCombo()
        {
            try
            {
                cmbProducts.Items.Clear();
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                var collection = db.GetCollection<Products>("Products");

                var filter = Builders<Products>.Filter.And(
        Builders<Products>.Filter.Where(p => p.isDeleted == false));
                List<Products> lstPayments = collection.Find(filter).ToList();

                foreach (Products p in lstPayments)
                {
                    cmbProducts.Items.Add(p);
                }
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("EXCEPTION", "ERROR LOADING DATA " + ex.Message);
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            ProductsOrdered prodMod = dgvProducts.SelectedItem as ProductsOrdered;

            if (prodMod != null)
            {
                lstProductForDrugstores.Remove(prodMod);
                dgvProducts.ItemsSource = lstProductForDrugstores;
                dgvProducts.Items.Refresh();
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            double x = 0;
            double dblTotal = 0;
            double dblPaym = 0;
            if (FhDrugstoreView != null)
            {
                foreach (ProductsOrdered po in lstProductForDrugstores)
                {
                    x += po.Total;
                }
                FhDrugstoreView.lstProductsOrdered = lstProductForDrugstores;

                if (lstPaymentDrugstores != null && lstPaymentDrugstores.Count > 0)
                {

                    dblPaym = 0;
                    foreach (PaymentsDrugstores fds in lstPaymentDrugstores)
                    {
                        dblPaym += fds.Payment;
                    }
                }

                FhDrugstoreView.txtTotal.Text = (x -= dblPaym).ToString();

            }
            lstProductForDrugstores = new List<ProductsOrdered>();
            lstPaymentDrugstores = new List<PaymentsDrugstores>();

        }


        private void CheckIsNumeric(TextCompositionEventArgs e)
        {
            int result;

            if (!(int.TryParse(e.Text, out result) || e.Text == "."))
            {
                e.Handled = true;
            }
        }

        private void txtQuantity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtTotal_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private async Task<bool> checkFields()
        {
            bool ifAllCorrect = false;
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (cmbProducts.SelectedItem == null)
            {
                await window.ShowMessageAsync("Products", "Please select product.");
            }
            else if (string.IsNullOrEmpty(txtQuantity.Text))
            {
                await window.ShowMessageAsync("Quantity", "Please input value.");
            }
            else if (string.IsNullOrEmpty(txtQuantity.Text))
            {
                await window.ShowMessageAsync("Total", "Please input value.");
            }
            else
            {
                ifAllCorrect = true;
            }

            return ifAllCorrect;
        }

        private void clearFields()
        {
            cmbProducts.SelectedItem = null;
            txtQuantity.Text = "";
            txtTotal.Text = "0";

        }

        private void cmbProducts_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            //Products p = cmbProducts.SelectedItem as Products;

            //if (p != null)
            //{
            //    txtPrice.Text = p.Price.ToString();
            //}
        }

        private void txtQuantity_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            //double a = Convert.ToDouble(txtPrice.Text);
            //int b = !(string.IsNullOrEmpty(txtQuantity.Text)) ? Convert.ToInt32(txtQuantity.Text) : 0;

            //double dblTotal = b * a;
            //txtTotal.Text = dblTotal.ToString();
        }

        //private void loadProductsOnCombo()
        //{
        //    conDB = new ConnectionDB();
        //    ProductModel prod = new ProductModel();

        //    string queryString = "SELECT ID, productname, description FROM dbfh.tblproducts WHERE isDeleted = 0";

        //    MySqlDataReader reader = conDB.getSelectConnection(queryString, null);
        //    cmbProducts.Items.Clear();
        //    while (reader.Read())
        //    {
        //        prod.ProductID = reader["ID"].ToString();
        //        prod.ProductName = reader["productname"].ToString();
        //        prod.Description = reader["description"].ToString();

        //        cmbProducts.Items.Add(prod);

        //        prod = new ProductModel();
        //    }

        //    conDB.closeConnection();
        //}

    }
}
