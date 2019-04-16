using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSales.MongoClasses
{
    public class PaymentsDrugstores
    {
        public ObjectId Id { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime PaymentDate { get; set; }

        public double Payment { get; set; }

        public bool isDeleted { get; set; }

        public Banks PaymentMode { get; set; }

        public string Notes { get; set; }

        [BsonIgnore]
        public string strDate { get; set; }
    }
}
