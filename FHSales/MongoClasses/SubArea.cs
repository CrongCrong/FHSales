using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSales.MongoClasses
{
    public class SubArea
    {

        public ObjectId Id { get; set; }

        public string SubAreaName { get; set; }

        public bool isDeleted { get; set; }

    }
}
