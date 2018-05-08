using FHSales.Classes;
using FHSales.views;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Windows;
using System;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;

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
        bool ifEdit = false;
        string strSalesID = "";
        ProductModel prodToEdit;

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
                ifEdit = true;
                dgvProducts.ItemsSource = lstProductModel;
                dgvProducts.Items.Refresh();
                strSalesID = lstProductModel[0].SalesID;
                //btnAdd.IsEnabled = false;
                //btnRemove.IsEnabled = false;
            }
        }

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ProductModel prodAdd = cmbProducts.SelectedItem as ProductModel;
            bool x = await checkFields();

            if (x)
            {
                if (string.IsNullOrEmpty(txtQuantity.Text))
                {
                    prodAdd.Quantity = "0";
                }
                else
                {
                    prodAdd.Quantity = txtQuantity.Text;
                }

                if (string.IsNullOrEmpty(txtTotal.Text))
                {
                    prodAdd.TotalAmount = "0";
                }
                else
                {
                    prodAdd.TotalAmount = txtTotal.Text;
                }

                if (ifEdit)
                {
                    prodAdd.newlyAdded = true;
                    prodAdd.SalesID = strSalesID;

                    lstProductModel.Add(prodAdd);
                    dgvProducts.ItemsSource = lstProductModel;
                    dgvProducts.Items.Refresh();
                }
                else
                {
                    prodAdd.isDeleted = "0";
                    lstProductModel.Add(prodAdd);
                    dgvProducts.ItemsSource = lstProductModel;
                    dgvProducts.Items.Refresh();
                }
                clearFields();
            }
            
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
                if (ifEdit && !prodMod.newlyAdded)
                {
                    foreach(ProductModel p in lstProductModel)
                    {
                        if (p.ID.Equals(prodMod.ID))
                        {
                            p.isDeleted = "1";
                        }
                    }

                    dgvProducts.ItemsSource = lstProductModel;
                    dgvProducts.Items.Refresh();

                }else if(ifEdit && prodMod.newlyAdded)
                {
                    lstProductModel.Remove(prodMod);
                    dgvProducts.ItemsSource = lstProductModel;
                    dgvProducts.Items.Refresh();
                }
                else
                {
                    lstProductModel.Remove(prodMod);
                    dgvProducts.ItemsSource = lstProductModel;
                    dgvProducts.Items.Refresh();
                }
                
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(lstProductModel != null)
            {
                FhBoxesViews.lstProductModel = lstProductModel;
            }
            double x = 0;
            foreach (ProductModel p in lstProductModel)
            {
                if (!p.isDeleted.Equals("1"))
                {
                    x += Convert.ToDouble(p.TotalAmount);
                }
                
            }
            FhBoxesViews.txtTotalPrice.Text = x.ToString();
            
        }

        //private void btnEdit_Click(object sender, RoutedEventArgs e)
        //{
        //    ProductModel pm = dgvProducts.SelectedItem as ProductModel;
        //    btnUpdate.Visibility = Visibility.Visible;
        //    prodToEdit = pm;
        //    if(pm != null)
        //    {
        //        foreach(ProductModel p in cmbProducts.Items)
        //        {
        //            if (p.ID.Equals(pm.ID))
        //            {
        //                cmbProducts.SelectedItem = p;
        //            }
        //        }
        //        txtQuantity.Text = pm.Quantity;
        //        txtTotal.Text = pm.TotalAmount;
        //    }
        //}

        //private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        //{
        //    ProductModel pm = cmbProducts.SelectedItem as ProductModel;
        //    bool x = await checkFields();
        //    if (pm != null)
        //    {
        //        if (x)
        //        {
        //            foreach (ProductModel p in lstProductModel)
        //            {
        //                if (prodToEdit.Equals(p))
        //                {
        //                    if (string.IsNullOrEmpty(txtQuantity.Text))
        //                    {
        //                        p.Quantity = "0";
        //                    }
        //                    else
        //                    {
        //                        p.Quantity = txtQuantity.Text;
        //                    }

        //                    if (string.IsNullOrEmpty(txtTotal.Text))
        //                    {
        //                        p.TotalAmount = "0";
        //                    }
        //                    else
        //                    {
        //                        p.TotalAmount = txtTotal.Text;
        //                    }
        //                    p.ID = pm.ID;
        //                    p.ProductName = pm.ProductName;
        //                }
        //            }
        //        }
                
        //    }

        //    dgvProducts.ItemsSource = lstProductModel;
        //    dgvProducts.Items.Refresh();
        //    clearFields();
        //    btnUpdate.Visibility = Visibility.Hidden;
        //    btnAdd.Visibility = Visibility.Visible;
            
        //}

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
            txtTotal.Text = "";
        }

    }
}
