using FHSales.Classes;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;

namespace FHSales.views
{
    /// <summary>
    /// Interaction logic for PurchaseOrderWindow.xaml
    /// </summary>
    public partial class PurchaseOrderWindow : UserControl
    {
        public PurchaseOrderWindow()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        PurchaseOrderModel purchaseOrderModel;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgvPO.ItemsSource = loadDataGridDetails();
            btnUpdate.Visibility = Visibility.Hidden;
        }

        private List<PurchaseOrderModel> loadDataGridDetails()
        {
            conDB = new ConnectionDB();
            List<PurchaseOrderModel> lstPurchaseOrder = new List<PurchaseOrderModel>();
            PurchaseOrderModel purchase = new PurchaseOrderModel();

            string queryString = "SELECT tblpurchaseorder.ID, ponumber, sinumber, drnumber, paymentdate, deliverydate, " +
                "quantity, drugstoreID, productID FROM ((dbfh.tblpurchaseorder INNER JOIN dbfh.tbldrugstores ON tblpurchaseorder.drugstoreID = tbldrugstores.ID)" +
                " INNER JOIN dbfh.tblproducts ON dbfh.tblpurchaseorder.productID = tblproducts.ID) " +
                "WHERE dbfh.tblpurchaseorder.isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);


            while (reader.Read())
            {
                purchase.ID = reader["ID"].ToString();
                purchase.PONumber = reader["ponumber"].ToString();
                purchase.SINumber = reader["sinumber"].ToString();
                purchase.DRNumber = reader["drnumber"].ToString();
                DateTime dte = DateTime.Parse(reader["paymentdate"].ToString());
                purchase.PaymentDate = dte.ToShortDateString();
                dte = DateTime.Parse(reader["deliverydate"].ToString());
                purchase.DeliveryDate = dte.ToShortDateString();
                purchase.Quantity = reader["quantity"].ToString();
                purchase.DrugstoreID = reader["drugstoreID"].ToString();
                purchase.ProductID = reader["productID"].ToString();
                lstPurchaseOrder.Add(purchase);
                purchase = new PurchaseOrderModel();
            }

            conDB.closeConnection();

            return lstPurchaseOrder;

        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            purchaseOrderModel = dgvPO.SelectedItem as PurchaseOrderModel;

            if(purchaseOrderModel != null)
            {
                btnUpdate.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Hidden;

                txtPO.Text = purchaseOrderModel.PONumber;
                txtSI.Text = purchaseOrderModel.SINumber;
                txtDR.Text = purchaseOrderModel.DRNumber;
                paymentDate.Text = purchaseOrderModel.PaymentDate;
                deliveryDate.Text = purchaseOrderModel.DeliveryDate;
                txtQuantity.Text = purchaseOrderModel.Quantity;

                foreach (DrugstoreModel dsm in comboDrugstore.Items)
                {
                    if (dsm.ID.Equals(purchaseOrderModel.DrugstoreID))
                    {
                        comboDrugstore.SelectedItem = dsm;
                    }
                }

                foreach (ProductModel prM in comboProduct.Items)
                {
                    if (prM.ID.Equals(purchaseOrderModel.ProductID))
                    {
                        comboProduct.SelectedItem = prM;
                    }
                }

            }
        }

        private void saveRecord()
        {
            conDB = new ConnectionDB();

            string queryString = "INSERT INTO dbfh.tblpurchaseorder (ponumber, sinumber, drnumber, paymentdate, deliverydate," +
                " quantity, drugstoreID, productID, isDeleted) VALUES (?,?,?,?,?,?,?,?,0)";

            List<string> parameters = new List<string>();

            parameters.Add(txtPO.Text);
            parameters.Add(txtSI.Text);
            parameters.Add(txtDR.Text);
            DateTime date = DateTime.Parse(paymentDate.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
            date = DateTime.Parse(deliveryDate.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
            parameters.Add(txtQuantity.Text);
            parameters.Add(comboDrugstore.SelectedValue.ToString());
            parameters.Add(comboProduct.SelectedValue.ToString());

            conDB.AddRecordToDatabase(queryString, parameters);

            conDB.closeConnection();
        }

        private async Task<bool> checkFields()
        {
            bool ifAllCorrect = false;
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (string.IsNullOrEmpty(txtPO.Text))
            {
                await window.ShowMessageAsync("PO Number", "Please input PO number.");
            }
            else if (string.IsNullOrEmpty(txtSI.Text))
            {
                await window.ShowMessageAsync("SI Number", "Please provide SI number.");
            }
            else if (string.IsNullOrEmpty(txtDR.Text))
            {
                await window.ShowMessageAsync("DR Number", "Please select DR number.");
            }
            else if (string.IsNullOrEmpty(paymentDate.Text))
            {
                await window.ShowMessageAsync("Payment Date", "Please select payment date.");
            }
            else if (string.IsNullOrEmpty(deliveryDate.Text))
            {
                await window.ShowMessageAsync("Delivery Date", "Please select delivery date.");
            }
            else
            {
                ifAllCorrect = true;
            }

            return ifAllCorrect;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void checkDate_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkCategory_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkDrugstore_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkDate_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void checkCategory_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void checkDrugstore_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
