using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace webapi.Models
{
    public class Device
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        public string? deviceName { get; set; }
        public string? deviceType { get; set; }

    }
}
