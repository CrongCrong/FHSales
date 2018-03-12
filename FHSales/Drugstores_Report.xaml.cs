using FHSales.Classes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using System;
using System.Windows;
using System.Windows.Controls;

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
            loadProductOnCombo();
            loadReportDates();
            loadChartType();
            hideOrShowDates(false);
            hideOrShowYear(false);
        }

        ConnectionDB conDB;

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            loadDrugstoreOnCombo();
        }

        private async void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (checkComboIfNull() && cmbReportDate.SelectedItem != null)
            {
                ReportDate repDate = cmbReportDate.SelectedItem as ReportDate;
                repDate.ReportType = cmbChartType.SelectedValue.ToString();

                if (cmbDrugstore.SelectedValue.ToString().Equals("ALL"))
                {
                    repDate.ifAllDrugstore = true;
                }else
                {
                    repDate.ifAllDrugstore = false;
                }

                if (cmbProduct.SelectedValue.ToString().Equals("ALL"))
                {
                    repDate.ifAllProducts = true;
                }
                else
                {
                    repDate.ifAllProducts = false;
                }

                repDate.DrugstoreID = cmbDrugstore.SelectedValue.ToString();
                repDate.ProductID = cmbProduct.SelectedValue.ToString();

                if (repDate.DateReport.Equals("BY MONTH"))
                {
                    DateTime searchDate = DateTime.Parse(dateFrom.Text);
                    repDate.MonthFrom = searchDate.Year + "-" + searchDate.Month + "-" + searchDate.Day;

                    searchDate = DateTime.Parse(dateTo.Text);
                    repDate.MonthTo = searchDate.Year + "-" + searchDate.Month + "-" + searchDate.Day;
                    
                    ReportForm report = new ReportForm(repDate, "DRUGSTORES");
                    report.ShowDialog();
                }
                else
                {
                    repDate.YearFrom = txtYearFrom.Text + "-01-01";
                    repDate.YearTo = txtYearTo.Text + "-12-31";
                    ReportForm report = new ReportForm(repDate, "DRUGSTORES");
                    report.ShowDialog();
                }
            }
            else
            {
                await window.ShowMessageAsync("GENERATE REPORT", "Please select values for report.");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void loadDrugstoreOnCombo()
        {
            conDB = new ConnectionDB();

            string queryString = "SELECT ID, drugstore, description FROM dbfh.tbldrugstores WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);
            cmbDrugstore.Items.Clear();
            DrugstoreModel drugAll = new DrugstoreModel();
            drugAll.ID = "ALL";
            drugAll.Description = "ALL";
            cmbDrugstore.Items.Add(drugAll);
            while (reader.Read())
            {
                DrugstoreModel drg = new DrugstoreModel();
                drg.ID = reader["ID"].ToString();
                drg.DrugstoreName = reader["drugstore"].ToString();
                drg.Description = reader["description"].ToString();

                cmbDrugstore.Items.Add(drg);
            }
            conDB.closeConnection();
        }

        private void loadProductOnCombo()
        {
            conDB = new ConnectionDB();
            ProductModel prod = new ProductModel();

            string queryString = "SELECT ID, productname, description FROM dbfh.tblproducts WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            cmbProduct.Items.Clear();
            ProductModel prodAll = new ProductModel();
            prodAll.ID = "ALL";
            prodAll.Description = "ALL";
            cmbProduct.Items.Add(prodAll);
            while (reader.Read())
            {
                prod.ID = reader["ID"].ToString();
                prod.ProductName = reader["productname"].ToString();
                prod.Description = reader["description"].ToString();

                cmbProduct.Items.Add(prod);
                
                prod = new ProductModel();
            }

            conDB.closeConnection();
        }

        private void loadReportDates()
        {
            ReportDate dateReport = new ReportDate();
            dateReport.DateReport = "BY YEAR";

            cmbReportDate.Items.Add(dateReport);
            dateReport = new ReportDate();
            dateReport.DateReport = "BY MONTH";
            cmbReportDate.Items.Add(dateReport);
        }

        private void loadChartType()
        {
            ChartType chart = new ChartType();
            chart.Type = "BAR";
            cmbChartType.Items.Add(chart);

            chart = new ChartType();
            chart.Type = "PIE";
            cmbChartType.Items.Add(chart);
        }

        private void hideOrShowDates(bool ifShow)
        {
            if (!ifShow)
            {
                label.Visibility = Visibility.Hidden;
                dateFrom.Visibility = Visibility.Hidden;
                label1.Visibility = Visibility.Hidden;
                dateTo.Visibility = Visibility.Hidden;
            }
            else
            {
                label.Visibility = Visibility.Visible;
                dateFrom.Visibility = Visibility.Visible;
                label1.Visibility = Visibility.Visible;
                dateTo.Visibility = Visibility.Visible;
            }

        }

        private void hideOrShowYear(bool ifShow)
        {
            if (!ifShow)
            {
                label4.Visibility = Visibility.Hidden;
                txtYearFrom.Visibility = Visibility.Hidden;
                label5.Visibility = Visibility.Hidden;
                txtYearTo.Visibility = Visibility.Hidden;
            }
            else
            {
                label4.Visibility = Visibility.Visible;
                txtYearFrom.Visibility = Visibility.Visible;
                label5.Visibility = Visibility.Visible;
                txtYearTo.Visibility = Visibility.Visible;
            }
        }

        private void cmbReportDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbReportDate.SelectedValue.Equals("BY MONTH"))
            {
                hideOrShowDates(true);
                hideOrShowYear(false);
            }
            else if (cmbReportDate.SelectedValue.Equals("BY YEAR"))
            {
                hideOrShowYear(true);
                hideOrShowDates(false);
            }
        }

        private bool checkComboIfNull()
        {
            bool ifOn = false;
            if(cmbDrugstore.SelectedItem != null && cmbProduct.SelectedItem != null && cmbChartType.SelectedItem != null 
                && cmbReportDate.SelectedItem != null)
            {
                if(cmbReportDate.SelectedValue.Equals("BY MONTH"))
                {
                    if (!string.IsNullOrEmpty(dateFrom.Text) && !string.IsNullOrEmpty(dateTo.Text))
                    {
                        ifOn = true;
                    }

                }else
                {
                    if (!string.IsNullOrEmpty(txtYearFrom.Text) && !string.IsNullOrEmpty(txtYearTo.Text))
                    {
                        ifOn = true;
                    }
                }                          
            }

            return ifOn;
        }

    }
}
