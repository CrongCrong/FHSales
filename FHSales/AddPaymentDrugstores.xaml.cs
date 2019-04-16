using FHSales.Classes;
using FHSales.MongoClasses;
using FHSales.views;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace FHSales
{
    /// <summary>
    /// Interaction logic for AddPaymentDrugstores.xaml
    /// </summary>
    public partial class AddPaymentDrugstores : MetroWindow
    {
        public AddPaymentDrugstores()
        {
            InitializeComponent();
        }

        List<PaymentsDrugstores> lstPaymentsDrg = new List<PaymentsDrugstores>();
        List<ProductsOrdered> lstProductsOrderd = new List<ProductsOrdered>();

        FHDrugstores fhDrugstores;
        ConnectionDB conDB;

        public AddPaymentDrugstores(FHDrugstores fhD, List<PaymentsDrugstores> lstPayments, List<ProductsOrdered> lstProdOrd)
        {
            fhDrugstores = fhD;
            lstProductsOrderd = lstProdOrd;
            lstPaymentsDrg = lstPayments;
            InitializeComponent();
        }


        private void MetroWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (lstPaymentsDrg.Count > 0)
            {
                dgvPayments.ItemsSource = lstPaymentsDrg;
            }
            loadPaymentModeOnCombo();
        }


        private void TxtPayment_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {

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


        private void BtnAdd_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            PaymentsDrugstores payment = new PaymentsDrugstores();
            Banks bb = cmbCashBank.SelectedItem as Banks;

            DateTime dte = DateTime.Parse(datePayment.Text);
            payment.PaymentDate = DateTime.Parse(dte.ToLocalTime().ToShortDateString());
            payment.Payment = Convert.ToDouble(txtPayment.Text);
            payment.strDate = payment.PaymentDate.ToShortDateString();
            payment.Notes = txtNotes.Text;
            payment.PaymentMode = bb;
            lstPaymentsDrg.Add(payment);

            dgvPayments.Items.Refresh();
            dgvPayments.ItemsSource = lstPaymentsDrg;
            payment = new PaymentsDrugstores();
        }

        private void BtnRemove_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            PaymentsDrugstores prodMod = dgvPayments.SelectedItem as PaymentsDrugstores;

            if (prodMod != null)
            {
                lstPaymentsDrg.Remove(prodMod);
                dgvPayments.ItemsSource = lstPaymentsDrg;
                dgvPayments.Items.Refresh();
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            double x = 0;

            foreach (ProductsOrdered po in lstProductsOrderd)
            {
                x += po.Total;
            }

            fhDrugstores.lstProductsOrdered = lstProductsOrderd;
            lstProductsOrderd = new List<ProductsOrdered>();

            if (lstPaymentsDrg != null && lstPaymentsDrg.Count > 0)
            {
                fhDrugstores.lstPayments = lstPaymentsDrg;
                double dblTotal = x;
                double dblPaym = 0;
                foreach (PaymentsDrugstores fds in lstPaymentsDrg)
                {
                    dblPaym += fds.Payment;

                }
                dblTotal -= dblPaym;
                fhDrugstores.txtTotal.Text = dblTotal.ToString();
                lstPaymentsDrg = new List<PaymentsDrugstores>();
            }


        }
    }
}
