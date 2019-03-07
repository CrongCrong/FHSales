

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FHSales.MongoClasses
{
    public class Products
    {
        public ObjectId Id { get; set; }

        public string ProductName { get; set; }

        public string Description { get; set; }

        public double Price { get; set; }

        public bool isDeleted { get; set; }

        [BsonIgnore]
        public int Qty { get; set; }
    }
}
