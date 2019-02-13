using MongoDB.Bson;


namespace FHSales.MongoClasses
{
    public class ProductsOrdered
    {
        public ObjectId Id { get; set; }

        public ObjectId OrderID { get; set; }

        public string ProductName { get; set; }

        public int Qty { get; set; }

        public double Price { get; set; }

        public double Total { get; set; }

        public string Remarks { get; set; }

        public bool isDeleted { get; set; }
    }
}
