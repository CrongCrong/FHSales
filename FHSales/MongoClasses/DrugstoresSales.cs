using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSales.MongoClasses
{
    public class DrugstoresSales
    {
        public ObjectId Id { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DeliveryDate { get; set; }

        public string Drugstorename { get; set; }

        public Agents Agent { get; set; }

        public Area Areas { get; set; }

        public string SubArea { get; set; }

        public Products Product { get; set; }

        public string Quantity { get; set; }

        public bool isDeleted { get; set; }

        public double Total { get; set; }

        public List<ProductsOrdered> ProductsOrdered { get; set; }

        [BsonIgnore]
        public string strDate { get; set; }
    }
}
