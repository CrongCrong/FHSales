using FHSales.Classes;
using MahApps.Metro.Controls;
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

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            ReportForm rf = new ReportForm(lstPurchaseOrderReports());
            rf.ShowDialog();
        }

        private List<PurchaseOrderModel> lstPurchaseOrderReports()
        {
            conDB = new ConnectionDB();
            List<PurchaseOrderModel> lstPurchaseOrderModel = new List<PurchaseOrderModel>();
            PurchaseOrderModel pom = new PurchaseOrderModel();
            string queryString = "SELECT dbfh.tblpurchaseorder.ID, ponumber, sinumber, drnumber, paymentdate, " +
                "deliverydate, quantity, drugstoreID, productID, dbfh.tblproducts.productname, isPaid, amount," +
                " dbfh.tbldrugstores.description, dbfh.tbldrugstores.paymentdue AS 'terms', " +
                "dbfh.tblpurchaseorder.paymentduedate FROM ((dbfh.tblpurchaseorder INNER JOIN " +
                "dbfh.tbldrugstores ON tblpurchaseorder.drugstoreID = tbldrugstores.ID) INNER JOIN " +
                "dbfh.tblproducts ON dbfh.tblpurchaseorder.productID = tblproducts.ID) " +
                "WHERE dbfh.tblpurchaseorder.isDeleted = 0"; //order by deliverydate ASC";
            List<string> parameters = new List<string>();

            if (radioDeliveryDate.IsChecked.Value)
            {
                queryString += " AND (deliverydate BETWEEN ? AND ?)";

                if (chkAddPaid.IsChecked.Value == false)
                {
                   queryString += " AND (isPaid = 0)";
                }            
            }
            else if (radioPaymentDate.IsChecked.Value)
            {
                queryString += " AND (paymentduedate BETWEEN ? AND ?)";
            }

            queryString += " order by deliverydate ASC";

            DateTime sdate = DateTime.Parse(dateFrom.Text);
            parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);
            sdate = DateTime.Parse(dateTo.Text);
            parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);
            string temp = "";
            while (reader.Read())
            {
                pom.ID = reader["ID"].ToString();
                pom.DrugstoreName = reader["description"].ToString();
                pom.Quantity = reader["quantity"].ToString();
                pom.ProductName = reader["productname"].ToString();
                pom.Amount = reader["amount"].ToString();
                pom.SINumber = reader["sinumber"].ToString();
                pom.isPaid = reader["isPaid"].ToString();

                temp = reader["deliverydate"].ToString();

                if (pom.isPaid.Equals("1"))
                {
                    pom.boolPaid = true;
                }else
                {
                    pom.boolPaid = false;
                }

                DateTime d1;
                if (!string.IsNullOrEmpty(temp) && !temp.Equals("0000-00-00"))
                {
                    d1 = DateTime.Parse(temp);
                    pom.DeliveryDate = d1.ToShortDateString();
                }
                else
                {
                    pom.DeliveryDate = "";
                }

                pom.Terms = reader["terms"].ToString();
                temp = reader["paymentduedate"].ToString();
                if (!string.IsNullOrEmpty(temp) && !temp.Equals("0000-00-00"))
                {
                    d1 = DateTime.Parse(temp);
                    pom.PaymentDueDate = d1.ToShortDateString();

                    //CHECK IF DATE IS BEYOND DUE DATE
                    //DateTime dtePaymentDue = DateTime.Parse(pom.PaymentDueDate);
                    //DateTime dteNow = DateTime.Now;

                    //if (dteNow.Date >= dtePaymentDue)
                    //{
                    //    pom.boolPaid = true;
                    //}
                    //else
                    //{
                    //    pom.boolPaid = false;
                    //}
                }
                else
                {
                    pom.PaymentDueDate = "";
                }
                temp = reader["paymentdate"].ToString();
                if (!string.IsNullOrEmpty(temp) && !temp.Equals("0000-00-00"))
                {
                    d1 = DateTime.Parse(temp);
                    pom.PaymentDate = d1.ToShortDateString();
                }else
                {
                    pom.PaymentDate = "";
                }

                pom.PONumber = reader["ponumber"].ToString();
                lstPurchaseOrderModel.Add(pom);
                pom = new PurchaseOrderModel();
            }

            conDB.closeConnection();

            return lstPurchaseOrderModel;
        }

    }
}
