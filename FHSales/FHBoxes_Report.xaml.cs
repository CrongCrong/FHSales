using FHSales.Classes;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace FHSales
{
    /// <summary>
    /// Interaction logic for FHBoxes_Report.xaml
    /// </summary>
    public partial class FHBoxes_Report : MetroWindow
    {
        public FHBoxes_Report()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            loadCashBankonCombo();
        }

        private void loadCashBankonCombo()
        {
            conDB = new ConnectionDB();
            BankModel bankCash = new BankModel();

            string queryString = "SELECT ID, bankname, description FROM dbfh.tblbank WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);
            cmbCashBank.Items.Clear();
            while (reader.Read())
            {
                bankCash.ID = reader["ID"].ToString();
                bankCash.BankName = reader["bankname"].ToString();
                bankCash.Description = reader["description"].ToString();

                cmbCashBank.Items.Add(bankCash);
                bankCash = new BankModel();
            }

            conDB.closeConnection();

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            if ((!string.IsNullOrEmpty(dateFrom.Text) && !string.IsNullOrEmpty(dateTo.Text)) &&
                cmbCashBank.SelectedItem != null)
            {
                BankModel bnk = cmbCashBank.SelectedItem as BankModel;
                ReportForm rf = new ReportForm(lstFHBoxesSold());
                rf.ShowDialog();
            }

        }

        private List<DirectSalesModel> lstFHBoxesSold()
        {
            List<DirectSalesModel> lstDS = new List<DirectSalesModel>();
            DirectSalesModel ds = new DirectSalesModel();

            conDB = new ConnectionDB();
            string queryString = "SELECT deliverydate, clientname, totalamt, qty, remarks " +
                "FROM (dbfh.tbldirectsales INNER JOIN dbfh.tblproductsordered ON tbldirectsales.ID = " +
                "tblproductsordered.salesID) WHERE tbldirectsales.isDeleted = 0 AND tblproductsordered.isDeleted = 0 " +
                "AND tblproductsordered.productID =  1 AND cashbankID = ? AND deliverydate BETWEEN ? AND ? AND isPaid = ?";

            List<string> parameters = new List<string>();
            parameters.Add(cmbCashBank.SelectedValue.ToString());

            DateTime date = DateTime.Parse(dateFrom.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            date = DateTime.Parse(dateTo.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            string iPaid = (chkPaid.IsChecked.Value) ? "1" : "0";
            parameters.Add(iPaid);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                DateTime dte = DateTime.Parse(reader["deliverydate"].ToString());
                ds.DeliveryDate = dte.ToShortDateString();
                ds.ClientName = reader["clientname"].ToString();
                ds.TotalPrice = Convert.ToInt32(reader["totalamt"].ToString());
                ds.Quantity = reader["qty"].ToString();
                ds.Remarks = reader["remarks"].ToString();
                lstDS.Add(ds);
                ds = new DirectSalesModel();
            }
            conDB.closeConnection();
            return lstDS;
        }

    }
}
