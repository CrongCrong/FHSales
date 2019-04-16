using FHSales.Classes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace FHSales
{
    /// <summary>
    /// Interaction logic for DirectSalesDetails.xaml
    /// </summary>
    public partial class DirectSalesDetails : MetroWindow
    {
        public DirectSalesDetails()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        FHSales.views.FHBoxes fhBoxes;
        DirectSalesModel directSalesModel;

        public DirectSalesDetails(FHSales.views.FHBoxes f, DirectSalesModel dm)
        {
            fhBoxes = f;
            directSalesModel = dm;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            getCourierList();
            getBankList();
            if (directSalesModel != null)
            {
                deliveryDateDS.Text = directSalesModel.DeliveryDate;
                txtClientName.Text = directSalesModel.ClientName;
                txtQuantity.Text = directSalesModel.Quantity;
                txtExpenses.Text = directSalesModel.Expenses.ToString();
                txtTotalPrice.Text = directSalesModel.TotalPrice.ToString();
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool blCorrect = await checkFields();
            if (blCorrect)
            {
                updateDirectSalesRecord();
                await this.ShowMessageAsync("UPDATE RECORD", "Record updated successfully!");
                fhBoxes.dgvDirectSales.ItemsSource = loadDataGridDetailsDirectSales();
                this.Close();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async Task<bool> checkFields()
        {
            bool ifAllCorrect = false;

            if (string.IsNullOrEmpty(deliveryDateDS.Text))
            {
                await this.ShowMessageAsync("Date", "Please select date.");
            }
            else if (string.IsNullOrEmpty(txtClientName.Text))
            {
                await this.ShowMessageAsync("Client Nam", "Please input client name.");
            }
            else if (string.IsNullOrEmpty(txtQuantity.Text))
            {
                await this.ShowMessageAsync("Quantity", "Please input quantity.");
            }
            else if (cmbCashBank.SelectedItem == null)
            {
                await this.ShowMessageAsync("Cash/Bank", "Please select if cash or bank.");
            }
            else if (cmbCourier.SelectedItem == null)
            {
                await this.ShowMessageAsync("Courier", "Please select courier.");
            }
            else if (string.IsNullOrEmpty(txtExpenses.Text))
            {
                await this.ShowMessageAsync("Expenses", "Please input expenses.");
            }
            else if (string.IsNullOrEmpty(txtTotalPrice.Text))
            {
                await this.ShowMessageAsync("Total Price", "Please input total price");
            }
            else
            {

                ifAllCorrect = true;
            }

            return ifAllCorrect;
        }

        private void getCourierList()
        {
            conDB = new ConnectionDB();
            string queryString = "SELECT ID, couriername, description FROM dbfh.tblcourier WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                CourierModel courier = new CourierModel();
                courier.ID = reader["ID"].ToString();
                courier.CourierName = reader["couriername"].ToString();
                courier.Description = reader["description"].ToString();

                cmbCourier.Items.Add(courier);
                if (directSalesModel != null)
                {
                    if (courier.ID.Equals(directSalesModel.CourierID))
                    {
                        cmbCourier.SelectedItem = courier;
                    }
                }
            }
            conDB.closeConnection();
        }

        private void getBankList()
        {
            string queryString = "SELECT ID, bankname, description FROM dbfh.tblbank WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                BankModel bank = new BankModel();
                bank.ID = reader["ID"].ToString();
                bank.BankName = reader["bankname"].ToString();
                bank.Description = reader["description"].ToString();

                cmbCashBank.Items.Add(bank);
                if (directSalesModel != null)
                {
                    if (bank.ID.Equals(directSalesModel.CashBankID))
                    {
                        cmbCashBank.SelectedItem = bank;
                    }
                }
            }
            conDB.closeConnection();
        }

        private void updateDirectSalesRecord()
        {
            string queryString = "UPDATE dbfh.tbldirectsales SET deliverydate = ?, clientname = ?, quantity = ?, cashbankID = ?, " +
                "courierID = ?, expenses = ?, totalprice = ? WHERE ID = ?";

            List<string> parameters = new List<string>();
            DateTime date = DateTime.Parse(deliveryDateDS.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            parameters.Add(txtClientName.Text);
            parameters.Add(txtQuantity.Text);
            parameters.Add(cmbCashBank.SelectedValue.ToString());
            parameters.Add(cmbCourier.SelectedValue.ToString());
            parameters.Add(txtExpenses.Text);
            parameters.Add(txtTotalPrice.Text);
            parameters.Add(directSalesModel.ID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();

        }

        private List<DirectSalesModel> loadDataGridDetailsDirectSales()
        {
            conDB = new ConnectionDB();
            List<DirectSalesModel> lstDirectSales = new List<DirectSalesModel>();
            DirectSalesModel directSales = new DirectSalesModel();

            string queryString = "SELECT dbfh.tbldirectsales.ID, clientname, quantity, cashbankID, bankname, courierID, " +
                "couriername, totalprice, deliverydate, expenses FROM((dbfh.tbldirectsales INNER JOIN dbfh.tblbank ON " +
                "dbfh.tbldirectsales.cashbankID = dbfh.tblbank.ID) INNER JOIN dbfh.tblcourier ON " +
                "dbfh.tbldirectsales.courierID = dbfh.tblcourier.ID) WHERE dbfh.tbldirectsales.isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                directSales.ID = reader["ID"].ToString();
                directSales.ClientName = reader["clientname"].ToString();
                directSales.Quantity = reader["quantity"].ToString();
                directSales.CashBankID = reader["cashbankID"].ToString();
                directSales.CashBankName = reader["bankname"].ToString();
                directSales.CourierID = reader["courierID"].ToString();
                directSales.CourierName = reader["couriername"].ToString();
                directSales.TotalPrice = Convert.ToInt32(reader["totalprice"].ToString());

                DateTime dte = DateTime.Parse(reader["deliveryDate"].ToString());
                directSales.DeliveryDate = dte.ToShortDateString();

                directSales.Expenses = Convert.ToInt32(reader["expenses"].ToString());

                lstDirectSales.Add(directSales);
                directSales = new DirectSalesModel();
            }
            conDB.closeConnection();

            return lstDirectSales;

        }
    }
}
