using FHSales.Classes;
using FHSales.MongoClasses;
using FHSales.views;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MongoDB.Driver;
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
using System.Windows.Shapes;

namespace FHSales
{
    /// <summary>
    /// Interaction logic for AddProductDirectDaily.xaml
    /// </summary>
    public partial class AddProductDirectDaily : MetroWindow
    {
        public AddProductDirectDaily()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        List<ProductsOrderedDS> lstProdDS = new List<ProductsOrderedDS>();
        DirectSalesDailyView directSalesDailyView;
        MahApps.Metro.Controls.MetroWindow window;



        public AddProductDirectDaily(DirectSalesDailyView dsv, List<ProductsOrderedDS> lstpod)
        {
            window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
            directSalesDailyView = dsv;
            lstProdDS = lstpod;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            btnUpdate.Visibility = Visibility.Hidden;
            loadProductsOnCombo();
            if (directSalesDailyView != null)
            {
                dgvProductsOrdered.ItemsSource = lstProdDS;
            }
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

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            double dblTotal = 0;

            foreach (ProductsOrderedDS p in lstProdDS)
            {
                dblTotal += Convert.ToDouble(p.Price);
            }
            directSalesDailyView.lstProductsOrderedDS = lstProdDS;
            directSalesDailyView.txtTotalPrice.Text = dblTotal.ToString();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            ProductsOrderedDS ps = new ProductsOrderedDS();
            Products p = cmbProducts.SelectedItem as Products;

            if (p != null)
            {
                ps.Products = p;
                ps.Price = txtPrice.Text;
                ps.Qty = txtQty.Text;
            }

            lstProdDS.Add(ps);
            dgvProductsOrdered.ItemsSource = lstProdDS;
            dgvProductsOrdered.Items.Refresh();
            ps = new ProductsOrderedDS();
            clearFields();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            ProductsOrderedDS ps = dgvProductsOrdered.SelectedItem as ProductsOrderedDS;

            if (ps != null)
            {
                lstProdDS.Remove(ps);
                dgvProductsOrdered.ItemsSource = lstProdDS;
                dgvProductsOrdered.Items.Refresh();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            clearFields();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void clearFields()
        {
            cmbProducts.SelectedItem = null;
            txtPrice.Text = "";
            txtQty.Text = "";
        }
    }
}
