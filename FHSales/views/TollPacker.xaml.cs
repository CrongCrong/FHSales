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
    /// Interaction logic for TollPacker.xaml
    /// </summary>
    public partial class TollPacker : UserControl
    {
        public TollPacker()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        string queryString = "";
        List<string> parameters;
        string recID = "";

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgvTollPacker.ItemsSource = loadDataGridDetails();
            btnUpdate.Visibility = Visibility.Hidden;
        }   

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            TollPackerModel tollMod = dgvTollPacker.SelectedItem as TollPackerModel;
            
            if(tollMod != null)
            {
                recID = tollMod.ID;
                txtTollPacker.Text = tollMod.TollpackerName;
                txtDescription.Text = tollMod.Description;
                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Visible;
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (x)
            {
                updateRecord(recID);
                clearFields();
                dgvTollPacker.ItemsSource = loadDataGridDetails();
                await window.ShowMessageAsync("Update Record", "Record updated successfully!");
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (x)
            {
                saveRecord();
                clearFields();
                dgvTollPacker.ItemsSource = loadDataGridDetails();
                await window.ShowMessageAsync("Save Record", "Record successfully saved!");
            }
        }

        private void updateRecord(string strID)
        {
            conDB = new ConnectionDB();
            queryString = "UPDATE dbfh.tbltollpacker SET name = ?, description = ? WHERE ID = ?";

            parameters = new List<string>();
            parameters.Add(txtTollPacker.Text);
            parameters.Add(txtDescription.Text);
            parameters.Add(strID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            clearFields();
            btnUpdate.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;
        }

        private async Task<bool> checkFields()
        {
            bool ifOkay = false;
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (string.IsNullOrEmpty(txtTollPacker.Text))
                   {
                await window.ShowMessageAsync("Toll Packer", "Please input name.");
            }else if (string.IsNullOrEmpty(txtDescription.Text))
            {
                await window.ShowMessageAsync("Description", "Please input description.");
            }else
            {
                ifOkay = true;
            }

            return ifOkay;
        }

        private void clearFields()
        {
            txtTollPacker.Text = "";
            txtDescription.Text = "";
            
        }

        private List<TollPackerModel> loadDataGridDetails()
        {
            conDB = new ConnectionDB();
            queryString = "SELECT ID, name, description FROM dbfh.tbltollpacker WHERE isDeleted = 0";
            List<TollPackerModel> lstTollpacker = new List<TollPackerModel>();
            TollPackerModel tollPacker = new TollPackerModel();

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                tollPacker.ID = reader["ID"].ToString();
                tollPacker.TollpackerName = reader["name"].ToString();
                tollPacker.Description = reader["description"].ToString();

                lstTollpacker.Add(tollPacker);
                tollPacker = new TollPackerModel();
            }

            conDB.closeConnection();
            return lstTollpacker;
        }

        private void saveRecord()
        {
            conDB = new ConnectionDB();
            queryString = "INSERT INTO dbfh.tbltollpacker (name, description, isDeleted) VALUES (?,?,0)";

            parameters = new List<string>();
            parameters.Add(txtTollPacker.Text);
            parameters.Add(txtDescription.Text);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
        }

    }
}
