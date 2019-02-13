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
    /// Interaction logic for Freebies.xaml
    /// </summary>
    public partial class Freebies : UserControl
    {
        public Freebies()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        string recordID = "";
             
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgvFreebies.ItemsSource = loadDataGridDetails();
            btnUpdate.Visibility = Visibility.Hidden;
            if (!UserModel.isDSAdmin)
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
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (bl)
            {
                saveRecord();
                txtCategory.Text = "";
                txtDescription.Text = "";
                dgvFreebies.ItemsSource = loadDataGridDetails();
                await window.ShowMessageAsync("SAVE RECORD", "Record saved successfully!");
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            updateRecord(recordID);
            await window.ShowMessageAsync("UPDATE RECORD", "Record updated successfully!");
            dgvFreebies.ItemsSource = loadDataGridDetails();
            txtCategory.Text = "";
            txtDescription.Text = "";
            recordID = "";
            btnUpdate.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;
        }

        private void btnEditDirectSales_Click(object sender, RoutedEventArgs e)
        {
            CategoryModel catMod = dgvFreebies.SelectedItem as CategoryModel;
            if(catMod != null)
            {
                recordID = catMod.ID;
                txtCategory.Text = catMod.CategoryName;
                txtDescription.Text = catMod.Description;
                btnUpdate.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Hidden;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            txtCategory.Text = "";
            txtDescription.Text = "";
            recordID = "";
            btnUpdate.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;
        }

        private async Task<bool> checkFields()
        {
            bool ifAllCorrect = false;
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (string.IsNullOrEmpty(txtCategory.Text))
            {
                await window.ShowMessageAsync("CATEGORY", "Please provide category name.");
            }
            else if (string.IsNullOrEmpty(txtDescription.Text))
            {
                await window.ShowMessageAsync("DESCRIPTION", "Please provide category description");
            }
            else
            {
                ifAllCorrect = true;
            }

            return ifAllCorrect;
        }

        private List<CategoryModel> loadDataGridDetails()
        {
            conDB = new ConnectionDB();

            List<CategoryModel> lstCategory = new List<CategoryModel>();
            CategoryModel category = new CategoryModel();

            string queryString = "SELECT ID, categoryname, description FROM dbfh.tblcategory WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                category.ID = reader["ID"].ToString();
                category.CategoryName = reader["categoryname"].ToString();
                category.Description = reader["description"].ToString();
                lstCategory.Add(category);
                category = new CategoryModel();

            }

            conDB.closeConnection();
            return lstCategory;
        }

        private void saveRecord()
        {
            conDB = new ConnectionDB();
            string queryString = "INSERT INTO dbfh.tblcategory (categoryname, description, isDeleted) VALUE (?,?,0)";

            List<string> parameters = new List<string>();
            parameters.Add(txtCategory.Text);
            parameters.Add(txtDescription.Text);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();

        }

        private void updateRecord(string strID)
        {
            conDB = new ConnectionDB();

            string queryString = "UPDATE dbfh.tblcategory SET categoryname = ?, description = ? WHERE ID = ?";
            List<string> parameters = new List<string>();
            parameters.Add(txtCategory.Text);
            parameters.Add(txtDescription.Text);
            parameters.Add(strID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();

        }
    }
}
