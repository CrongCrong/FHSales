using FHSales.Classes;
using FHSales.views;
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
    /// Interaction logic for StocksDetailsIn.xaml
    /// </summary>
    public partial class StocksDetailsIn : MetroWindow
    {


        public StocksDetailsIn()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        StocksInventoryJuice stocksJuiceView;
        StocksInventoryModel stocksInventoryMod;
        string queryString = "";
        string recordID = "";
        List<string> parameters;
        TRAN_TYPE strTranType;


        public StocksDetailsIn(StocksInventoryJuice sij)
        {
            stocksJuiceView = sij;
            InitializeComponent();
        }

        public StocksDetailsIn(StocksInventoryJuice sij, TRAN_TYPE t)
        {
            strTranType = t;
            stocksJuiceView = sij;
            InitializeComponent();
        }



        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            loadProductsOnCombo();
            btnUpdate.Visibility = Visibility.Hidden;

        }

        private void loadProductsOnCombo()
        {
            conDB = new ConnectionDB();
            FHJuiceProductModel prod = new FHJuiceProductModel();
            queryString = "SELECT ID, productname, description FROM dbjuiceacct.tblproducts WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                prod.ID = reader["ID"].ToString();
                prod.ProductName = reader["productname"].ToString();
                prod.Description = reader["description"].ToString();

                cmbProducts.Items.Add(prod);
                prod = new FHJuiceProductModel();
            }

            conDB.closeConnection();
        }



        private void saveProductToStocks()
        {
            conDB = new ConnectionDB();
            queryString = "INSERT INTO dbjuiceacct.tblupperwarehouse (productID, stockdate, qty, isDeleted) VALUES (?,?,?,0)";

            parameters = new List<string>();
            parameters.Add(cmbProducts.SelectedValue.ToString());
            DateTime pdate = DateTime.Parse(stockDate.Text);
            parameters.Add(pdate.Year + "/" + pdate.Month + "/" + pdate.Day);
            parameters.Add(txtQty.Text);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
        }

        private void saveProductsToStocksOut()
        {
            conDB = new ConnectionDB();
            queryString = "INSERT INTO dbjuiceacct.tbllowerwarehouse (productID, stockdate, qty, isDeleted) VALUES (?,?,?,0)";
            parameters = new List<string>();

            parameters.Add(cmbProducts.SelectedValue.ToString());
            DateTime pdate = DateTime.Parse(stockDate.Text);
            parameters.Add(pdate.Year + "/" + pdate.Month + "/" + pdate.Day);
            parameters.Add(txtQty.Text);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();

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

        private async Task<bool> checkFields()
        {
            bool ifOkay = false;

            if (string.IsNullOrEmpty(stockDate.Text))
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
            else
            {
                ifOkay = true;
            }

            return ifOkay;
        }


        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();

            if (x)
            {
                List<FHJuiceProductModel> lstProductsStocksIn;
                List<FHJuiceProductModel> lstProductsStocksOut;

                double dblQtyIn = 0;
                double dblQtyOut = 0;

                if (strTranType.Equals(TRAN_TYPE.STOCKS_OUT_ADD))
                {
                    saveProductsToStocksOut();
                    lstProductsStocksOut = loadProductsStocksLowerWarehouse();
                    lstProductsStocksIn = loadProductsStocksUpperwarehouse();
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
                    stocksJuiceView.dgvStocks.ItemsSource = lstProductsStocksIn;
                    stocksJuiceView.dgvStocksOut.ItemsSource = lstProductsStocksOut;
                    await this.ShowMessageAsync("SAVE RECORD", "Record saved successfully!");
                    this.Close();
                }
                else
                {
                    saveProductToStocks();
                    lstProductsStocksOut = loadProductsStocksLowerWarehouse();
                    lstProductsStocksIn = loadProductsStocksUpperwarehouse();
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
                    stocksJuiceView.dgvStocks.ItemsSource = lstProductsStocksIn;
                    stocksJuiceView.dgvStocksOut.ItemsSource = lstProductsStocksOut;
                    await this.ShowMessageAsync("SAVE RECORD", "Record saved successfully!");
                    this.Close();
                }
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {

        }

        
    }
}
