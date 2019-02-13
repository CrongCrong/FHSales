using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSales.MongoClasses
{
    public class ProductsOrderedDS
    {

        public Products Products { get; set; }

        public string Qty { get; set; }

        public string Price { get; set; }
 
    }
}
