using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSales.MongoClasses
{
    public class DirectSalesDaily
    {
        public ObjectId Id { get; set; }

        public Clients Client { get; set; }

        public Banks Bank { get; set; }

        public Couriers Courier { get; set; }       

        public string Expenses { get; set; }

        public string Total { get; set; }

        public string Remarks { get; set; }

        public string TrackingNo { get; set; }

        public DateTime DateOrdered { get; set; }

        public string FreePads { get; set; }

        public string FreeJuice { get; set; }

        public bool isDeleted { get; set; }

        public bool isConsolidated { get; set; }

        public List<ProductsOrderedDS> ProductsOrdered { get; set; }

        [BsonIgnore]
        public string strDateOrdered { get; set; }

        [BsonIgnore]
        public string strFullName { get; set; }

        [BsonIgnore]
        public string strClientFullName { get; set; }

        [BsonIgnore]
        public string strBankName { get; set; }

        [BsonIgnore]
        public string strCourierName { get; set; }

        [BsonIgnore]
        public string strTotalBoxes { get; set; }

        [BsonIgnore]
        public string strTotalGuava { get; set; }

        [BsonIgnore]
        public string strTotalGuyabano { get; set; }


    }
}
