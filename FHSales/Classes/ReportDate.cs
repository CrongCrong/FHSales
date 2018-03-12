namespace FHSales.Classes
{
    public class ReportDate
    {
        public string DateReport { get; set; }

        public string MonthFrom { get; set; }

        public string MonthTo { get; set; }

        public string YearFrom { get; set; }

        public string YearTo { get; set; }

        public string ReportType { get; set; }

        public bool  ifAllDrugstore { get; set; }

        public bool ifAllProducts { get; set; }

        public string DrugstoreID { get; set; }

        public string ProductID { get; set; }
    }
}
