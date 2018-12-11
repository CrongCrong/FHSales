using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSales.Classes
{
    public class StocksInventoryModel
    {

        public string ID { get; set; }

        public string TollpackerName { get; set; }

        public string TollpackerID { get; set; }

        public string StocksDescription { get; set; }

        public string Qty { get; set; }

        public string QtyIn { get; set; }

        public string QtyOut { get; set; }

        public string QtyNet { get; set; }

        public string DeliveryDate { get; set; }

        public string ProductName { get; set; }

        public string ProductID { get; set; }

        public string Paid { get; set; }

        public string DRNo { get; set; }

        public string Amount { get; set; }

        public string Terms { get; set; }

        public string PaymentDueDate { get; set; }
    }
}
