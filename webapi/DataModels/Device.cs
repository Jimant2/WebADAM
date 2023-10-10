using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace webapi.DataModels
{
    public class Device
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        public string? deviceName { get; set; }
        public string? valueType { get; set; }

    }
}
