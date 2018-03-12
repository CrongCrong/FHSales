
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSales.Classes
{
    public class PurchaseOrderModel
    {

        public string ID { get; set; }

        public string PONumber { get; set; }

        public string SINumber { get; set; }

        public string DRNumber { get; set; }

        public string PaymentDate { get; set; }

        public string DeliveryDate { get; set; }

        public string Quantity { get; set; }

        public DrugstoreModel Drugstore { get; set; }

        public ProductModel Product { get; set; }

        public string DrugstoreID { get; set; }

        public string ProductID { get; set; }
    }
}
