using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using webapi.LicenseModels;

namespace webapi.Models
{
    public class Users
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public License LicenseXml { get; set; }
    }
}
