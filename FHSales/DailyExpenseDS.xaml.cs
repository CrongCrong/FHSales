using FHSales.Classes;
using FHSales.MongoClasses;
using FHSales.views;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MongoDB.Driver;
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
    /// Interaction logic for DailyExpenseDS.xaml
    /// </summary>
    public partial class DailyExpenseDS : MetroWindow
    {
        public DailyExpenseDS()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        DirectSalesDailyView directSalesDailyView;

        public DailyExpenseDS(DirectSalesDailyView dsd)
        {
            directSalesDailyView = dsd;
            InitializeComponent();
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var x = await this.ShowProgressAsync("LOADING", "Please wait...", false) as ProgressDialogController;
            dgvExpenses.ItemsSource = await loadDataGridDetails();
            await x.CloseAsync();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();
            var z = await this.ShowProgressAsync("LOADING", "Please wait...", false) as ProgressDialogController;
            if (x)
            {
                saveRecord();
                clearFields();
                dgvExpenses.ItemsSource = await loadDataGridDetails();
                await this.ShowMessageAsync("SAVE RECORD", "Record saved successfully!");
            }
            await z.CloseAsync();
        }

        private void RemoveRecord_Click(object sender, RoutedEventArgs e)
        {

        }

        private async Task<List<Expenses>> loadDataGridDetails()
        {
            List<Expenses> lstExp = new List<Expenses>();
            conDB = new ConnectionDB();
            DateTime dteNow = DateTime.Parse(DateTime.Now.ToShortDateString());
            try
            {
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                var collection = db.GetCollection<Expenses>("Expenses");
                var filter = Builders<Expenses>.Filter.And(
        Builders<Expenses>.Filter.Where(p => p.isDeleted == false),
         Builders<Expenses>.Filter.Lte("DateRecorded", dteNow));

                lstExp = collection.Find(filter).ToList();
                lstExp = lstExp.OrderByDescending(a => a.DateRecorded).ToList();

                foreach (Expenses exx in lstExp)
                {
                    exx.strDateRecorded = exx.DateRecorded.ToShortDateString();
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("ERROR", "Caused by: " + ex.StackTrace);
            }
            return lstExp;
        }

        private async void saveRecord()
        {
            try
            {
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                Expenses exp = new Expenses();

                DateTime dte = DateTime.Parse(dateExpense.Text);
                exp.DateRecorded = DateTime.Parse(dte.ToLocalTime().ToShortDateString());
                exp.ExpensesValue = txtExpense.Text;
                exp.Notes = txtNotes.Text;

                var collection = db.GetCollection<Expenses>("Expenses");
                collection.InsertOne(exp);
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("ERROR", "Caused by: " + ex.StackTrace);
            }
        }

        private async Task<bool> checkFields()
        {
            bool ifCorrect = false;

            if (string.IsNullOrEmpty(txtExpense.Text))
            {
                await this.ShowMessageAsync("EXPENSE", "Please input value.");
            }
            else if (string.IsNullOrEmpty(dateExpense.Text))
            {
                await this.ShowMessageAsync("DATE", "Please select date.");
            }
            else if (string.IsNullOrEmpty(txtNotes.Text))
            {
                await this.ShowMessageAsync("NOTES", "Please input any value.");
            }
            else
            {
                ifCorrect = true;
            }
            return ifCorrect;
        }

        private void clearFields()
        {
            dateExpense.Text = "";
            txtExpense.Text = "";
            txtNotes.Text = "";
            dgvExpenses.IsEnabled = true;
        }

        private async void loadExpense()
        {
            DateTime dteNow = DateTime.Parse(DateTime.Now.ToShortDateString());
            string dtee = "1" + "/" + dteNow.Month + "/" + dteNow.Year;
            DateTime dteFirstDay = DateTime.Parse(dtee);


            double sumEX = 0;
            try
            {
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                var collection = db.GetCollection<Expenses>("Expenses");

                var filter = Builders<Expenses>.Filter.And(
        Builders<Expenses>.Filter.Where(p => p.isDeleted == false),
         Builders<Expenses>.Filter.Eq("DateRecorded", dteNow));

                List<Expenses> lstee = collection.Find(filter).ToList();

                foreach (Expenses exx in lstee)
                {
                    sumEX = sumEX + Convert.ToDouble(exx.ExpensesValue);
                }
                directSalesDailyView. lblExpenseToday.Content = "For this day: Php " + sumEX.ToString("0.##");

                filter = Builders<Expenses>.Filter.And(
       Builders<Expenses>.Filter.Where(p => p.isDeleted == false),
        Builders<Expenses>.Filter.Gte("DateRecorded", dteFirstDay),
        Builders<Expenses>.Filter.Lte("DateRecorded", dteNow));

                lstee = collection.Find(filter).ToList();
                sumEX = 0;
                foreach (Expenses exx in lstee)
                {
                    sumEX = sumEX + Convert.ToDouble(exx.ExpensesValue);
                }
                directSalesDailyView.lblExpenseMonth.Content = "For this Month: Php " + sumEX.ToString("0.##");

            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("EXCEPTION", "ERROR LOADING DATA " + ex.Message);
            }
        }

        private void CheckIsNumeric(TextCompositionEventArgs e)
        {
            int result;

            if (!(int.TryParse(e.Text, out result) || e.Text == "."))
            {
                e.Handled = true;
            }
        }


        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            loadExpense();
        }

        private void txtExpense_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }
    }
}
