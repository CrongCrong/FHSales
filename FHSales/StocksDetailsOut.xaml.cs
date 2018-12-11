using FHSales.Classes;
using FHSales.views;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FHSales
{
    /// <summary>
    /// Interaction logic for StocksDetailsOut.xaml
    /// </summary>
    public partial class StocksDetailsOut : MetroWindow
    {
        public StocksDetailsOut()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        StocksInventoryJuice stocksJuiceView;
        StocksInventoryModel stocksInventoryMod;
        string queryString = "";
        string recordID = "";
        List<string> parameters;

        public StocksDetailsOut(StocksInventoryJuice sij, StocksInventoryModel sim)
        {
            stocksJuiceView = sij;
            stocksInventoryMod = sim;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            loadProductsCombo();
            if (stocksInventoryMod != null)
            {
                recordID = stocksInventoryMod.ID;
                dateDelivered.Text = stocksInventoryMod.DeliveryDate;
                txtQty.Text = stocksInventoryMod.Qty;
                txtDRNo.Text = stocksInventoryMod.DRNo;
                txtAmount.Text = stocksInventoryMod.Amount;
                txtTerms.Text = stocksInventoryMod.Terms;
                paymentDueDate.Text = stocksInventoryMod.PaymentDueDate;

                if (stocksInventoryMod.Paid.Equals("YES"))
                {
                    chkPaid.IsChecked = true;
                }
                else
                {
                    chkPaid.IsChecked = false;
                }

                foreach (ProductModel p in cmbProducts.Items)
                {
                    if (stocksInventoryMod.ProductID.Equals(p.ID))
                    {
                        cmbProducts.SelectedItem = p;
                    }
                }
            }
        }

        private void loadProductsCombo()
        {
            conDB = new ConnectionDB();
            queryString = "SELECT ID, productname, description FROM dbfh.tblproducts WHERE isDeleted = 0";
            ProductModel product = new ProductModel();

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);
            cmbProducts.Items.Clear();
            while (reader.Read())
            {
                product.ID = reader["ID"].ToString();
                product.ProductName = reader["productname"].ToString();
                product.Description = reader["description"].ToString();
                cmbProducts.Items.Add(product);

                product = new ProductModel();
            }

            conDB.closeConnection();
        }

        private void updateRecord()
        {
            conDB = new ConnectionDB();

            queryString = "UPDATE dbfh.tblstocksjuiceout SET datedelivered = ?, productID = ?, qty = ?, " +
                "drNo = ?, isPaid = ?, terms = ?, paymentduedate = ? WHERE ID = ?";

            parameters = new List<string>();

            DateTime sdate = DateTime.Parse(dateDelivered.Text);
            parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);
            parameters.Add(cmbProducts.SelectedValue.ToString());
            parameters.Add(txtQty.Text);
            parameters.Add(txtDRNo.Text);
            string strPaid = (chkPaid.IsChecked.Value) ? "1" : "0";
            parameters.Add(strPaid);
            parameters.Add(txtTerms.Text);
            sdate = DateTime.Parse(paymentDueDate.Text);
            parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);

            parameters.Add(recordID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
        }

        private List<StocksInventoryModel> loadStocksOut()
        {
            conDB = new ConnectionDB();
            List<StocksInventoryModel> lstStocksInventory = new List<StocksInventoryModel>();
            StocksInventoryModel stocksInventory = new StocksInventoryModel();

            queryString = "SELECT dbfh.tblstocksjuiceout.ID, datedelivered, dbfh.tblstocksjuiceout.description, dbfh.tblproducts.description as productname, " +
               "qty, isPaid, drNo, amount, dbfh.tblproducts.ID as prodID, terms, paymentduedate FROM " +
               "(dbfh.tblstocksjuiceout INNER JOIN dbfh.tblproducts ON dbfh.tblstocksjuiceout.productID = dbfh.tblproducts.ID)" +
               "WHERE dbfh.tblstocksjuiceout.isDeleted = 0 AND dbfh.tblproducts.isDeleted = 0 ORDER BY datedelivered ASC";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                stocksInventory.ID = reader["ID"].ToString();

                DateTime dte = DateTime.Parse(reader["datedelivered"].ToString());
                stocksInventory.DeliveryDate = dte.ToShortDateString();

                if (string.IsNullOrEmpty(reader["paymentduedate"].ToString()))
                {
                    stocksInventory.PaymentDueDate = "";
                }
                else
                {
                    dte = DateTime.Parse(reader["paymentduedate"].ToString());
                    stocksInventory.PaymentDueDate = dte.ToShortDateString();
                }

                stocksInventory.StocksDescription = reader["description"].ToString();
                stocksInventory.ProductName = reader["productname"].ToString();
                stocksInventory.Qty = reader["qty"].ToString();
                stocksInventory.Paid = (reader["isPaid"].ToString().Equals("1") ? "YES" : "NO");
                stocksInventory.DRNo = reader["drNo"].ToString();
                stocksInventory.ProductID = reader["prodID"].ToString();
                stocksInventory.Amount = reader["amount"].ToString();
                stocksInventory.Terms = reader["terms"].ToString();

                lstStocksInventory.Add(stocksInventory);
                stocksInventory = new StocksInventoryModel();
            }
            conDB.closeConnection();

            return lstStocksInventory;
        }

        private List<StocksInventoryModel> loadTotalOut()
        {
            List<StocksInventoryModel> lstStocksOut = new List<StocksInventoryModel>();
            StocksInventoryModel sim = new StocksInventoryModel();

            conDB = new ConnectionDB();
            queryString = "SELECT dbfh.tblstocksjuiceout.productID, sum(qty) as qtyOut, tblproducts.description as productName" +
                " FROM (dbfh.tblstocksjuiceout INNER JOIN tblproducts ON dbfh.tblstocksjuiceout.productID = dbfh.tblproducts.ID) " +
                "WHERE dbfh.tblstocksjuiceout.isDeleted = 0 AND dbfh.tblproducts.isDeleted = 0 GROUP BY dbfh.tblstocksjuiceout.productID";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                sim.ProductID = reader["productID"].ToString();
                sim.QtyOut = reader["qtyOut"].ToString();
                sim.ProductName = reader["productName"].ToString();
                lstStocksOut.Add(sim);
                sim = new StocksInventoryModel();
            }

            conDB.closeConnection();
            return lstStocksOut;
        }

        private List<StocksInventoryModel> loadTotalIn()
        {
            List<StocksInventoryModel> lstStocksIn = new List<StocksInventoryModel>();
            StocksInventoryModel sim = new StocksInventoryModel();
            conDB = new ConnectionDB();
            queryString = "SELECT dbfh.tblstocksjuice.productID, sum(qty) as qty, tblproducts.description " +
                "FROM (dbfh.tblstocksjuice INNER JOIN tblproducts ON dbfh.tblstocksjuice.productID = dbfh.tblproducts.ID) " +
                "WHERE dbfh.tblstocksjuice.isDeleted = 0 GROUP BY dbfh.tblstocksjuice.productID";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                sim.ProductID = reader["productID"].ToString();
                sim.QtyIn = reader["qty"].ToString();
                sim.ProductName = reader["description"].ToString();
                lstStocksIn.Add(sim);
                sim = new StocksInventoryModel();
            }
            conDB.closeConnection();

            return lstStocksIn;
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();

            if (x)
            {
                updateRecord();
                stocksJuiceView.dgvStocksOut.ItemsSource = loadStocksOut();
                List<StocksInventoryModel> TotalInAndOut = loadTotalIn();
                List<StocksInventoryModel> TotalOut = loadTotalOut();

                foreach (StocksInventoryModel s in TotalInAndOut)
                {
                    foreach (StocksInventoryModel o in TotalOut)
                    {
                        if (s.ProductID.Equals(o.ProductID))
                        {
                            s.QtyOut = o.QtyOut;
                            s.QtyNet = (Convert.ToDouble(s.QtyIn) - Convert.ToDouble(o.QtyOut)).ToString();
                        }
                    }
                }

                //stocksJuiceView.dgvStocksInAndOut.ItemsSource = TotalInAndOut;

                await this.ShowMessageAsync("UPDATE RECORD", "Record updated successfully");
            }
        }

        private async Task<bool> checkFields()
        {
            bool ifOkay = false;

            if (string.IsNullOrEmpty(dateDelivered.Text))
            {
                await this.ShowMessageAsync("Date", "Please select date.");
            }
            else if (cmbProducts.SelectedItem == null)
            {
                await this.ShowMessageAsync("Product", "Please select products.");
            }
            else if (string.IsNullOrEmpty(txtQty.Text))
            {
                await this.ShowMessageAsync("Quantity", "Please input value.");
            }
            else if (string.IsNullOrEmpty(txtDRNo.Text))
            {
                await this.ShowMessageAsync("DR No.", "Please input value.");
            }
            else if (string.IsNullOrEmpty(txtTerms.Text))
            {
                await this.ShowMessageAsync("Terms", "Please input value.");
            }
            else if (string.IsNullOrEmpty(paymentDueDate.Text))
            {
                await this.ShowMessageAsync("Payment Due Date", "Please input value.");
            }
            else
            {
                ifOkay = true;
            }

            return ifOkay;
        }

        private void txtTerms_TextChanged(object sender, TextChangedEventArgs e)
        {
            int iDays = (string.IsNullOrEmpty(txtTerms.Text)) ? 0 : Convert.ToInt32(txtTerms.Text);

            DateTime sdate = DateTime.Parse(dateDelivered.Text);
            sdate = sdate.AddDays(iDays);

            paymentDueDate.Text = sdate.ToShortDateString();
        }

    }
}
