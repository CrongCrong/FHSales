using FHSales.Classes;
using FHSales.MongoClasses;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FHSales
{
    /// <summary>
    /// Interaction logic for Drugstores_Report.xaml
    /// </summary>
    public partial class Drugstores_Report : MetroWindow
    {

        public Drugstores_Report()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            loadAreaOnCombo();
        }

        private async void loadAreaOnCombo()
        {
            try
            {
                cmbArea.Items.Clear();
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                var collection = db.GetCollection<Area>("Area");
                var filter = Builders<Area>.Filter.And(
        Builders<Area>.Filter.Where(p => p.isDeleted == false));
                List<Area> lstPayments = collection.Find(filter).ToList();
                foreach (Area p in lstPayments)
                {
                    cmbArea.Items.Add(p);
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("EXCEPTION", "ERROR LOADING DATA " + ex.Message);
            }
        }



        private async Task<List<DrugstoresSales>> GenerateReport()
        {
            List<DrugstoresSales> lstDrgSales = new List<DrugstoresSales>();
            DateTime dteNow = DateTime.Parse(dateFrom.Text);
            DateTime dteFirstDay = DateTime.Parse(dateTo.Text);
            Area dd = cmbArea.SelectedItem as Area;
            try
            {
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                var collection = db.GetCollection<DrugstoresSales>("DrugstoresSales");

                var filter = Builders<DrugstoresSales>.Filter.And(
        Builders<DrugstoresSales>.Filter.Where(p => p.isDeleted == false));

                if ((!string.IsNullOrEmpty(dateFrom.Text)) && (!string.IsNullOrEmpty(dateTo.Text)))
                {
                    filter = filter & Builders<DrugstoresSales>.Filter.And(
          Builders<DrugstoresSales>.Filter.Gte("DeliveryDate", dteNow),
         Builders<DrugstoresSales>.Filter.Lte("DeliveryDate", dteFirstDay));
                }



                if (cmbArea.SelectedItem != null)
                {

                    filter = filter & Builders<DrugstoresSales>.Filter.And(
         Builders<DrugstoresSales>.Filter.Where(a => a.Areas.Id == dd.Id));
                }


                lstDrgSales = collection.Find(filter).ToList().OrderBy(b => b.Drugstorename).OrderBy(b => b.DeliveryDate).ToList();

                double payments = 0;
                foreach (DrugstoresSales ddd in lstDrgSales)
                {
                    ddd.strAreaName = ddd.Areas.AreaName;
                    ddd.strDate = ddd.DeliveryDate.ToString("MM/dd/yyyy");

                    foreach (PaymentsDrugstores pd in ddd.Payments)
                    {
                        payments += pd.Payment;
                    }
                    ddd.strBalance = payments.ToString();
                    payments = 0;
                }

            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("ERROR", "Cause by: " + ex.StackTrace);
            }

            return lstDrgSales;
        }

        private async void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            ReportForm rForm = new ReportForm(await GenerateReport());
            rForm.ShowDialog();
        }
    }
}
