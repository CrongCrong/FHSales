using FHSales.Classes;
using FHSales.MongoClasses;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MongoDB.Driver;
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
            loadPaymentModeOnCombo();
            loadProductsOnCombo();
        }

        //private void loadCashBankonCombo()
        //{
        //    conDB = new ConnectionDB();
        //    BankModel bankCash = new BankModel();

        //    string queryString = "SELECT ID, bankname, description FROM dbfh.tblbank WHERE isDeleted = 0";

        //    MySqlDataReader reader = conDB.getSelectConnection(queryString, null);
        //    cmbCashBank.Items.Clear();
        //    while (reader.Read())
        //    {
        //        bankCash.ID = reader["ID"].ToString();
        //        bankCash.BankName = reader["bankname"].ToString();
        //        bankCash.Description = reader["description"].ToString();

        //        cmbCashBank.Items.Add(bankCash);
        //        bankCash = new BankModel();
        //    }

        //    conDB.closeConnection();

        //}
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
                await this.ShowMessageAsync("EXCEPTION", "ERROR LOADING DATA " + ex.Message);
            }
        }

        private async void loadPaymentModeOnCombo()
        {
            try
            {
                cmbCashBank.Items.Clear();
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();

                var db = client.GetDatabase("DBFH");

                var collection = db.GetCollection<Banks>("Banks");

                var filter = Builders<Banks>.Filter.And(
        Builders<Banks>.Filter.Where(p => p.isDeleted == false));
                List<Banks> lstPayments = collection.Find(filter).ToList();
                foreach (Banks p in lstPayments)
                {
                    cmbCashBank.Items.Add(p);

                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("EXCEPTION", "ERROR LOADING DATA " + ex.Message);
            }
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            if ((!string.IsNullOrEmpty(dateFrom.Text) && !string.IsNullOrEmpty(dateTo.Text)) &&
                cmbCashBank.SelectedItem != null && cmbProducts.SelectedItem != null)
            {
                BankModel bnk = cmbCashBank.SelectedItem as BankModel;
                ReportForm rf = new ReportForm(await lstFHBoxesSoldDS());
                rf.ShowDialog();
            }
            else
            {
                await this.ShowMessageAsync("ERROR", " Please select values");
            }

        }

        private async System.Threading.Tasks.Task<List<DirectSalesDaily>> lstFHBoxesSoldDS()
        {
            Banks bb = cmbCashBank.SelectedItem as Banks;
            DateTime dteNow = (!string.IsNullOrEmpty(dateFrom.Text)) ? DateTime.Parse(dateFrom.Text) : DateTime.Now;
            DateTime dteFirstDay = (!string.IsNullOrEmpty(dateTo.Text)) ? DateTime.Parse(dateTo.Text) : DateTime.Now;
            Products pp = cmbProducts.SelectedItem as Products;
            List<DirectSalesDaily> lstDS = new List<DirectSalesDaily>();
            try
            {
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                var collection = db.GetCollection<DirectSalesDaily>("DirectSalesDaily");

                var filter = Builders<DirectSalesDaily>.Filter.And(
        Builders<DirectSalesDaily>.Filter.Where(p => p.isDeleted == false),
        Builders<DirectSalesDaily>.Filter.Where(c => c.Client.isDeleted == false));

                filter = filter & Builders<DirectSalesDaily>.Filter.And(
          Builders<DirectSalesDaily>.Filter.Where(d => d.Bank.Id == bb.Id));

                filter = filter & Builders<DirectSalesDaily>.Filter.And(
           Builders<DirectSalesDaily>.Filter.Gte("DateOrdered", dteNow),
          Builders<DirectSalesDaily>.Filter.Lte("DateOrdered", dteFirstDay));

                filter = filter & Builders<DirectSalesDaily>.Filter.And(
                    Builders<DirectSalesDaily>.Filter.ElemMatch(prod => prod.ProductsOrdered, x => x.Products == pp));


                filter = filter & Builders<DirectSalesDaily>.Filter.Where(p => p.isPaid == chkPaid.IsChecked);

                lstDS = collection.Find(filter).ToList();
                double dbTotalQty = 0;
                double dbTotalPrice = 0;
                foreach (DirectSalesDaily dd in lstDS)
                {
                    dd.strDateOrdered = dd.DateOrdered.ToShortDateString();
                    dd.strClientFullName = dd.Client.LastName + ", " + dd.Client.FirstName;

                    foreach (ProductsOrderedDS ppp in dd.ProductsOrdered)
                    {
                        if (ppp.Products != null)
                        {
                            if (ppp.Products.Id.Equals(pp.Id))
                            {
                                dd.strProductName = ppp.Products.ProductName;
                                dbTotalQty = dbTotalQty + Convert.ToDouble(ppp.Qty);
                                dd.strTotalBoxes = dbTotalQty.ToString();
                                dbTotalPrice += Convert.ToDouble(ppp.Price);
                                dd.strTotalGuava = dbTotalPrice.ToString();
                                dd.strFullName = pp.ProductName + " - " + bb.Description;
                            }
                        }
                        dbTotalPrice = 0;
                    }
                    dbTotalQty = 0;
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("ERROR", "Caused by: " + ex.StackTrace);
            }
            return lstDS;
        }

        //private List<DirectSalesModel> lstFHBoxesSold()
        //{
        //    List<DirectSalesModel> lstDS = new List<DirectSalesModel>();
        //    DirectSalesModel ds = new DirectSalesModel();

        //    conDB = new ConnectionDB();
        //    string queryString = "SELECT deliverydate, clientname, totalamt, qty, remarks " +
        //        "FROM (dbfh.tbldirectsales INNER JOIN dbfh.tblproductsordered ON tbldirectsales.ID = " +
        //        "tblproductsordered.salesID) WHERE tbldirectsales.isDeleted = 0 AND tblproductsordered.isDeleted = 0 " +
        //        "AND tblproductsordered.productID =  1 AND cashbankID = ? AND deliverydate BETWEEN ? AND ? AND isPaid = ?";

        //    List<string> parameters = new List<string>();
        //    parameters.Add(cmbCashBank.SelectedValue.ToString());

        //    DateTime date = DateTime.Parse(dateFrom.Text);
        //    parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

        //    date = DateTime.Parse(dateTo.Text);
        //    parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

        //    string iPaid = (chkPaid.IsChecked.Value) ? "1" : "0";
        //    parameters.Add(iPaid);

        //    MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

        //    while (reader.Read())
        //    {
        //        DateTime dte = DateTime.Parse(reader["deliverydate"].ToString());
        //        ds.DeliveryDate = dte.ToShortDateString();
        //        ds.ClientName = reader["clientname"].ToString();
        //        ds.TotalPrice = Convert.ToInt32(reader["totalamt"].ToString());
        //        ds.Quantity = reader["qty"].ToString();
        //        ds.Remarks = reader["remarks"].ToString();
        //        lstDS.Add(ds);
        //        ds = new DirectSalesModel();
        //    }
        //    conDB.closeConnection();
        //    return lstDS;
        //}

    }
}
