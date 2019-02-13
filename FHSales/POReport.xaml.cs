using FHSales.Classes;
using FHSales.MongoClasses;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MongoDB.Driver;
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
using System.Windows.Shapes;

namespace FHSales
{
    /// <summary>
    /// Interaction logic for POReport.xaml
    /// </summary>
    public partial class POReport : MetroWindow
    {
        public POReport()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            chkAddPaid.IsChecked = false;
            chkAddPaid.Visibility = Visibility.Hidden;
        }

        private void radioDeliveryDate_Checked(object sender, RoutedEventArgs e)
        {
            radioPaymentDate.IsChecked = false;
            chkAddPaid.Visibility = Visibility.Visible;
        }

        private void radioPaymentDate_Checked(object sender, RoutedEventArgs e)
        {
            radioDeliveryDate.IsChecked = false;
            chkAddPaid.IsChecked = false;
            chkAddPaid.Visibility = Visibility.Hidden;
        }

        private async void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            //List<PurchaseOrders> lstA = await getPurchaseOrderReports();
            ReportForm rf = new ReportForm(await getPurchaseOrderReports());
            rf.ShowDialog();
        }


        private async Task<List<PurchaseOrders>> getPurchaseOrderReports()
        {
            List<PurchaseOrders> lstPurchaseOrders = new List<PurchaseOrders>();
            DateTime dteNow = (!string.IsNullOrEmpty(dateFrom.Text)) ? DateTime.Parse(dateFrom.Text) : DateTime.Now;
            DateTime dteFirstDay = (!string.IsNullOrEmpty(dateTo.Text)) ? DateTime.Parse(dateTo.Text) : DateTime.Now;
            try
            {
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                var collection = db.GetCollection<PurchaseOrders>("PurchaseOrders");

                var filter = Builders<PurchaseOrders>.Filter.And(
        Builders<PurchaseOrders>.Filter.Where(p => p.isDeleted == false));

                if (radioDeliveryDate.IsChecked.Value)
                {
                    filter = Builders<PurchaseOrders>.Filter.And(
          Builders<PurchaseOrders>.Filter.Gte("DeliveryDate", dteNow),
         Builders<PurchaseOrders>.Filter.Lte("DeliveryDate", dteFirstDay));
                }

                else if (radioPaymentDate.IsChecked.Value)
                {
                    filter = Builders<PurchaseOrders>.Filter.And(
          Builders<PurchaseOrders>.Filter.Gte("PaymentDueDate", dteNow),
         Builders<PurchaseOrders>.Filter.Lte("PaymentDueDate", dteFirstDay));
                }

                if (chkAddPaid.IsChecked.Value)
                {
                    filter = filter & Builders<PurchaseOrders>.Filter.Or(
          Builders<PurchaseOrders>.Filter.Eq("isPaid", false),
          Builders<PurchaseOrders>.Filter.Eq("isPaid", true));
                }
                else
                {
                    filter = filter & Builders<PurchaseOrders>.Filter.And(
          Builders<PurchaseOrders>.Filter.Eq("isPaid", false));
                }

                lstPurchaseOrders = collection.Find(filter).ToList();
                lstPurchaseOrders = lstPurchaseOrders.OrderByDescending(a => a.DeliveryDate).ToList();

                foreach (PurchaseOrders pp in lstPurchaseOrders)
                {
                    pp.Terms = pp.Drugstore.Terms;
                    pp.DrugstoreName = pp.Drugstore.DrugstoreName;
                    pp.ProductName = pp.Product.ProductName;
                    pp.strDeliveryDate = pp.DeliveryDate.ToShortDateString();
                    pp.strPaymentDueDate = pp.PaymentDueDate.ToShortDateString();
                    pp.strPaymentDate = pp.PaymentDate.ToShortDateString();
                    pp.strAmount = pp.Amount.ToString("0.##");
                }

            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("ERROR", "Caused by: " + ex.StackTrace);
            }

            return lstPurchaseOrders;
        }


        //private List<PurchaseOrderModel> lstPurchaseOrderReports()
        //{
        //    conDB = new ConnectionDB();
        //    List<PurchaseOrderModel> lstPurchaseOrderModel = new List<PurchaseOrderModel>();
        //    PurchaseOrderModel pom = new PurchaseOrderModel();
        //    string queryString = "SELECT dbfh.tblpurchaseorder.ID, ponumber, sinumber, drnumber, paymentdate, " +
        //        "deliverydate, quantity, drugstoreID, productID, dbfh.tblproducts.productname, isPaid, amount," +
        //        " dbfh.tbldrugstores.description, dbfh.tbldrugstores.paymentdue AS 'terms', " +
        //        "dbfh.tblpurchaseorder.paymentduedate FROM ((dbfh.tblpurchaseorder INNER JOIN " +
        //        "dbfh.tbldrugstores ON tblpurchaseorder.drugstoreID = tbldrugstores.ID) INNER JOIN " +
        //        "dbfh.tblproducts ON dbfh.tblpurchaseorder.productID = tblproducts.ID) " +
        //        "WHERE dbfh.tblpurchaseorder.isDeleted = 0"; //order by deliverydate ASC";
        //    List<string> parameters = new List<string>();

        //    if (radioDeliveryDate.IsChecked.Value)
        //    {
        //        queryString += " AND (deliverydate BETWEEN ? AND ?)";

        //        if (chkAddPaid.IsChecked.Value == false)
        //        {
        //            queryString += " AND (isPaid = 0)";
        //        }
        //    }
        //    else if (radioPaymentDate.IsChecked.Value)
        //    {
        //        queryString += " AND (paymentduedate BETWEEN ? AND ?)";
        //    }

        //    queryString += " order by deliverydate ASC";

        //    DateTime sdate = DateTime.Parse(dateFrom.Text);
        //    parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);
        //    sdate = DateTime.Parse(dateTo.Text);
        //    parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);

        //    MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);
        //    string temp = "";
        //    while (reader.Read())
        //    {
        //        pom.ID = reader["ID"].ToString();
        //        pom.DrugstoreName = reader["description"].ToString();
        //        pom.Quantity = reader["quantity"].ToString();
        //        pom.ProductName = reader["productname"].ToString();
        //        pom.Amount = reader["amount"].ToString();
        //        pom.SINumber = reader["sinumber"].ToString();
        //        pom.isPaid = reader["isPaid"].ToString();

        //        temp = reader["deliverydate"].ToString();

        //        if (pom.isPaid.Equals("1"))
        //        {
        //            pom.boolPaid = true;
        //        }
        //        else
        //        {
        //            pom.boolPaid = false;
        //        }

        //        DateTime d1;
        //        if (!string.IsNullOrEmpty(temp) && !temp.Equals("0000-00-00"))
        //        {
        //            d1 = DateTime.Parse(temp);
        //            pom.DeliveryDate = d1.ToShortDateString();
        //        }
        //        else
        //        {
        //            pom.DeliveryDate = "";
        //        }

        //        pom.Terms = reader["terms"].ToString();
        //        temp = reader["paymentduedate"].ToString();
        //        if (!string.IsNullOrEmpty(temp) && !temp.Equals("0000-00-00"))
        //        {
        //            d1 = DateTime.Parse(temp);
        //            pom.PaymentDueDate = d1.ToShortDateString();

        //            //CHECK IF DATE IS BEYOND DUE DATE
        //            //DateTime dtePaymentDue = DateTime.Parse(pom.PaymentDueDate);
        //            //DateTime dteNow = DateTime.Now;

        //            //if (dteNow.Date >= dtePaymentDue)
        //            //{
        //            //    pom.boolPaid = true;
        //            //}
        //            //else
        //            //{
        //            //    pom.boolPaid = false;
        //            //}
        //        }
        //        else
        //        {
        //            pom.PaymentDueDate = "";
        //        }
        //        temp = reader["paymentdate"].ToString();
        //        if (!string.IsNullOrEmpty(temp) && !temp.Equals("0000-00-00"))
        //        {
        //            d1 = DateTime.Parse(temp);
        //            pom.PaymentDate = d1.ToShortDateString();
        //        }
        //        else
        //        {
        //            pom.PaymentDate = "";
        //        }

        //        pom.PONumber = reader["ponumber"].ToString();
        //        lstPurchaseOrderModel.Add(pom);
        //        pom = new PurchaseOrderModel();
        //    }

        //    conDB.closeConnection();

        //    return lstPurchaseOrderModel;
        //}

    }
}
