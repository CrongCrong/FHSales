using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSales.MongoClasses
{
    public class DirectSales
    {

        public ObjectId Id { get; set; }

        public string ClientName { get; set; }

        public Banks Bank { get; set; }

        public Couriers Courier { get; set; }

        public SalesOffices SalesOffice { get; set; }

        public string Total { get; set; }

        public DateTime DeliveryDate { get; set; }

        public string Expenses { get; set; }

        public bool isPaid { get; set; }

        public bool isDeleted { get; set; }

        public string Remarks { get; set; }

    }
}
