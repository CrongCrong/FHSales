using FHSales.Classes;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Windows;

namespace FHSales
{
    /// <summary>
    /// Interaction logic for Freebies_Report.xaml
    /// </summary>
    public partial class Freebies_Report : MetroWindow
    {
        public Freebies_Report()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;

        private void MetroWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            loadReportDates();
            loadChartType();
            hideOrShowDates(false);
            hideOrShowYear(false);
            //loadProductOnCombo();
            loadCategoryOnCombo();
            //hideOrShowProduct(false);
            //hideOrShowCategory(false);
           // loadPerOnCombo();
        }

        private void btnCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
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

        //private void loadPerOnCombo()
        //{
        //    Per per = new Per();
        //    per.strPer = "CATEGORY";
        //    cmbPer.Items.Add(per);
        //    per = new Per();
        //    per.strPer = "PRODUCT";
        //    cmbPer.Items.Add(per);
        //}

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
  
        private void loadCategoryOnCombo()
        {
            conDB = new ConnectionDB();
            CategoryModel cat = new CategoryModel();

            string queryString = "SELECT ID, categoryname, description FROM dbfh.tblcategory WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);
            //cmbCategory.Items.Clear();
            
            while (reader.Read())
            {
                cat.ID = reader["ID"].ToString();
                cat.CategoryName = reader["categoryname"].ToString();
                cat.Description = reader["description"].ToString();

                //cmbCategory.Items.Add(cat);
                
                cat = new CategoryModel();
            }

            conDB.closeConnection();
        }

        //private void loadProductOnCombo()
        //{
        //    conDB = new ConnectionDB();
        //    ProductModel prod = new ProductModel();

        //    string queryString = "SELECT ID, productname, description FROM dbfh.tblproducts WHERE isDeleted = 0";

        //    MySqlDataReader reader = conDB.getSelectConnection(queryString, null);
        //    cmbProduct.Items.Clear();

        //    while (reader.Read())
        //    {
        //        prod.ID = reader["ID"].ToString();
        //        prod.ProductName = reader["productname"].ToString();
        //        prod.Description = reader["description"].ToString();

        //        cmbProduct.Items.Add(prod);

        //        prod = new ProductModel();
        //    }

        //    conDB.closeConnection();
        //}

        //private void hideOrShowCategory(bool ifShow)
        //{
        //    if (!ifShow)
        //    {
        //        label2.Visibility = Visibility.Hidden;
        //        cmbCategory.Visibility = Visibility.Hidden;

        //    }else
        //    {
        //        label2.Visibility = Visibility.Visible;
        //        cmbCategory.Visibility = Visibility.Visible;

        //    }
        //}

        //private void hideOrShowProduct(bool ifShow)
        //{
        //    if (!ifShow)
        //    {
        //        label7.Visibility = Visibility.Hidden;
        //        cmbProduct.Visibility = Visibility.Hidden;
        //    }else
        //    {
        //        label7.Visibility = Visibility.Visible;
        //        cmbProduct.Visibility = Visibility.Visible;
        //    }

        //}

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            if (cmbReportDate.SelectedItem != null)
            {
                ReportDate repDate = cmbReportDate.SelectedItem as ReportDate;
                //BankModel bnk = cmbCashBank.SelectedItem as BankModel;
                repDate.ReportType = cmbChartType.SelectedValue.ToString();

                if (repDate.DateReport.Equals("BY MONTH"))
                {
                    DateTime searchDate = DateTime.Parse(dateFrom.Text);
                    repDate.MonthFrom = searchDate.Year + "-" + searchDate.Month + "-" + searchDate.Day;

                    searchDate = DateTime.Parse(dateTo.Text);
                    repDate.MonthTo = searchDate.Year + "-" + searchDate.Month + "-" + searchDate.Day;
                    ReportForm report = new ReportForm(repDate, "FREEBIES");
                    report.ShowDialog();
                }
                else
                {
                    repDate.YearFrom = txtYearFrom.Text + "-01-01";
                    repDate.YearTo = txtYearTo.Text + "-12-31";
                    ReportForm report = new ReportForm(repDate, "FREEBIES");
                    report.ShowDialog();
                }


            }
        }

        //private void cmbPer_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        //{
        //    if (cmbPer.SelectedValue.ToString().Equals("CATEGORY"))
        //    {
        //        hideOrShowCategory(true);
        //        hideOrShowProduct(false);

        //    }else if (cmbPer.SelectedValue.ToString().Equals("PRODUCT"))
        //    {
        //        hideOrShowProduct(true);
        //        hideOrShowCategory(false);
        //    }
        //}
    }
}
