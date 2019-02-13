using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSales.MongoClasses
{
    public class Area
    {

        public ObjectId Id { get; set; }

        public string AreaName { get; set; }

        public List<SubArea> SubArea { get; set; }

        public bool isDeleted { get; set; }
    }
}
