using FHSales.Classes;
using MahApps.Metro.Controls;
using Microsoft.Reporting.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace FHSales
{
    /// <summary>
    /// Interaction logic for ReportForm.xaml
    /// </summary>
    public partial class ReportForm : MetroWindow
    {
        public ReportForm()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        BankModel bankModel;
        ReportDate reportDate;
        string strFHProduct;
        public ReportForm(BankModel bm, ReportDate rd, string s)
        {
            reportDate = rd;
            bankModel = bm;
            strFHProduct = s;
            InitializeComponent();
        }

        public ReportForm(ReportDate rd, string s)
        {
            reportDate = rd;
            strFHProduct = s;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (strFHProduct.Equals("FHBOXES"))
            {
                #region FH BOXES REPORT
                if (bankModel.ID.Equals("ALL") && reportDate.DateReport.Equals("BY MONTH"))
                {
                    reportViewer.Reset();
                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    LocalReport localReport = reportViewer.LocalReport;


                    ReportDataSource rds;
                    if (reportDate.ReportType.Equals("BAR"))
                    {
                        localReport.ReportPath = "Reports/FHBOXES_ALL_CASHBANK.rdlc";
                        rds = new ReportDataSource("DataSet1", getDirectSalesModelAllBoxes());
                    }
                    else
                    {
                        localReport.ReportPath = "Reports/FHBOXESALLPIE.rdlc";
                        rds = new ReportDataSource("DataSet2", getDirectSalesModelAllBoxes());
                    }


                    System.Drawing.Printing.PageSettings ps = new System.Drawing.Printing.PageSettings();
                    ps.Landscape = true;
                    ps.PaperSize = new System.Drawing.Printing.PaperSize("A4", 827, 1170);
                    ps.PaperSize.RawKind = (int)System.Drawing.Printing.PaperKind.A4;
                    reportViewer.SetPageSettings(ps);

                    reportViewer.LocalReport.DataSources.Add(rds);

                    reportViewer.RefreshReport();

                }
                else if (bankModel.ID.Equals("ALL") && reportDate.DateReport.Equals("BY YEAR"))
                {
                    reportViewer.Reset();
                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    LocalReport localReport = reportViewer.LocalReport;


                    ReportDataSource rds;
                    if (reportDate.ReportType.Equals("BAR"))
                    {

                        localReport.ReportPath = "Reports/FHBOXES_ALL_YEAR_CASHBANK.rdlc";
                        rds = new ReportDataSource("DataSet1", getDirectSalesModelAllYearBoxes());
                    }
                    else
                    {
                        localReport.ReportPath = "Reports/FHBOXES_ALL_YEAR_PIE.rdlc";
                        rds = new ReportDataSource("DataSet2", getDirectSalesModelAllYearBoxes());
                    }


                    System.Drawing.Printing.PageSettings ps = new System.Drawing.Printing.PageSettings();
                    ps.Landscape = true;
                    ps.PaperSize = new System.Drawing.Printing.PaperSize("A4", 827, 1170);
                    ps.PaperSize.RawKind = (int)System.Drawing.Printing.PaperKind.A4;
                    reportViewer.SetPageSettings(ps);

                    reportViewer.LocalReport.DataSources.Add(rds);

                    reportViewer.RefreshReport();
                }

                #endregion
            }
            else if (strFHProduct.Equals("FHBOTTLES"))
            {
                #region FH BOTTLES REPORT
                if (bankModel.ID.Equals("ALL") && reportDate.DateReport.Equals("BY MONTH"))
                {
                    reportViewer.Reset();
                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    LocalReport localReport = reportViewer.LocalReport;


                    ReportDataSource rds;
                    if (reportDate.ReportType.Equals("BAR"))
                    {
                        localReport.ReportPath = "Reports/FHBOTTLES_ALL_CASHBANK.rdlc";
                        rds = new ReportDataSource("DataSet1", getDirectSalesModelAllBottles());
                    }
                    else
                    {
                        localReport.ReportPath = "Reports/FHBOTTLESALLPIE.rdlc";
                        rds = new ReportDataSource("DataSet1", getDirectSalesModelAllBottles());
                    }


                    System.Drawing.Printing.PageSettings ps = new System.Drawing.Printing.PageSettings();
                    ps.Landscape = true;
                    ps.PaperSize = new System.Drawing.Printing.PaperSize("A4", 827, 1170);
                    ps.PaperSize.RawKind = (int)System.Drawing.Printing.PaperKind.A4;
                    reportViewer.SetPageSettings(ps);

                    reportViewer.LocalReport.DataSources.Add(rds);

                    reportViewer.RefreshReport();

                }
                else if (bankModel.ID.Equals("ALL") && reportDate.DateReport.Equals("BY YEAR"))
                {
                    reportViewer.Reset();
                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    LocalReport localReport = reportViewer.LocalReport;


                    ReportDataSource rds;
                    if (reportDate.ReportType.Equals("BAR"))
                    {

                        localReport.ReportPath = "Reports/FHBOTTLES_ALL_YEAR_CASHBANK.rdlc";
                        rds = new ReportDataSource("DataSet1", getDirectSalesModelAllYearBottles());
                    }
                    else
                    {
                        localReport.ReportPath = "Reports/FHBOTTLES_ALL_YEAR_PIE.rdlc";
                        rds = new ReportDataSource("DataSet1", getDirectSalesModelAllYearBottles());
                    }


                    System.Drawing.Printing.PageSettings ps = new System.Drawing.Printing.PageSettings();
                    ps.Landscape = true;
                    ps.PaperSize = new System.Drawing.Printing.PaperSize("A4", 827, 1170);
                    ps.PaperSize.RawKind = (int)System.Drawing.Printing.PaperKind.A4;
                    reportViewer.SetPageSettings(ps);

                    reportViewer.LocalReport.DataSources.Add(rds);

                    reportViewer.RefreshReport();
                }

                #endregion
            }
            else if (strFHProduct.Equals("FHGUAVA"))
            {
                #region FH GUAVA REPORT
                if (bankModel.ID.Equals("ALL") && reportDate.DateReport.Equals("BY MONTH"))
                {
                    reportViewer.Reset();
                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    LocalReport localReport = reportViewer.LocalReport;


                    ReportDataSource rds;
                    if (reportDate.ReportType.Equals("BAR"))
                    {
                        localReport.ReportPath = "Reports/FHGUAVA_ALL_CASHBANK.rdlc";
                        rds = new ReportDataSource("DataSet1", getDirectSalesModelAllGuava());
                    }
                    else
                    {
                        localReport.ReportPath = "Reports/FHGUAVAALLPIE.rdlc";
                        rds = new ReportDataSource("DataSet1", getDirectSalesModelAllGuava());
                    }


                    System.Drawing.Printing.PageSettings ps = new System.Drawing.Printing.PageSettings();
                    ps.Landscape = true;
                    ps.PaperSize = new System.Drawing.Printing.PaperSize("A4", 827, 1170);
                    ps.PaperSize.RawKind = (int)System.Drawing.Printing.PaperKind.A4;
                    reportViewer.SetPageSettings(ps);

                    reportViewer.LocalReport.DataSources.Add(rds);

                    reportViewer.RefreshReport();

                }
                else if (bankModel.ID.Equals("ALL") && reportDate.DateReport.Equals("BY YEAR"))
                {
                    reportViewer.Reset();
                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    LocalReport localReport = reportViewer.LocalReport;


                    ReportDataSource rds;
                    if (reportDate.ReportType.Equals("BAR"))
                    {

                        localReport.ReportPath = "Reports/FHGUAVA_ALL_YEAR_CASHBANK.rdlc";
                        rds = new ReportDataSource("DataSet1", getDirectSalesModelAllYearGuava());
                    }
                    else
                    {
                        localReport.ReportPath = "Reports/FHGUAVA_ALL_YEAR_PIE.rdlc";
                        rds = new ReportDataSource("DataSet1", getDirectSalesModelAllYearGuava());
                    }


                    System.Drawing.Printing.PageSettings ps = new System.Drawing.Printing.PageSettings();
                    ps.Landscape = true;
                    ps.PaperSize = new System.Drawing.Printing.PaperSize("A4", 827, 1170);
                    ps.PaperSize.RawKind = (int)System.Drawing.Printing.PaperKind.A4;
                    reportViewer.SetPageSettings(ps);

                    reportViewer.LocalReport.DataSources.Add(rds);

                    reportViewer.RefreshReport();
                }

                #endregion
            }
            else if (strFHProduct.Equals("FHGUYABANO"))
            {
                #region FH GUYABANO REPORT
                if (bankModel.ID.Equals("ALL") && reportDate.DateReport.Equals("BY MONTH"))
                {
                    reportViewer.Reset();
                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    LocalReport localReport = reportViewer.LocalReport;


                    ReportDataSource rds;
                    if (reportDate.ReportType.Equals("BAR"))
                    {
                        localReport.ReportPath = "Reports/FHGUYABANO_ALL_CASHBANK.rdlc";
                        rds = new ReportDataSource("DataSet1", getDirectSalesModelAllGuyabano());
                    }
                    else
                    {
                        localReport.ReportPath = "Reports/FHGUYABANOALLPIE.rdlc";
                        rds = new ReportDataSource("DataSet1", getDirectSalesModelAllGuyabano());
                    }


                    System.Drawing.Printing.PageSettings ps = new System.Drawing.Printing.PageSettings();
                    ps.Landscape = true;
                    ps.PaperSize = new System.Drawing.Printing.PaperSize("A4", 827, 1170);
                    ps.PaperSize.RawKind = (int)System.Drawing.Printing.PaperKind.A4;
                    reportViewer.SetPageSettings(ps);

                    reportViewer.LocalReport.DataSources.Add(rds);

                    reportViewer.RefreshReport();

                }
                else if (bankModel.ID.Equals("ALL") && reportDate.DateReport.Equals("BY YEAR"))
                {
                    reportViewer.Reset();
                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    LocalReport localReport = reportViewer.LocalReport;


                    ReportDataSource rds;
                    if (reportDate.ReportType.Equals("BAR"))
                    {

                        localReport.ReportPath = "Reports/FHGUYABANO_ALL_YEAR_CASHBANK.rdlc";
                        rds = new ReportDataSource("DataSet1", getDirectSalesModelAllYearGuyabano());
                    }
                    else
                    {
                        localReport.ReportPath = "Reports/FHGUYABANO_ALL_YEAR_PIE.rdlc";
                        rds = new ReportDataSource("DataSet1", getDirectSalesModelAllYearGuyabano());
                    }


                    System.Drawing.Printing.PageSettings ps = new System.Drawing.Printing.PageSettings();
                    ps.Landscape = true;
                    ps.PaperSize = new System.Drawing.Printing.PaperSize("A4", 827, 1170);
                    ps.PaperSize.RawKind = (int)System.Drawing.Printing.PaperKind.A4;
                    reportViewer.SetPageSettings(ps);

                    reportViewer.LocalReport.DataSources.Add(rds);

                    reportViewer.RefreshReport();
                }
                #endregion
            }
            else if (strFHProduct.Equals("FREEBIES"))
            {
                #region FREEBIES

                ReportDataSource rds = new ReportDataSource();
                if (reportDate.DateReport.Equals("BY MONTH"))
                {
                    reportViewer.Reset();
                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    LocalReport localReport = reportViewer.LocalReport;


                    if (reportDate.ReportType.Equals("BAR"))
                    {
                        localReport.ReportPath = "Reports/FREEBIES_ALL.rdlc";
                        rds = new ReportDataSource("DataSet1", getFreebiesRecords());
                    }
            
                    System.Drawing.Printing.PageSettings ps = new System.Drawing.Printing.PageSettings();
                    ps.Landscape = true;
                    ps.PaperSize = new System.Drawing.Printing.PaperSize("A4", 827, 1170);
                    ps.PaperSize.RawKind = (int)System.Drawing.Printing.PaperKind.A4;
                    reportViewer.SetPageSettings(ps);

                    reportViewer.LocalReport.DataSources.Add(rds);

                    reportViewer.RefreshReport();

                }
                else if (reportDate.DateReport.Equals("BY YEAR"))
                {
                    reportViewer.Reset();
                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    LocalReport localReport = reportViewer.LocalReport;
                    
                    if (reportDate.ReportType.Equals("BAR"))
                    {

                        localReport.ReportPath = "Reports/FREEBIES_ALL_YEAR.rdlc";
                        rds = new ReportDataSource("DataSet1", getFreebiesRecordsYear());
                    }
                   
                    System.Drawing.Printing.PageSettings ps = new System.Drawing.Printing.PageSettings();
                    ps.Landscape = true;
                    ps.PaperSize = new System.Drawing.Printing.PaperSize("A4", 827, 1170);
                    ps.PaperSize.RawKind = (int)System.Drawing.Printing.PaperKind.A4;
                    reportViewer.SetPageSettings(ps);

                    reportViewer.LocalReport.DataSources.Add(rds);

                    reportViewer.RefreshReport();
                }

                #endregion
            }
            else if (strFHProduct.Equals("DRUGSTORES"))
            {
                #region DRUGSTORE
                ReportDataSource rds = new ReportDataSource();
                if (reportDate.DateReport.Equals("BY MONTH"))
                {
                    reportViewer.Reset();
                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    LocalReport localReport = reportViewer.LocalReport;


                    if(reportDate.ifAllDrugstore && reportDate.ifAllProducts)
                    {
                        if (reportDate.ReportType.Equals("BAR"))
                        {
                            localReport.ReportPath = "Reports/DRUGSTORE_ALL_ALLPRODUCT.rdlc";
                            rds = new ReportDataSource("DataSet1", getDrugstoreSalesAllDrugstoreAllProduct());
                        }
                    }else if(reportDate.ifAllDrugstore && !reportDate.ifAllProducts)
                    {
                        if (reportDate.ReportType.Equals("BAR"))
                        {
                            localReport.ReportPath = "Reports/DRUGSTORE_ALL_PERPRODUCT.rdlc";
                            rds = new ReportDataSource("DataSet1", getDrugstoreSalesPerProductAllDrugstores());
                        }
                    }else if (!reportDate.ifAllDrugstore && reportDate.ifAllProducts)
                    {
                        if (reportDate.ReportType.Equals("BAR"))
                        {
                            localReport.ReportPath = "Reports/DRUGSTORE_PER_ALLPRODUCT.rdlc";
                            rds = new ReportDataSource("DataSet1", getDrugstoreSalesAllProductPerDrugstores());
                        }
                    }else if (!reportDate.ifAllDrugstore && !reportDate.ifAllProducts)
                    {
                        if (reportDate.ReportType.Equals("BAR"))
                        {
                            localReport.ReportPath = "Reports/DRUGSTORE_PER_PERPRODUCT.rdlc";
                            rds = new ReportDataSource("DataSet1", getDrugstoreSalesPerProductPerDrugstores());
                        }
                    }

                    

                    System.Drawing.Printing.PageSettings ps = new System.Drawing.Printing.PageSettings();
                    ps.Landscape = true;
                    ps.PaperSize = new System.Drawing.Printing.PaperSize("A4", 827, 1170);
                    ps.PaperSize.RawKind = (int)System.Drawing.Printing.PaperKind.A4;
                    reportViewer.SetPageSettings(ps);

                    reportViewer.LocalReport.DataSources.Add(rds);

                    reportViewer.RefreshReport();

                }
                else if (reportDate.DateReport.Equals("BY YEAR"))
                {
                    reportViewer.Reset();
                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    LocalReport localReport = reportViewer.LocalReport;

                    if (reportDate.ifAllDrugstore && reportDate.ifAllProducts)
                    {
                        if (reportDate.ReportType.Equals("BAR"))
                        {

                            localReport.ReportPath = "Reports/DRUGSTORE_ALL_ALLPRODUCT_YEAR.rdlc";
                            rds = new ReportDataSource("DataSet1", getDrugstoreSalesAllDrugstoreAllProductYear());
                        }

                       
                    }
                    else if (reportDate.ifAllDrugstore && !reportDate.ifAllProducts)
                    {
                        if (reportDate.ReportType.Equals("BAR"))
                        {

                            localReport.ReportPath = "Reports/DRUGSTORE_ALL_PERPRODUCT_YEAR.rdlc";
                            rds = new ReportDataSource("DataSet1", getDrugstoreSalesPerProductAllDrugstoresYear());
                        }
                    }
                    else if (!reportDate.ifAllDrugstore && reportDate.ifAllProducts)
                    {
                        if (reportDate.ReportType.Equals("BAR"))
                        {
                            localReport.ReportPath = "Reports/DRUGSTORE_PER_ALLPRODUCT_YEAR.rdlc";
                            rds = new ReportDataSource("DataSet1", getDrugstoreSalesAllProductPerDrugstoresYear());
                        }
                    }
                    else if (!reportDate.ifAllDrugstore && !reportDate.ifAllProducts)
                    {
                        if (reportDate.ReportType.Equals("BAR"))
                        {
                            localReport.ReportPath = "Reports/DRUGSTORE_PER_PERPRODUCT_YEAR.rdlc";
                            rds = new ReportDataSource("DataSet1", getDrugstoreSalesPerProductPerDrugstoresYear());
                        }
                    }

                    System.Drawing.Printing.PageSettings ps = new System.Drawing.Printing.PageSettings();
                    ps.Landscape = true;
                    ps.PaperSize = new System.Drawing.Printing.PaperSize("A4", 827, 1170);
                    ps.PaperSize.RawKind = (int)System.Drawing.Printing.PaperKind.A4;
                    reportViewer.SetPageSettings(ps);

                    reportViewer.LocalReport.DataSources.Add(rds);

                    reportViewer.RefreshReport();

                }


                #endregion               
            }


        }


        private List<DirectSalesModel> getDirectSalesModelAllBoxes()
        {
            conDB = new ConnectionDB();

            List<DirectSalesModel> lstDirectSalesAll = new List<DirectSalesModel>();

            DirectSalesModel directSales = new DirectSalesModel();
            directSales.Title = "SALES REPORT : " + reportDate.MonthFrom.Substring(0, 4);
            String queryString = "SELECT monthname(deliverydate) as deliverydate, cashbankID, dbfh.tblbank.description, sum(totalprice) as total FROM " +
                "(dbfh.tbldirectsales INNER JOIN dbfh.tblbank ON dbfh.tbldirectsales.cashbankID = dbfh.tblbank.ID) " +
                "WHERE (dbfh.tbldirectsales.deliverydate BETWEEN ? AND ?) AND (dbfh.tbldirectsales.salestypeID = 1) AND (dbfh.tbldirectsales.isDeleted = 0) GROUP BY MONTH(deliverydate)";

            List<string> parameters = new List<string>();
            parameters.Add(reportDate.MonthFrom);
            parameters.Add(reportDate.MonthTo);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                directSales.CashBankID = reader["cashbankID"].ToString();
                directSales.CashBankName = reader["description"].ToString();
                directSales.DeliveryDate = reader["deliverydate"].ToString();
                directSales.TotalPrice = Convert.ToInt32(reader["total"].ToString());

                lstDirectSalesAll.Add(directSales);
                directSales = new DirectSalesModel();
            }

            conDB.closeConnection();

            return lstDirectSalesAll;

        }

        private List<DirectSalesModel> getDirectSalesModelAllYearBoxes()
        {
            conDB = new ConnectionDB();

            List<DirectSalesModel> lstDirectSalesAll = new List<DirectSalesModel>();

            DirectSalesModel directSales = new DirectSalesModel();
            directSales.Title = "SALES REPORT : " + reportDate.YearFrom.Substring(0, 4) + " - " + reportDate.YearTo.Substring(0, 4);
            String queryString = "SELECT year(deliverydate) as deliverydate, cashbankID, dbfh.tblbank.description, sum(totalprice) as total FROM " +
                "(dbfh.tbldirectsales INNER JOIN dbfh.tblbank ON dbfh.tbldirectsales.cashbankID = dbfh.tblbank.ID) " +
                "WHERE (dbfh.tbldirectsales.deliverydate BETWEEN ? AND ?) AND (dbfh.tbldirectsales.salestypeID = 1) AND (dbfh.tbldirectsales.isDeleted = 0) GROUP BY year(deliverydate)";

            List<string> parameters = new List<string>();
            parameters.Add(reportDate.YearFrom);
            parameters.Add(reportDate.YearTo);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                directSales.CashBankID = reader["cashbankID"].ToString();
                directSales.CashBankName = reader["description"].ToString();
                directSales.DeliveryDate = reader["deliverydate"].ToString();
                directSales.TotalPrice = Convert.ToInt32(reader["total"].ToString());

                lstDirectSalesAll.Add(directSales);
                directSales = new DirectSalesModel();
            }

            conDB.closeConnection();

            return lstDirectSalesAll;

        }

        private List<DirectSalesModel> getDirectSalesModelAllBottles()
        {
            conDB = new ConnectionDB();

            List<DirectSalesModel> lstDirectSalesAll = new List<DirectSalesModel>();

            DirectSalesModel directSales = new DirectSalesModel();
            directSales.Title = "SALES REPORT : " + reportDate.MonthFrom.Substring(0, 4);
            String queryString = "SELECT monthname(deliverydate) as deliverydate, cashbankID, dbfh.tblbank.description, sum(totalprice) as total FROM " +
                "(dbfh.tbldirectsales INNER JOIN dbfh.tblbank ON dbfh.tbldirectsales.cashbankID = dbfh.tblbank.ID) " +
                "WHERE (dbfh.tbldirectsales.deliverydate BETWEEN ? AND ?) AND (dbfh.tbldirectsales.salestypeID = 2) AND (dbfh.tbldirectsales.isDeleted = 0) GROUP BY MONTH(deliverydate)";

            List<string> parameters = new List<string>();
            parameters.Add(reportDate.MonthFrom);
            parameters.Add(reportDate.MonthTo);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                directSales.CashBankID = reader["cashbankID"].ToString();
                directSales.CashBankName = reader["description"].ToString();
                directSales.DeliveryDate = reader["deliverydate"].ToString();
                directSales.TotalPrice = Convert.ToInt32(reader["total"].ToString());

                lstDirectSalesAll.Add(directSales);
                directSales = new DirectSalesModel();
            }

            conDB.closeConnection();

            return lstDirectSalesAll;

        }

        private List<DirectSalesModel> getDirectSalesModelAllYearBottles()
        {
            conDB = new ConnectionDB();

            List<DirectSalesModel> lstDirectSalesAll = new List<DirectSalesModel>();

            DirectSalesModel directSales = new DirectSalesModel();
            directSales.Title = "SALES REPORT : " + reportDate.YearFrom.Substring(0, 4) + " - " + reportDate.YearTo.Substring(0, 4);
            String queryString = "SELECT year(deliverydate) as deliverydate, cashbankID, dbfh.tblbank.description, sum(totalprice) as total FROM " +
                "(dbfh.tbldirectsales INNER JOIN dbfh.tblbank ON dbfh.tbldirectsales.cashbankID = dbfh.tblbank.ID) " +
                "WHERE (dbfh.tbldirectsales.deliverydate BETWEEN ? AND ?) AND (dbfh.tbldirectsales.salestypeID = 2) AND (dbfh.tbldirectsales.isDeleted = 0) GROUP BY year(deliverydate)";

            List<string> parameters = new List<string>();
            parameters.Add(reportDate.YearFrom);
            parameters.Add(reportDate.YearTo);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                directSales.CashBankID = reader["cashbankID"].ToString();
                directSales.CashBankName = reader["description"].ToString();
                directSales.DeliveryDate = reader["deliverydate"].ToString();
                directSales.TotalPrice = Convert.ToInt32(reader["total"].ToString());

                lstDirectSalesAll.Add(directSales);
                directSales = new DirectSalesModel();
            }

            conDB.closeConnection();

            return lstDirectSalesAll;

        }

        private List<DirectSalesModel> getDirectSalesModelAllGuava()
        {
            conDB = new ConnectionDB();

            List<DirectSalesModel> lstDirectSalesAll = new List<DirectSalesModel>();

            DirectSalesModel directSales = new DirectSalesModel();
            directSales.Title = "SALES REPORT : " + reportDate.MonthFrom.Substring(0, 4);
            String queryString = "SELECT monthname(deliverydate) as deliverydate, cashbankID, dbfh.tblbank.description, sum(totalprice) as total FROM " +
                "(dbfh.tbldirectsales INNER JOIN dbfh.tblbank ON dbfh.tbldirectsales.cashbankID = dbfh.tblbank.ID) " +
                "WHERE (dbfh.tbldirectsales.deliverydate BETWEEN ? AND ?) AND (dbfh.tbldirectsales.salestypeID = 3) AND (dbfh.tbldirectsales.isDeleted = 0) GROUP BY MONTH(deliverydate)";

            List<string> parameters = new List<string>();
            parameters.Add(reportDate.MonthFrom);
            parameters.Add(reportDate.MonthTo);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                directSales.CashBankID = reader["cashbankID"].ToString();
                directSales.CashBankName = reader["description"].ToString();
                directSales.DeliveryDate = reader["deliverydate"].ToString();
                directSales.TotalPrice = Convert.ToInt32(reader["total"].ToString());

                lstDirectSalesAll.Add(directSales);
                directSales = new DirectSalesModel();
            }

            conDB.closeConnection();

            return lstDirectSalesAll;

        }

        private List<DirectSalesModel> getDirectSalesModelAllYearGuava()
        {
            conDB = new ConnectionDB();

            List<DirectSalesModel> lstDirectSalesAll = new List<DirectSalesModel>();

            DirectSalesModel directSales = new DirectSalesModel();
            directSales.Title = "SALES REPORT : " + reportDate.YearFrom.Substring(0, 4) + " - " + reportDate.YearTo.Substring(0, 4);
            String queryString = "SELECT year(deliverydate) as deliverydate, cashbankID, dbfh.tblbank.description, sum(totalprice) as total FROM " +
                "(dbfh.tbldirectsales INNER JOIN dbfh.tblbank ON dbfh.tbldirectsales.cashbankID = dbfh.tblbank.ID) " +
                "WHERE (dbfh.tbldirectsales.deliverydate BETWEEN ? AND ?) AND (dbfh.tbldirectsales.salestypeID = 3) AND (dbfh.tbldirectsales.isDeleted = 0) GROUP BY year(deliverydate)";

            List<string> parameters = new List<string>();
            parameters.Add(reportDate.YearFrom);
            parameters.Add(reportDate.YearTo);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                directSales.CashBankID = reader["cashbankID"].ToString();
                directSales.CashBankName = reader["description"].ToString();
                directSales.DeliveryDate = reader["deliverydate"].ToString();
                directSales.TotalPrice = Convert.ToInt32(reader["total"].ToString());

                lstDirectSalesAll.Add(directSales);
                directSales = new DirectSalesModel();
            }

            conDB.closeConnection();

            return lstDirectSalesAll;

        }

        private List<DirectSalesModel> getDirectSalesModelAllGuyabano()
        {
            conDB = new ConnectionDB();

            List<DirectSalesModel> lstDirectSalesAll = new List<DirectSalesModel>();

            DirectSalesModel directSales = new DirectSalesModel();
            directSales.Title = "SALES REPORT : " + reportDate.MonthFrom.Substring(0, 4);
            String queryString = "SELECT monthname(deliverydate) as deliverydate, cashbankID, dbfh.tblbank.description, sum(totalprice) as total FROM " +
                "(dbfh.tbldirectsales INNER JOIN dbfh.tblbank ON dbfh.tbldirectsales.cashbankID = dbfh.tblbank.ID) " +
                "WHERE (dbfh.tbldirectsales.deliverydate BETWEEN ? AND ?) AND (dbfh.tbldirectsales.salestypeID = 4)  AND (dbfh.tbldirectsales.isDeleted = 0) GROUP BY MONTH(deliverydate)";

            List<string> parameters = new List<string>();
            parameters.Add(reportDate.MonthFrom);
            parameters.Add(reportDate.MonthTo);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                directSales.CashBankID = reader["cashbankID"].ToString();
                directSales.CashBankName = reader["description"].ToString();
                directSales.DeliveryDate = reader["deliverydate"].ToString();
                directSales.TotalPrice = Convert.ToInt32(reader["total"].ToString());

                lstDirectSalesAll.Add(directSales);
                directSales = new DirectSalesModel();
            }

            conDB.closeConnection();

            return lstDirectSalesAll;

        }

        private List<DirectSalesModel> getDirectSalesModelAllYearGuyabano()
        {
            conDB = new ConnectionDB();

            List<DirectSalesModel> lstDirectSalesAll = new List<DirectSalesModel>();

            DirectSalesModel directSales = new DirectSalesModel();
            directSales.Title = "SALES REPORT : " + reportDate.YearFrom.Substring(0, 4) + " - " + reportDate.YearTo.Substring(0, 4);
            String queryString = "SELECT year(deliverydate) as deliverydate, cashbankID, dbfh.tblbank.description, sum(totalprice) as total FROM " +
                "(dbfh.tbldirectsales INNER JOIN dbfh.tblbank ON dbfh.tbldirectsales.cashbankID = dbfh.tblbank.ID) " +
                "WHERE (dbfh.tbldirectsales.deliverydate BETWEEN ? AND ?) AND (dbfh.tbldirectsales.salestypeID = 4) AND (dbfh.tbldirectsales.isDeleted = 0) GROUP BY year(deliverydate)";

            List<string> parameters = new List<string>();
            parameters.Add(reportDate.YearFrom);
            parameters.Add(reportDate.YearTo);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                directSales.CashBankID = reader["cashbankID"].ToString();
                directSales.CashBankName = reader["description"].ToString();
                directSales.DeliveryDate = reader["deliverydate"].ToString();
                directSales.TotalPrice = Convert.ToInt32(reader["total"].ToString());

                lstDirectSalesAll.Add(directSales);
                directSales = new DirectSalesModel();
            }

            conDB.closeConnection();

            return lstDirectSalesAll;

        }

        private List<FreebiesModel> getFreebiesRecords()
        {
            conDB = new ConnectionDB();

            List<FreebiesModel> lstFreebies = new List<FreebiesModel>();
            FreebiesModel freeby = new FreebiesModel();

            string queryString = "SELECT sum(quantity) as total, monthname(deliverydate) as deliverydate, " +                
                "dbfh.tblfreebies.categoryID, dbfh.tblcategory.description AS catdesc, dbfh.tblproducts.description AS proddesc, " +
                "dbfh.tblproducts.ID AS prodID FROM((dbfh.tblfreebies INNER JOIN dbfh.tblcategory ON dbfh.tblfreebies.categoryID  " +
                "= dbfh.tblcategory.ID) INNER JOIN dbfh.tblproducts ON dbfh.tblfreebies.productID = dbfh.tblproducts.ID) " +
                "WHERE (dbfh.tblfreebies.deliverydate BETWEEN ? AND ?) AND (dbfh.tblfreebies.isDeleted = 0) GROUP BY dbfh.tblfreebies.categoryID, dbfh.tblfreebies.productID, deliverydate";

            List<string> parameters = new List<string>();
            parameters.Add(reportDate.MonthFrom);
            parameters.Add(reportDate.MonthTo);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                freeby.Quantity = Convert.ToInt32(reader["total"].ToString());
                //DateTime dte = DateTime.Parse(reader["deliverydate"].ToString());
                freeby.DeliveryDate = reader["deliverydate"].ToString();
                freeby.CategoryID = reader["categoryID"].ToString();
                freeby.CategoryName = reader["catdesc"].ToString();
                freeby.ProductID = reader["prodID"].ToString();
                freeby.ProductName = reader["proddesc"].ToString();

                lstFreebies.Add(freeby);
                freeby = new FreebiesModel();
            }
            conDB.closeConnection();

            return lstFreebies;
        }

        private List<FreebiesModel> getFreebiesRecordsYear()
        {
            conDB = new ConnectionDB();

            List<FreebiesModel> lstFreebies = new List<FreebiesModel>();
            FreebiesModel freeby = new FreebiesModel();

            string queryString = "SELECT sum(quantity) as total, year(deliverydate) as deliverydate, " +
                "dbfh.tblfreebies.categoryID, dbfh.tblcategory.description AS catdesc, dbfh.tblproducts.description AS proddesc, " +
                "dbfh.tblproducts.ID AS prodID FROM((dbfh.tblfreebies INNER JOIN dbfh.tblcategory ON dbfh.tblfreebies.categoryID  " +
                "= dbfh.tblcategory.ID) INNER JOIN dbfh.tblproducts ON dbfh.tblfreebies.productID = dbfh.tblproducts.ID) " +
                "WHERE (dbfh.tblfreebies.deliverydate BETWEEN ? AND ?) AND (dbfh.tblfreebies.isDeleted = 0) GROUP BY dbfh.tblfreebies.categoryID, dbfh.tblfreebies.productID, deliverydate";

            List<string> parameters = new List<string>();
            parameters.Add(reportDate.YearFrom);
            parameters.Add(reportDate.YearTo);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                freeby.Quantity = Convert.ToInt32(reader["total"].ToString());
                //DateTime dte = DateTime.Parse(reader["deliverydate"].ToString());
                freeby.DeliveryDate = reader["deliverydate"].ToString();
                freeby.CategoryID = reader["categoryID"].ToString();
                freeby.CategoryName = reader["catdesc"].ToString();
                freeby.ProductID = reader["prodID"].ToString();
                freeby.ProductName = reader["proddesc"].ToString();

                lstFreebies.Add(freeby);
                freeby = new FreebiesModel();
            }
            conDB.closeConnection();

            return lstFreebies;
        }

        private List<DrugstoresSalesModel> getDrugstoreSalesAllDrugstoreAllProduct()
        {
            conDB = new ConnectionDB();

            List<DrugstoresSalesModel> lstDrugstore = new List<DrugstoresSalesModel>();
            DrugstoresSalesModel drugstoresMod = new DrugstoresSalesModel();

            drugstoresMod.Title = "All Drugstores - All Products";
            string queryString = "SELECT monthname(deliverydate) as deliverydate, sum(quantity) as total, " +    
                "dbfh.tbldrugstores.description AS drugstore  FROM (dbfh.tblsales INNER JOIN dbfh.tbldrugstores " +
                "ON dbfh.tblsales.drugstoreID = dbfh.tbldrugstores.ID) WHERE (deliverydate BETWEEN ? AND ?) AND " +
                "(dbfh.tblsales.isDeleted = 0) GROUP BY DATE(dbfh.tblsales.deliverydate), dbfh.tblsales.drugstoreID";

            List<string> parameters = new List<string>();
            parameters.Add(reportDate.MonthFrom);
            parameters.Add(reportDate.MonthTo);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                drugstoresMod.DeliveryDate = reader["deliverydate"].ToString();
                drugstoresMod.Quantity = Convert.ToInt32(reader["total"].ToString());
                drugstoresMod.DrugstoreName = reader["drugstore"].ToString();

                lstDrugstore.Add(drugstoresMod);
                drugstoresMod = new DrugstoresSalesModel();
            }

            conDB.closeConnection();

            return lstDrugstore;
        }

        private List<DrugstoresSalesModel> getDrugstoreSalesAllDrugstoreAllProductYear()
        {
            conDB = new ConnectionDB();

            List<DrugstoresSalesModel> lstDrugstore = new List<DrugstoresSalesModel>();
            DrugstoresSalesModel drugstoresMod = new DrugstoresSalesModel();

            drugstoresMod.Title = "All Drugstores - All Products";
            string queryString = "SELECT year(deliverydate) as deliverydate, sum(quantity) as total " +
                "FROM dbfh.tblsales WHERE (deliverydate BETWEEN ? AND ?) AND " +
                "(dbfh.tblsales.isDeleted = 0) GROUP BY year(deliverydate)";

            List<string> parameters = new List<string>();
            parameters.Add(reportDate.YearFrom);
            parameters.Add(reportDate.YearTo);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                drugstoresMod.DeliveryDate = reader["deliverydate"].ToString();
                drugstoresMod.Total = Convert.ToInt32(reader["total"].ToString());

                lstDrugstore.Add(drugstoresMod);
                drugstoresMod = new DrugstoresSalesModel();
            }

            conDB.closeConnection();

            return lstDrugstore;
        }

        private List<DrugstoresSalesModel> getDrugstoreSalesPerProductAllDrugstores()
        {
            conDB = new ConnectionDB();

            List<DrugstoresSalesModel> lstDrugstore = new List<DrugstoresSalesModel>();
            DrugstoresSalesModel drugstoresMod = new DrugstoresSalesModel();


            string queryString = "SELECT monthname(deliverydate) as deliverydate, sum(quantity) as total, " +
                "dbfh.tbldrugstores.description AS drugstore, dbfh.tblproducts.description AS product " +
                "FROM ((dbfh.tblsales INNER JOIN dbfh.tbldrugstores " +
                "ON dbfh.tblsales.drugstoreID = dbfh.tbldrugstores.ID) INNER JOIN dbfh.tblproducts " +
                "ON dbfh.tblsales.productID = dbfh.tblproducts.ID)  WHERE (deliverydate BETWEEN ? AND ?) AND " +
                "(dbfh.tblsales.isDeleted = 0) AND (dbfh.tblsales.productID = ?) GROUP BY deliverydate ORDER BY DATE(deliverydate) ASC";

            List<string> parameters = new List<string>();
            parameters.Add(reportDate.MonthFrom);
            parameters.Add(reportDate.MonthTo);
            parameters.Add(reportDate.ProductID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                
                drugstoresMod.DeliveryDate = reader["deliverydate"].ToString();
                drugstoresMod.ProductName = reader["product"].ToString();
                drugstoresMod.Quantity = Convert.ToInt32(reader["total"].ToString());
                drugstoresMod.DrugstoreName = reader["drugstore"].ToString();
                drugstoresMod.Title = "MONTHLY REPORT: " + drugstoresMod.ProductName;
                lstDrugstore.Add(drugstoresMod);
                drugstoresMod = new DrugstoresSalesModel();
            }

            conDB.closeConnection();

            return lstDrugstore;
        }

        private List<DrugstoresSalesModel> getDrugstoreSalesPerProductAllDrugstoresYear()
        {
            conDB = new ConnectionDB();

            List<DrugstoresSalesModel> lstDrugstore = new List<DrugstoresSalesModel>();
            DrugstoresSalesModel drugstoresMod = new DrugstoresSalesModel();


            string queryString = "SELECT year(deliverydate) as deliverydate, sum(quantity) as total,  " +
                "dbfh.tblproducts.description AS product " +
                "FROM (dbfh.tblsales INNER JOIN dbfh.tblproducts " +
                "ON dbfh.tblsales.productID = dbfh.tblproducts.ID) WHERE (deliverydate BETWEEN ? AND ?) AND " +
                "(dbfh.tblsales.isDeleted = 0) AND (dbfh.tblsales.productID = ?) GROUP BY year(deliverydate)";

            List<string> parameters = new List<string>();
            parameters.Add(reportDate.YearFrom);
            parameters.Add(reportDate.YearTo);
            parameters.Add(reportDate.ProductID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                drugstoresMod.DeliveryDate = reader["deliverydate"].ToString();
                drugstoresMod.ProductName = reader["product"].ToString();
                drugstoresMod.Total = Convert.ToInt32(reader["total"].ToString());
                drugstoresMod.Title = "YEARLY REPORT: " + drugstoresMod.ProductName;
                lstDrugstore.Add(drugstoresMod);
                
                drugstoresMod = new DrugstoresSalesModel();
            }

            conDB.closeConnection();

            return lstDrugstore;
        }

        private List<DrugstoresSalesModel> getDrugstoreSalesAllProductPerDrugstores()
        {
            conDB = new ConnectionDB();

            List<DrugstoresSalesModel> lstDrugstore = new List<DrugstoresSalesModel>();
            DrugstoresSalesModel drugstoresMod = new DrugstoresSalesModel();


            string queryString = "SELECT monthname(deliverydate) as deliverydate, sum(quantity) as total, " +
                "dbfh.tbldrugstores.description AS drugstore, dbfh.tblproducts.description AS product " +
                "FROM ((dbfh.tblsales INNER JOIN dbfh.tbldrugstores " +
                "ON dbfh.tblsales.drugstoreID = dbfh.tbldrugstores.ID) INNER JOIN dbfh.tblproducts " +
                "ON dbfh.tblsales.productID = dbfh.tblproducts.ID)  WHERE (deliverydate BETWEEN ? AND ?) AND " +
                "(dbfh.tblsales.isDeleted = 0) AND (dbfh.tblsales.drugstoreID = ?) GROUP BY DATE(deliverydate) ORDER BY DATE(deliverydate) ASC";

            List<string> parameters = new List<string>();
            
            parameters.Add(reportDate.MonthFrom);
            parameters.Add(reportDate.MonthTo);
            parameters.Add(reportDate.DrugstoreID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                drugstoresMod.DeliveryDate = reader["deliverydate"].ToString();
                drugstoresMod.ProductName = reader["product"].ToString();
                drugstoresMod.Quantity = Convert.ToInt32(reader["total"].ToString());
                drugstoresMod.DrugstoreName = reader["drugstore"].ToString();
                drugstoresMod.Title = "MONTHLY REPORT FOR: " + drugstoresMod.DrugstoreName;
                lstDrugstore.Add(drugstoresMod);
                drugstoresMod = new DrugstoresSalesModel();
            }

            conDB.closeConnection();

            return lstDrugstore;
        }

        private List<DrugstoresSalesModel> getDrugstoreSalesAllProductPerDrugstoresYear()
        {
            conDB = new ConnectionDB();

            List<DrugstoresSalesModel> lstDrugstore = new List<DrugstoresSalesModel>();
            DrugstoresSalesModel drugstoresMod = new DrugstoresSalesModel();


            string queryString = "SELECT year(deliverydate) as deliverydate, sum(quantity) as total, " +
                "dbfh.tbldrugstores.description AS drugstore, dbfh.tblproducts.description AS product " +
                "FROM ((dbfh.tblsales INNER JOIN dbfh.tbldrugstores " +
                "ON dbfh.tblsales.drugstoreID = dbfh.tbldrugstores.ID) INNER JOIN dbfh.tblproducts " +
                "ON dbfh.tblsales.productID = dbfh.tblproducts.ID)  WHERE (deliverydate BETWEEN ? AND ?) AND " +
                "(dbfh.tblsales.isDeleted = 0) AND (dbfh.tblsales.drugstoreID = ?) GROUP BY year(deliverydate)";

            List<string> parameters = new List<string>();

            parameters.Add(reportDate.YearFrom);
            parameters.Add(reportDate.YearTo);
            parameters.Add(reportDate.DrugstoreID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                drugstoresMod.DeliveryDate = reader["deliverydate"].ToString();
                drugstoresMod.ProductName = reader["product"].ToString();
                drugstoresMod.Total = Convert.ToInt32(reader["total"].ToString());
                drugstoresMod.DrugstoreName = reader["drugstore"].ToString();
                drugstoresMod.Title = "YEARLY REPORT FOR: " + drugstoresMod.DrugstoreName;
                lstDrugstore.Add(drugstoresMod);
                drugstoresMod = new DrugstoresSalesModel();
            }

            conDB.closeConnection();

            return lstDrugstore;
        }

        private List<DrugstoresSalesModel> getDrugstoreSalesPerProductPerDrugstores()
        {
            conDB = new ConnectionDB();

            List<DrugstoresSalesModel> lstDrugstore = new List<DrugstoresSalesModel>();
            DrugstoresSalesModel drugstoresMod = new DrugstoresSalesModel();


            string queryString = "SELECT monthname(deliverydate) as deliverydate, sum(quantity) as total, " +
                "dbfh.tbldrugstores.description AS drugstore, dbfh.tblproducts.description AS product " +
                "FROM ((dbfh.tblsales INNER JOIN dbfh.tbldrugstores " +
                "ON dbfh.tblsales.drugstoreID = dbfh.tbldrugstores.ID) INNER JOIN dbfh.tblproducts " +
                "ON dbfh.tblsales.productID = dbfh.tblproducts.ID)  WHERE (deliverydate BETWEEN ? AND ?) AND " +
                "(dbfh.tblsales.isDeleted = 0) AND (dbfh.tblsales.drugstoreID = ?) AND (dbfh.tblsales.productID = ?) GROUP BY DATE(deliverydate) ORDER BY DATE(deliverydate) ASC";

            List<string> parameters = new List<string>();

            parameters.Add(reportDate.YearFrom);
            parameters.Add(reportDate.YearTo);
            parameters.Add(reportDate.DrugstoreID);
            parameters.Add(reportDate.ProductID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                drugstoresMod.DeliveryDate = reader["deliverydate"].ToString();
                drugstoresMod.ProductName = reader["product"].ToString();
                drugstoresMod.Total = Convert.ToInt32(reader["total"].ToString());
                drugstoresMod.DrugstoreName = reader["drugstore"].ToString();
                drugstoresMod.Title = "MONTHLY REPORT FOR: " + drugstoresMod.DrugstoreName;
                lstDrugstore.Add(drugstoresMod);
                drugstoresMod = new DrugstoresSalesModel();
            }

            conDB.closeConnection();

            return lstDrugstore;
        }

        private List<DrugstoresSalesModel> getDrugstoreSalesPerProductPerDrugstoresYear()
        {
            conDB = new ConnectionDB();

            List<DrugstoresSalesModel> lstDrugstore = new List<DrugstoresSalesModel>();
            DrugstoresSalesModel drugstoresMod = new DrugstoresSalesModel();


            string queryString = "SELECT year(deliverydate) as deliverydate, sum(quantity) as total, " +
                "dbfh.tbldrugstores.description AS drugstore, dbfh.tblproducts.description AS product " +
                "FROM ((dbfh.tblsales INNER JOIN dbfh.tbldrugstores " +
                "ON dbfh.tblsales.drugstoreID = dbfh.tbldrugstores.ID) INNER JOIN dbfh.tblproducts " +
                "ON dbfh.tblsales.productID = dbfh.tblproducts.ID)  WHERE (deliverydate BETWEEN ? AND ?) AND " +
                "(dbfh.tblsales.isDeleted = 0) AND (dbfh.tblsales.drugstoreID = ?) AND (dbfh.tblsales.productID = ?) GROUP BY year(deliverydate)";

            List<string> parameters = new List<string>();

            parameters.Add(reportDate.YearFrom);
            parameters.Add(reportDate.YearTo);
            parameters.Add(reportDate.DrugstoreID);
            parameters.Add(reportDate.ProductID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                drugstoresMod.DeliveryDate = reader["deliverydate"].ToString();
                drugstoresMod.ProductName = reader["product"].ToString();
                drugstoresMod.Total = Convert.ToInt32(reader["total"].ToString());
                drugstoresMod.DrugstoreName = reader["drugstore"].ToString();
                drugstoresMod.Title = "YEARLY REPORT FOR: " + drugstoresMod.DrugstoreName;
                lstDrugstore.Add(drugstoresMod);
                drugstoresMod = new DrugstoresSalesModel();
            }

            conDB.closeConnection();

            return lstDrugstore;
        }
    }
}
