using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using webapi.DefinitionModels;

namespace webapi.Models
{
    public class Device
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        public string? deviceName { get; set; }
        public string[]? valueType { get; set; }
        public Definition channelXml { get; set; }
    }
}
