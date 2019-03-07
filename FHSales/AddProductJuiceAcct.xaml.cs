using FHSales.Classes;
using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace FHSales
{
    /// <summary>
    /// Interaction logic for AddProductJuiceAcct.xaml
    /// </summary>
    public partial class AddProductJuiceAcct : MetroWindow
    {
        public AddProductJuiceAcct()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        string queryString = "";
        List<string> parameters;
        ViewDRJuice viewDR;
        FHJuiceProductModel productToUpdate;
        List<FHJuiceProductModel> lstFHproducts;
        bool ifUpdate = false;

        public AddProductJuiceAcct(ViewDRJuice vdj, List<FHJuiceProductModel> lstFHprod)
        {
            viewDR = vdj;
            lstFHproducts = lstFHprod;
            InitializeComponent();
        }

        public AddProductJuiceAcct(ViewDRJuice vdj, List<FHJuiceProductModel> lstFHprod, bool ifup)
        {
            viewDR = vdj;
            lstFHproducts = lstFHprod;
            ifUpdate = ifup;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            loadProductsOnCombo();
            btnUpdate.Visibility = Visibility.Hidden;
            if(lstFHproducts != null)
            {
                dgvProductsOrdered.ItemsSource = lstFHproducts;
            }
        }

        private void getProductsOnRecord()
        {
            conDB = new ConnectionDB();
            queryString = "";
        }

        private void loadProductsOnCombo()
        {
            conDB = new ConnectionDB();
            FHJuiceProductModel prod = new FHJuiceProductModel();
            queryString = "SELECT ID, productname, description FROM dbjuiceacct.tblproducts WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                prod.ID = reader["ID"].ToString();
                prod.ProductName = reader["productname"].ToString();
                prod.Description = reader["description"].ToString();

                cmbProducts.Items.Add(prod);
                prod = new FHJuiceProductModel();
            }

            conDB.closeConnection();
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();

            if (x)
            {
                FHJuiceProductModel prod = new FHJuiceProductModel();
                FHJuiceProductModel selectedProd = cmbProducts.SelectedItem as FHJuiceProductModel;
                prod.ProductName = selectedProd.ProductName;
                prod.ID = selectedProd.ID;
                prod.Price = txtPrice.Text;
                prod.Qty = txtQty.Text;
                prod.Remarks = txtRemarks.Text;
                prod.Total = (Convert.ToDouble(prod.Price) * Convert.ToDouble(txtQty.Text)).ToString();

                if (!ifUpdate)
                {
                    prod.NewlyAdded = false;
                    lstFHproducts.Add(prod);
                    prod = new FHJuiceProductModel();
                }
                else
                {
                    prod.NewlyAdded = true;
                    lstFHproducts.Add(prod);
                    prod = new FHJuiceProductModel();
                }

            }
            dgvProductsOrdered.Items.Refresh();
            dgvProductsOrdered.ItemsSource = lstFHproducts;

        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            FHJuiceProductModel slctCmb = cmbProducts.SelectedItem as FHJuiceProductModel;

            foreach (FHJuiceProductModel pp in lstFHproducts)
            {
                if (productToUpdate.ID.Equals(pp.ID))
                {
                    pp.ID = slctCmb.ID;
                    pp.ProductName = slctCmb.ProductName;
                    pp.Qty = txtQty.Text;
                    pp.Price = txtPrice.Text;
                    pp.Remarks = txtRemarks.Text;
                    pp.Total = (Convert.ToDouble(txtQty.Text) * Convert.ToDouble(txtPrice.Text)).ToString();
                }
            }
            dgvProductsOrdered.ItemsSource = lstFHproducts;
            dgvProductsOrdered.Items.Refresh();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            FHJuiceProductModel p = dgvProductsOrdered.SelectedItem as FHJuiceProductModel;

            if (p != null)
            {
                lstFHproducts.Remove(p);

                dgvProductsOrdered.ItemsSource = lstFHproducts;
                dgvProductsOrdered.Items.Refresh();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            productToUpdate = dgvProductsOrdered.SelectedItem as FHJuiceProductModel;

            if (productToUpdate != null)
            {
                foreach (ProductModel pp in cmbProducts.Items)
                {
                    if (productToUpdate.ID.Equals(pp.ID))
                    {
                        cmbProducts.SelectedItem = pp;
                        break;
                    }
                }
                txtPrice.Text = productToUpdate.Price;
                txtQty.Text = productToUpdate.Qty;
                txtRemarks.Text = productToUpdate.Remarks;
                button.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Visible;
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            viewDR.lstFHProducts = lstFHproducts;
            double dblTemp = 0;
            foreach (FHJuiceProductModel p in lstFHproducts)
            {
                dblTemp = dblTemp + Convert.ToDouble(p.Total);

            }
            viewDR.txtTotal.Text = dblTemp.ToString();
        }

        private async Task<bool> checkFields()
        {
            bool ifCorrect = false;

            if (cmbProducts.SelectedItem == null)
            {
                await this.ShowMessageAsync("Products", "Please select product");
            }
            else if (string.IsNullOrEmpty(txtQty.Text))
            {
                await this.ShowMessageAsync("Quantity", "Please input value");
            }
            else if (string.IsNullOrEmpty(txtPrice.Text))
            {
                await this.ShowMessageAsync("Price", "Please input value");
            }
            else
            {
                ifCorrect = true;
            }

            return ifCorrect;
        }

        private void txtPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
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

        private void txtQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }
    }
}
