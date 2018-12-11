using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSales.Classes
{
    public class FHJuiceProductModel
    {
        public string ID { get; set; }

        public string ProductName { get; set; }

        public string Description { get; set; }

        public string Qty { get; set; }

        public string Total { get; set; }

        public string Price { get; set; }

        public string OrderID { get; set; }

        public string RecordID { get; set; }

        public bool NewlyAdded { get; set; }

        public string Remarks { get; set; }

        public string StockDate { get; set; }
    }
}
