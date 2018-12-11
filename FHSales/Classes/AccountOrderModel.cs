using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSales.Classes
{
    public class AccountOrderModel
    {
        public string ID { get; set; }

        public string ClientID { get; set; }

        public string ClientName { get; set; }

        public string PaymentModeID { get; set; }

        public string PaymentMode { get; set; }

        public string DrNo { get; set; }

        public string DeliveryDate { get; set; }

        public string Terms { get; set; }

        public string PaymentDueDate { get; set; }

        public string Total { get; set; }

        public string isPaid { get; set; }

        public string RepID { get; set; }

        public string RepresentativeName { get; set; }
    }
}
