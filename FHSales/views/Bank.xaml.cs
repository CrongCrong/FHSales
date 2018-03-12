using FHSales.Classes;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FHSales.views
{
    /// <summary>
    /// Interaction logic for Bank.xaml
    /// </summary>
    public partial class Bank : UserControl
    {
        public Bank()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        string recordID = "";

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgvBanks.ItemsSource = loadBankOnDataGrid();
            btnUpdate.Visibility = Visibility.Hidden;
        }

        private List<BankModel> loadBankOnDataGrid()
        {
            conDB = new ConnectionDB();
            BankModel bank = new BankModel();
            List<BankModel> lstBanks = new List<BankModel>();

            string queryString = "SELECT ID, bankname, description FROM dbfh.tblbank WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                bank.ID = reader["ID"].ToString();
                bank.BankName = reader["bankname"].ToString();
                bank.Description = reader["description"].ToString();
                lstBanks.Add(bank);
                bank = new BankModel();     
            }

            conDB.closeConnection();

            return lstBanks;
        }

        private void btnEditDirectSales_Click(object sender, RoutedEventArgs e)
        {
            BankModel selectedBank = dgvBanks.SelectedItem as BankModel;

            if (selectedBank != null)
            {
                recordID = selectedBank.ID;
                txtBankName.Text = selectedBank.BankName;
                txtDescription.Text = selectedBank.Description;
                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Visible;
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            updateRecord(recordID);
            await window.ShowMessageAsync("UPDATE RECORD", "Record updated successfully!");
            dgvBanks.ItemsSource = loadBankOnDataGrid();
            txtBankName.Text = "";
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
                txtBankName.Text = "";
                txtDescription.Text = "";
                dgvBanks.ItemsSource = loadBankOnDataGrid();
                await window.ShowMessageAsync("SAVE RECORD", "Record saved successfully!");
            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            txtBankName.Text = "";
            txtDescription.Text = "";
            recordID = "";
            btnUpdate.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;
        }

        private void saveRecord()
        {
            conDB = new ConnectionDB();

            string queryString = "INSERT INTO dbfh.tblbank (bankname, description, isDeleted) VALUES (?,?,0)";
            List<string> parameters = new List<string>();
            parameters.Add(txtBankName.Text);
            parameters.Add(txtDescription.Text);

            conDB.AddRecordToDatabase(queryString, parameters);

            conDB.closeConnection();
        }

        private void updateRecord(string strID)
        {
            conDB = new ConnectionDB();
            string queryString = "UPDATE dbfh.tblbank SET bankname = ?, description = ? WHERE ID = ?";

            List<string> parameters = new List<string>();
            parameters.Add(txtBankName.Text);
            parameters.Add(txtDescription.Text);
            parameters.Add(strID);

            conDB.AddRecordToDatabase(queryString, parameters);

            conDB.closeConnection();
        }

        private async Task<bool> checkFields()
        {
            bool ifAllCorrect = false;
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (string.IsNullOrEmpty(txtBankName.Text))
            {
                await window.ShowMessageAsync("BANK NAME", "Please provide bank name.");
            }else if (string.IsNullOrEmpty(txtDescription.Text))
            {
                await window.ShowMessageAsync("DESCRIPTION", "Please provide bank description");
            }else
            {
                ifAllCorrect = true;
            }

            return ifAllCorrect;
        }
    }
}
