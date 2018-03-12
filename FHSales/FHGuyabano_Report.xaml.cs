using FHSales.Classes;
using MahApps.Metro.Controls;
using System;
using System.Windows;

namespace FHSales
{
    /// <summary>
    /// Interaction logic for FHGuyabano_Report.xaml
    /// </summary>
    public partial class FHGuyabano_Report : MetroWindow
    {
        public FHGuyabano_Report()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            loadCashBankonCombo();
            loadReportDates();
            loadChartType();
            hideOrShowDates(false);
            hideOrShowYear(false);
        }

        private void loadCashBankonCombo()
        {
            conDB = new ConnectionDB();
            BankModel bankCash = new BankModel();
            BankModel allBankCash = new BankModel();
            allBankCash.ID = "ALL";
            allBankCash.BankName = "All";
            allBankCash.Description = "All";
            
            cmbCashBank.Items.Add(allBankCash);
            conDB.closeConnection();

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

        private void loadReportDates()
        {
            ReportDate dateReport = new ReportDate();
            dateReport.DateReport = "BY YEAR";

            cmbReportDate.Items.Add(dateReport);
            dateReport = new ReportDate();
            dateReport.DateReport = "BY MONTH";
            cmbReportDate.Items.Add(dateReport);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCashBank.SelectedItem != null && cmbReportDate.SelectedItem != null)
            {
                ReportDate repDate = cmbReportDate.SelectedItem as ReportDate;
                BankModel bnk = cmbCashBank.SelectedItem as BankModel;
                repDate.ReportType = cmbChartType.SelectedValue.ToString();

                if (repDate.DateReport.Equals("BY MONTH"))
                {
                    DateTime searchDate = DateTime.Parse(dateFrom.Text);
                    repDate.MonthFrom = searchDate.Year + "-" + searchDate.Month + "-" + searchDate.Day;

                    searchDate = DateTime.Parse(dateTo.Text);
                    repDate.MonthTo = searchDate.Year + "-" + searchDate.Month + "-" + searchDate.Day;
                    ReportForm report = new ReportForm(bnk, repDate, "FHGUYABANO");
                    report.ShowDialog();
                }
                else
                {
                    repDate.YearFrom = txtYearFrom.Text + "-01-01";
                    repDate.YearTo = txtYearTo.Text + "-12-31";
                    ReportForm report = new ReportForm(bnk, repDate, "FHGUYABANO");
                    report.ShowDialog();
                }


            }
        }

        private void cmbReportDate_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
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
    }
}
