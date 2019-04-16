using FHSales.Classes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace FHSales.views
{
    /// <summary>
    /// Interaction logic for StocksInventoryJuice.xaml
    /// </summary>
    public partial class StocksInventoryJuice : UserControl
    {
        public StocksInventoryJuice()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        List<string> parameters;
        string queryString = "";
        MetroWindow window;
        List<FHJuiceProductModel> lstFHProducts;
        List<FHJuiceProductModel> lstProductsStocksIn;
        List<FHJuiceProductModel> lstProductsStocksOut;


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgvStocks.ItemsSource = loadDatagridDetails();
            dgvAccounts.ItemsSource = loadDREncoded();
            dgvConsolidatedDR.ItemsSource = loadConsolidatedDRS();
            lstProductsStocksIn = loadProductsStocksUpperwarehouse();
            lstProductsStocksOut = loadProductsStocksLowerWarehouse();

            List<StocksInventoryModel> TotalInAndOut = loadTotalIn();
            List<StocksInventoryModel> TotalOut = loadTotalOut();

            double dblQtyIn = 0;
            double dblQtyOut = 0;

            foreach (FHJuiceProductModel j in lstProductsStocksIn)
            {
                foreach (FHJuiceProductModel k in lstProductsStocksOut)
                {
                    if (j.ID.Equals(k.ID))
                    {
                        dblQtyIn = Convert.ToDouble(j.Qty);
                        dblQtyOut = Convert.ToDouble(k.Qty);
                        dblQtyIn = dblQtyIn - dblQtyOut;
                        j.Qty = dblQtyIn.ToString("N0");
                    }
                }
            }

            dgvStocks.ItemsSource = lstProductsStocksIn;
            dgvStocksOut.ItemsSource = lstProductsStocksOut;
            //foreach (StocksInventoryModel s in TotalInAndOut)
            //{
            //    foreach (StocksInventoryModel o in TotalOut)
            //    {
            //        if (s.ProductID.Equals(o.ProductID))
            //        {
            //            s.QtyOut = o.QtyOut;
            //            s.QtyNet = (Convert.ToDouble(s.QtyIn) - Convert.ToDouble(o.QtyOut)).ToString();
            //        }
            //    }
            //}

            //dgvStocksInAndOut.ItemsSource = TotalInAndOut;
            //dgvStocksOut.ItemsSource = loadStocksOut();

        }

        private List<StocksInventoryModel> loadDatagridDetails()
        {
            conDB = new ConnectionDB();
            List<StocksInventoryModel> lstStocksInventory = new List<StocksInventoryModel>();
            StocksInventoryModel stocksInventory = new StocksInventoryModel();

            queryString = "SELECT dbfh.tblstocksjuice.ID, datedelivered, dbfh.tblproducts.description as productname, " +
                "qty, dbfh.tblproducts.ID as prodID FROM (dbfh.tblstocksjuice INNER JOIN dbfh.tblproducts ON dbfh.tblstocksjuice.productID = dbfh.tblproducts.ID)" +
                "WHERE dbfh.tblstocksjuice.isDeleted = 0 AND dbfh.tblproducts.isDeleted = 0 ORDER BY datedelivered ASC";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                stocksInventory.ID = reader["ID"].ToString();

                DateTime dte = DateTime.Parse(reader["datedelivered"].ToString());
                stocksInventory.DeliveryDate = dte.ToShortDateString();
                stocksInventory.ProductID = reader["prodID"].ToString();
                stocksInventory.ProductName = reader["productname"].ToString();
                stocksInventory.Qty = reader["qty"].ToString();

                lstStocksInventory.Add(stocksInventory);
                stocksInventory = new StocksInventoryModel();
            }
            conDB.closeConnection();

            return lstStocksInventory;
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
                " FROM(dbfh.tblstocksjuiceout INNER JOIN tblproducts ON dbfh.tblstocksjuiceout.productID = dbfh.tblproducts.ID) " +
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

        private void deleteRecord(string strID)
        {
            conDB = new ConnectionDB();
            queryString = "UPDATE dbfh.tblstocksjuice SET isDeleted = 1 WHERE ID = ?";
            parameters = new List<string>();
            parameters.Add(strID);
            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
        }

        private void deleteRecordStocksOUt(string strID)
        {
            conDB = new ConnectionDB();
            queryString = "UPDATE dbfh.tblstocksjuiceout SET isDeleted = 1 WHERE ID = ?";
            parameters = new List<string>();
            parameters.Add(strID);
            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            StocksInventoryModel sim = dgvStocksOut.SelectedItem as StocksInventoryModel;

            if (sim != null)
            {
                StocksDetailsOut sdo = new StocksDetailsOut(this, sim);
                sdo.ShowDialog();
            }
        }

        private void MenuEditStocksIn_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void MenuDeleteStocksIn_Click(object sender, RoutedEventArgs e)
        {
            StocksInventoryModel si = dgvStocks.SelectedItem as StocksInventoryModel;

            if (si != null)
            {
                MessageDialogResult result = await window.ShowMessageAsync("Delete Stocks In",
                "Are you sure you want to delete stocks record?", MessageDialogStyle.AffirmativeAndNegative);

                if (result.Equals(MessageDialogResult.Affirmative))
                {
                    deleteRecord(si.ID);
                    dgvStocks.ItemsSource = loadDatagridDetails();
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

                    // dgvStocksInAndOut.ItemsSource = TotalInAndOut;
                }
            }

        }

        private void MenuEditStocksOut_Click(object sender, RoutedEventArgs e)
        {
            StocksInventoryModel sim = dgvStocksOut.SelectedItem as StocksInventoryModel;

            if (sim != null)
            {
                StocksDetailsOut sdo = new StocksDetailsOut(this, sim);
                sdo.ShowDialog();
            }
        }

        private async void MenuDeleteStocksOut_Click(object sender, RoutedEventArgs e)
        {
            StocksInventoryModel sim = dgvStocksOut.SelectedItem as StocksInventoryModel;
            if (sim != null)
            {
                MessageDialogResult result = await window.ShowMessageAsync("Delete Stocks In",
                "Are you sure you want to delete stocks record?", MessageDialogStyle.AffirmativeAndNegative);

                if (result.Equals(MessageDialogResult.Affirmative))
                {
                    deleteRecordStocksOUt(sim.ID);
                    dgvStocksOut.ItemsSource = loadStocksOut();
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

                    //dgvStocksInAndOut.ItemsSource = TotalInAndOut;
                }

            }
        }

        private void MenuAddStocks_Click(object sender, RoutedEventArgs e)
        {
            StocksDetailsIn stocksIn = new StocksDetailsIn(this);
            stocksIn.ShowDialog();
        }

        private void MenuAddStocksOut_Click(object sender, RoutedEventArgs e)
        {
            StocksDetailsIn stocksIn = new StocksDetailsIn(this, TRAN_TYPE.STOCKS_OUT_ADD);
            stocksIn.ShowDialog();
        }

        private List<AccountOrderModel> loadDREncoded()
        {
            conDB = new ConnectionDB();
            queryString = "SELECT dbjuiceacct.tblaccountorder.ID, drNo, clientID, dbjuiceacct.tblaccounts.accountname, " +
                "deliverydate, terms, paymentduedate, total, repID, paymentmodeID, isPaid FROM (dbjuiceacct.tblaccountorder INNER JOIN " +
                "dbjuiceacct.tblaccounts ON dbjuiceacct.tblaccountorder.clientID = dbjuiceacct.tblaccounts.ID) " +
                "WHERE dbjuiceacct.tblaccountorder.isDeleted = 0 AND dbjuiceacct.tblaccounts.isDeleted = 0 AND dbjuiceacct.tblaccountorder.isConso = 0";

            List<AccountOrderModel> lstDRs = new List<AccountOrderModel>();

            AccountOrderModel drs = new AccountOrderModel();

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                drs.ID = reader["ID"].ToString();
                drs.DrNo = reader["drNo"].ToString();
                drs.ClientName = reader["accountname"].ToString();
                drs.ClientID = reader["clientID"].ToString();
                DateTime pdate = DateTime.Parse(reader["deliverydate"].ToString());
                drs.DeliveryDate = pdate.ToShortDateString();

                pdate = DateTime.Parse(reader["paymentduedate"].ToString());
                drs.PaymentDueDate = pdate.ToShortDateString();
                drs.Terms = reader["terms"].ToString();
                drs.Total = reader["total"].ToString();
                drs.RepID = reader["repID"].ToString();
                drs.isPaid = reader["isPaid"].ToString();
                drs.PaymentModeID = reader["paymentmodeID"].ToString();

                lstDRs.Add(drs);
                drs = new AccountOrderModel();
            }

            conDB.closeConnection();

            return lstDRs;
        }

        private List<FHJuiceProductModel> getProductsOrdered(string strOrderID)
        {
            List<FHJuiceProductModel> lstProds = new List<FHJuiceProductModel>();

            queryString = "SELECT dbjuiceacct.tblproductsordered.ID, orderID, productID, qty, " +
                "price, productname, remarks FROM (dbjuiceacct.tblproductsordered INNER JOIN " +
                "dbjuiceacct.tblproducts ON tblproductsordered.productID = dbjuiceacct.tblproducts.ID) " +
                "WHERE dbjuiceacct.tblproductsordered.isDeleted = 0 AND orderID = ?";

            FHJuiceProductModel pMdl = new FHJuiceProductModel();
            parameters = new List<string>();
            parameters.Add(strOrderID);
            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {

                pMdl.RecordID = reader["ID"].ToString();
                pMdl.OrderID = reader["orderID"].ToString();
                pMdl.ID = reader["productID"].ToString();
                pMdl.Qty = reader["qty"].ToString();
                pMdl.Price = reader["price"].ToString();
                pMdl.Total = (Convert.ToDouble(pMdl.Qty) * Convert.ToDouble(pMdl.Price)).ToString();
                pMdl.Remarks = reader["remarks"].ToString();
                pMdl.ProductName = reader["productname"].ToString();
                lstProds.Add(pMdl);
                pMdl = new FHJuiceProductModel();
            }
            conDB.closeConnection();
            return lstProds;
        }

        private List<FHJuiceProductModel> getProductsOrderedConso(string strOrderID)
        {
            List<FHJuiceProductModel> lstProds = new List<FHJuiceProductModel>();

            queryString = "SELECT dbjuiceacct.tblproductsorderedconso.ID, orderID, productID, qty, " +
                "price, productname, remarks FROM (dbjuiceacct.tblproductsorderedconso INNER JOIN " +
                "dbjuiceacct.tblproducts ON tblproductsorderedconso.productID = dbjuiceacct.tblproducts.ID) " +
                "WHERE dbjuiceacct.tblproductsorderedconso.isDeleted = 0 AND orderID = ?";

            FHJuiceProductModel pMdl = new FHJuiceProductModel();
            parameters = new List<string>();
            parameters.Add(strOrderID);
            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {

                pMdl.RecordID = reader["ID"].ToString();
                pMdl.OrderID = reader["orderID"].ToString();
                pMdl.ID = reader["productID"].ToString();
                pMdl.Qty = reader["qty"].ToString();
                pMdl.Price = reader["price"].ToString();
                pMdl.Total = (Convert.ToDouble(pMdl.Qty) * Convert.ToDouble(pMdl.Price)).ToString();
                pMdl.Remarks = reader["remarks"].ToString();
                pMdl.ProductName = reader["productname"].ToString();
                lstProds.Add(pMdl);
                pMdl = new FHJuiceProductModel();
            }
            conDB.closeConnection();
            return lstProds;
        }

        private List<AccountOrderModel> loadConsolidatedDRS()
        {
            conDB = new ConnectionDB();
            queryString = "SELECT dbjuiceacct.tblaccountorderconso.ID, drNo, clientID, dbjuiceacct.tblaccounts.accountname, " +
                "deliverydate, terms, paymentduedate, total, repID, paymentmodeID, isPaid FROM (dbjuiceacct.tblaccountorderconso INNER JOIN " +
                "dbjuiceacct.tblaccounts ON dbjuiceacct.tblaccountorderconso.clientID = dbjuiceacct.tblaccounts.ID) " +
                "WHERE dbjuiceacct.tblaccountorderconso.isDeleted = 0 AND dbjuiceacct.tblaccounts.isDeleted = 0";

            List<AccountOrderModel> lstDRs = new List<AccountOrderModel>();

            AccountOrderModel drs = new AccountOrderModel();

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                drs.ID = reader["ID"].ToString();
                drs.DrNo = reader["drNo"].ToString();
                drs.ClientName = reader["accountname"].ToString();
                drs.ClientID = reader["clientID"].ToString();
                DateTime pdate = DateTime.Parse(reader["deliverydate"].ToString());
                drs.DeliveryDate = pdate.ToShortDateString();

                pdate = DateTime.Parse(reader["paymentduedate"].ToString());
                drs.PaymentDueDate = pdate.ToShortDateString();
                drs.Terms = reader["terms"].ToString();
                drs.Total = reader["total"].ToString();
                drs.RepID = reader["repID"].ToString();
                drs.isPaid = reader["isPaid"].ToString();
                drs.PaymentModeID = reader["paymentmodeID"].ToString();

                lstDRs.Add(drs);
                drs = new AccountOrderModel();
            }

            conDB.closeConnection();

            return lstDRs;
        }

        private List<FHJuiceProductModel> loadProductsStocksUpperwarehouse()
        {
            conDB = new ConnectionDB();
            List<FHJuiceProductModel> lstJuiceIn = new List<FHJuiceProductModel>();
            FHJuiceProductModel juiceIn = new FHJuiceProductModel();

            queryString = "SELECT dbjuiceacct.tblupperwarehouse.ID, productID, productname, description, " +
                "stockdate, sum(qty) as qty FROM(dbjuiceacct.tblupperwarehouse INNER JOIN dbjuiceacct.tblproducts ON " +
                "dbjuiceacct.tblupperwarehouse.productID = dbjuiceacct.tblproducts.ID) WHERE " +
                "dbjuiceacct.tblupperwarehouse.isDeleted = 0 AND dbjuiceacct.tblproducts.isDeleted = 0 GROUP BY productID";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                juiceIn.RecordID = reader["ID"].ToString();
                juiceIn.ID = reader["productID"].ToString();
                juiceIn.ProductName = reader["productname"].ToString();
                DateTime pdate = DateTime.Parse(reader["stockdate"].ToString());
                juiceIn.StockDate = pdate.ToShortDateString();
                juiceIn.Qty = reader["qty"].ToString();

                lstJuiceIn.Add(juiceIn);
                juiceIn = new FHJuiceProductModel();
            }
            conDB.closeConnection();
            return lstJuiceIn;
        }

        private List<FHJuiceProductModel> loadProductsStocksLowerWarehouse()
        {
            conDB = new ConnectionDB();
            List<FHJuiceProductModel> lstJuiceIn = new List<FHJuiceProductModel>();
            FHJuiceProductModel juiceIn = new FHJuiceProductModel();

            queryString = "SELECT dbjuiceacct.tbllowerwarehouse.ID, productID, productname, description, " +
                "stockdate, sum(qty) as qty FROM(dbjuiceacct.tbllowerwarehouse INNER JOIN dbjuiceacct.tblproducts ON " +
                "dbjuiceacct.tbllowerwarehouse.productID = dbjuiceacct.tblproducts.ID) WHERE " +
                "dbjuiceacct.tbllowerwarehouse.isDeleted = 0 AND dbjuiceacct.tblproducts.isDeleted = 0 GROUP BY productID";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                juiceIn.RecordID = reader["ID"].ToString();
                juiceIn.ID = reader["productID"].ToString();
                juiceIn.ProductName = reader["productname"].ToString();
                DateTime pdate = DateTime.Parse(reader["stockdate"].ToString());
                juiceIn.StockDate = pdate.ToShortDateString();
                juiceIn.Qty = reader["qty"].ToString();

                lstJuiceIn.Add(juiceIn);
                juiceIn = new FHJuiceProductModel();
            }
            conDB.closeConnection();
            return lstJuiceIn;
        }


        private void MenuViewDR_Click(object sender, RoutedEventArgs e)
        {
            AccountOrderModel am = dgvAccounts.SelectedItem as AccountOrderModel;
            if (am != null)
            {
                lstFHProducts = getProductsOrdered(am.ID);
                ViewDRJuice viewDR = new ViewDRJuice(this, am, lstFHProducts);
                viewDR.ShowDialog();
            }

        }

        private void MenuViewDRConso_Click(object sender, RoutedEventArgs e)
        {
            AccountOrderModel am = dgvConsolidatedDR.SelectedItem as AccountOrderModel;
            if (am != null)
            {
                lstFHProducts = getProductsOrderedConso(am.ID);
                ViewDRJuice viewDR = new ViewDRJuice(this, am, lstFHProducts, true);
                viewDR.ShowDialog();
            }

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            GenerateReportView grv = new GenerateReportView();
            grv.ShowDialog();
        }
    }
}
