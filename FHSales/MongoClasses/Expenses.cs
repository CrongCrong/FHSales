using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSales.MongoClasses
{
    public class Expenses
    {

        public ObjectId Id { get; set; }

        public string ExpensesValue { get; set; }

        public string Notes { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateRecorded { get; set; }

        public bool isDeleted { get; set; }

        [BsonIgnore]
        public string strDateRecorded { get; set; }

    }
}
