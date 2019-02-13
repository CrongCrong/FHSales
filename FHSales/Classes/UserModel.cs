using MongoDB.Bson;

namespace FHSales.Classes
{
    public static class UserModel
    {
        public static ObjectId Id { get; set; }

        public static string Username { get; set; }

        public static string Password { get; set; }

        public static string FirstName { get; set; }

        public static string LastName { get; set; }

        public static bool isDeleted { get; set; }

        public static bool isViewing { get; set; }

        public static bool isEditing { get; set; }

        public static byte[] bHash { get; set; }

        public static byte[] bSalt { get; set; }

        public static bool isPOAdmin { get; set; }

        public static bool isDSAdmin { get; set; }
    }
}
