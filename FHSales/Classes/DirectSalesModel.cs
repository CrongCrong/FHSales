namespace FHSales.Classes
{
    public class DirectSalesModel
    {
        public string ID { get; set; }

        public string DeliveryDate { get; set; }

        public string ClientName { get; set; }

        public string Quantity { get; set; }

        public string CashBankID { get; set; }

        public string CashBankName { get; set; }

        public string CourierID { get; set; }

        public string CourierName { get; set; }

        public int Expenses { get; set; }

        public int TotalPrice { get; set; }

        public string Title  { get; set; }
    }
}
