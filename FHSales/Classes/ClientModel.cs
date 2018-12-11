using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSales.Classes
{
    public class ClientModel
    {
        public string ID { get; set; }

        public string ClientID { get; set; }

        public string ClientName { get; set; }

        public string Address { get; set; }

        public string ContactNumber { get; set; }

        public string ContactPerson { get; set; }

        public bool isWithDispenser { get; set; }

        public string Qty { get; set; }

        public string Quota { get; set; }

        public string DRCount { get; set; }
    }
}
