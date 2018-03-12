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
    /// Interaction logic for FHFreebies.xaml
    /// </summary>
    public partial class FHFreebies : UserControl
    {
        public FHFreebies()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        FreebiesModel freebiesModel;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            searchDateFrom.IsEnabled = false;
            searchDateTo.IsEnabled = false;
            searchCategory.IsEnabled = false;

            dgvFreebies.ItemsSource = loadDataGridDetails();
            loadCategoryOnCombo();
            loadProductOnCombo();
            btnUpdate.Visibility = Visibility.Hidden;
            if (!UserModel.UserType.Equals(UserTypeEnum.DIRECTSALES_ADMIN) || !UserModel.UserType.Equals(UserTypeEnum.ADMIN))
            {
                btnEdit.Visibility = Visibility.Hidden;
                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Hidden;
                btnCancel.Visibility = Visibility.Hidden;
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool bl = await checkFields();
            if (bl)
            {
                MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

                saveRecord();
                await window.ShowMessageAsync("SAVE RECORD", "Record saved successfully!");
                dgvFreebies.ItemsSource = loadDataGridDetails();
                clearFields();
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool blCorrect = await checkFields();
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (blCorrect)
            {
                updateRecord(freebiesModel);
                clearFields();
                await window.ShowMessageAsync("UPDATE RECORD", "Record updated successfully!");
                dgvFreebies.ItemsSource = loadDataGridDetails();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            clearFields();
            btnSave.Visibility = Visibility.Visible;
            btnUpdate.Visibility = Visibility.Hidden;
            
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            freebiesModel = dgvFreebies.SelectedItem as FreebiesModel;
            btnUpdate.Visibility = Visibility.Visible;
            btnSave.Visibility = Visibility.Hidden;
            
            if(freebiesModel != null)
            {
                deliveryDate.Text = freebiesModel.DeliveryDate;
                txtClientName.Text = freebiesModel.ClientName;
                txtQuantity.Text = freebiesModel.Quantity.ToString();
                foreach(CategoryModel ct in cmbCategory.Items)
                {
                    if (ct.ID.Equals(freebiesModel.CategoryID))
                    {
                        cmbCategory.SelectedItem = ct;
                        
                    }
                }

                foreach(ProductModel pm in cmbProduct.Items)
                {
                    if (pm.ID.Equals(freebiesModel.ProductID))
                    {
                        cmbProduct.SelectedItem = pm;
                    }
                }

            }
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
                else if (checkCategory.IsChecked == true && searchCategory.SelectedItem == null)
                {
                    await window.ShowMessageAsync("SEARCH", "Please complete value to search.");
                }
                else
                {
                    dgvFreebies.ItemsSource = search();
                }
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            checkDate.IsChecked = false;
            checkCategory.IsChecked = false;
            dgvFreebies.ItemsSource = loadDataGridDetails();
        }


        private void checkCategory_Checked(object sender, RoutedEventArgs e)
        {
            searchCategory.IsEnabled = true;
        }

        private void checkCategory_Unchecked(object sender, RoutedEventArgs e)
        {
            searchCategory.IsEnabled = false;
            searchCategory.SelectedItem = null;
        }

        private void checkDate_Checked(object sender, RoutedEventArgs e)
        {
            searchDateFrom.IsEnabled = true;
            searchDateTo.IsEnabled = true;
        }

        private void checkDate_Unchecked(object sender, RoutedEventArgs e)
        {
            searchDateFrom.Text = "";
            searchDateTo.Text = "";
            searchDateFrom.IsEnabled = false;
            searchDateTo.IsEnabled = false;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Freebies_Report freebiesRep = new Freebies_Report();
            freebiesRep.ShowDialog();
        }

        private void clearFields()
        {
            deliveryDate.Text = "";
            txtClientName.Text = "";
            txtQuantity.Text = "";
            cmbCategory.SelectedItem = null;
            cmbProduct.SelectedItem = null;
        }

        private async Task<bool> checkFields()
        {
            bool ifAllCorrect = false;
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (string.IsNullOrEmpty(deliveryDate.Text))
            {
                await window.ShowMessageAsync("DELIVERY DATE", "Please select date.");
            }
            else if (string.IsNullOrEmpty(txtClientName.Text))
            {
                await window.ShowMessageAsync("CLIENT NAME", "Please provide client name.");
            }
            else if (string.IsNullOrEmpty(txtQuantity.Text))
            {
                await window.ShowMessageAsync("QUANTITY", "Please provide quantity.");
            }
            else if (cmbCategory.SelectedItem == null)
            {
                await window.ShowMessageAsync("CATEGORY", "Please select category.");
            }
            else
            {
                ifAllCorrect = true;
            }

            return ifAllCorrect;
        }

        private List<FreebiesModel> loadDataGridDetails()
        {
            conDB = new ConnectionDB();
            FreebiesModel freebies = new FreebiesModel();
            List<FreebiesModel> lstFreebies = new List<FreebiesModel>();

            string queryString = "SELECT dbfh.tblfreebies.ID, clientname, quantity, dbfh.tblcategory.description, dbfh.tblfreebies.categoryID as catID, deliverydate, dbfh.tblproducts.description as prod, dbfh.tblfreebies.productID as prodID  FROM ((dbfh.tblfreebies " +
                " INNER JOIN dbfh.tblcategory ON dbfh.tblfreebies.categoryID = dbfh.tblcategory.ID) INNER JOIN dbfh.tblproducts ON dbfh.tblfreebies.productID = dbfh.tblproducts.ID)" +
                " WHERE dbfh.tblfreebies.isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);
            while (reader.Read())
            {
                freebies.ID = reader["ID"].ToString();
                freebies.ClientName = reader["clientname"].ToString();
                freebies.Quantity = Convert.ToInt32(reader["quantity"].ToString());
                freebies.CategoryName = reader["description"].ToString();
                DateTime dte = DateTime.Parse(reader["deliveryDate"].ToString());
                freebies.DeliveryDate = dte.ToShortDateString();
                freebies.CategoryID = reader["catID"].ToString();
                freebies.ProductName = reader["prod"].ToString();
                freebies.ProductID = reader["prodID"].ToString();
                
                lstFreebies.Add(freebies);
                freebies = new FreebiesModel();
            }
            conDB.closeConnection();

            return lstFreebies;
        }

        private void loadCategoryOnCombo()
        {
            conDB = new ConnectionDB();
            CategoryModel cat = new CategoryModel();

            string queryString = "SELECT ID, categoryname, description FROM dbfh.tblcategory WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);
            cmbCategory.Items.Clear();
            searchCategory.Items.Clear();
            while (reader.Read())
            {
                cat.ID = reader["ID"].ToString();
                cat.CategoryName = reader["categoryname"].ToString();
                cat.Description = reader["description"].ToString();

                cmbCategory.Items.Add(cat);
                searchCategory.Items.Add(cat);
                cat = new CategoryModel();
            }

            conDB.closeConnection();
        }

        private void loadProductOnCombo()
        {
            conDB = new ConnectionDB();
            ProductModel prod = new ProductModel();

            string queryString = "SELECT ID, productname, description FROM dbfh.tblproducts WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);
            cmbProduct.Items.Clear();
            
            while (reader.Read())
            {
                prod.ID = reader["ID"].ToString();
                prod.ProductName = reader["productname"].ToString();
                prod.Description = reader["description"].ToString();

                cmbProduct.Items.Add(prod);
                
                prod = new ProductModel();
            }

            conDB.closeConnection();
        }

        private void saveRecord()
        {
            conDB = new ConnectionDB();

            string queryString = "INSERT INTO dbfh.tblfreebies (clientname, quantity, categoryID, productID, deliverydate, isDeleted) VALUES " +
                " (?,?,?,?,?,0)";
            List<string> parameters = new List<string>();

            parameters.Add(txtClientName.Text);
            parameters.Add(txtQuantity.Text);
            parameters.Add(cmbCategory.SelectedValue.ToString());
            parameters.Add(cmbProduct.SelectedValue.ToString());
            DateTime delDate = DateTime.Parse(deliveryDate.Text);
            parameters.Add(delDate.Year + "-" + delDate.Month + "-" + delDate.Day);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();

        }

        private void updateRecord(FreebiesModel fribis)
        {
            conDB = new ConnectionDB();

            string queryString = "UPDATE dbfh.tblfreebies SET clientname = ?, quantity = ?, categoryID = ?,  productID = ?, " +
                "deliverydate = ? WHERE ID = ?";

            List<string> parameters = new List<string>();
            parameters.Add(txtClientName.Text);
            parameters.Add(txtQuantity.Text);
            parameters.Add(cmbCategory.SelectedValue.ToString());
            parameters.Add(cmbProduct.SelectedValue.ToString());
            DateTime date = DateTime.Parse(deliveryDate.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
            parameters.Add(fribis.ID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
        }

        private List<FreebiesModel> search()
        {
            conDB = new ConnectionDB();
            FreebiesModel freebies = new FreebiesModel();
            List<FreebiesModel> lstFreebies = new List<FreebiesModel>();
            string queryString = "SELECT dbfh.tblfreebies.ID, clientname, quantity, dbfh.tblcategory.description, dbfh.tblfreebies.categoryID as catID, deliverydate, dbfh.tblproducts.description as prod, dbfh.tblfreebies.productID as prodID  FROM ((dbfh.tblfreebies " +
                " INNER JOIN dbfh.tblcategory ON dbfh.tblfreebies.categoryID = dbfh.tblcategory.ID) INNER JOIN dbfh.tblproducts ON dbfh.tblfreebies.productID = dbfh.tblproducts.ID)" +
                " WHERE dbfh.tblfreebies.isDeleted = 0";

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
                queryString += " AND (dbfh.tblfreebies.categoryID = ?)";
                parameters.Add(searchCategory.SelectedValue.ToString());
            }

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                freebies.ID = reader["ID"].ToString();
                freebies.ClientName = reader["clientname"].ToString();
                freebies.Quantity = Convert.ToInt32(reader["quantity"].ToString());
                freebies.CategoryName = reader["description"].ToString();
                DateTime dte = DateTime.Parse(reader["deliveryDate"].ToString());
                freebies.DeliveryDate = dte.ToShortDateString();
                freebies.CategoryID = reader["catID"].ToString();
                freebies.ProductName = reader["prod"].ToString();
                freebies.ProductID = reader["prodID"].ToString();
                lstFreebies.Add(freebies);

            }
            conDB.closeConnection();

            return lstFreebies;
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
