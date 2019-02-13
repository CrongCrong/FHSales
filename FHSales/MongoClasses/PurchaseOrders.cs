using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSales.MongoClasses
{
    public class PurchaseOrders
    {
        public ObjectId Id { get; set; }

        public string BatchNo { get; set; }

        public string PONumber { get; set; }

        public string SINumber { get; set; }

        public string DRNumber { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ExpiryDate { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime PaymentDueDate { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime PaymentDate { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DeliveryDate { get; set; }

        public string Qty { get; set; }

        public Drugstores Drugstore { get; set; }

        public Products Product { get; set; }

        [BsonRepresentation(BsonType.Decimal128)]
        public double Amount { get; set; }

        public bool isPaid { get; set; }

        public bool isDeleted { get; set; }

        [BsonIgnore]
        public string strPaymentDueDate { get; set; }

        [BsonIgnore]
        public string strPaymentDate { get; set; }

        [BsonIgnore]
        public string strDeliveryDate { get; set; }

        [BsonIgnore]
        public string strExpiryDate { get; set; }

        [BsonIgnore]
        public string DrugstoreName { get; set; }

        [BsonIgnore]
        public string ProductName { get; set; }

        [BsonIgnore]
        public string Terms { get; set; }

        [BsonIgnore]
        public string strAmount { get; set; }
    }
}
