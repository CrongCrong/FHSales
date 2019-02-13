using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSales.MongoClasses
{
    public class Users
    {
        public ObjectId Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool isDeleted { get; set; }

        public bool isViewing { get; set; }

        public bool isEditing { get; set; }

        public byte[] bHash { get; set; }

        public byte[] bSalt { get; set; }

        public bool isPOAdmin { get; set; }

        public bool isDSAdmin { get; set; }
    }
}
