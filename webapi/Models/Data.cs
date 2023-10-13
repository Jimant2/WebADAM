using MongoDB.Bson.Serialization.Attributes;

namespace webapi.Models
{
    public class Data
    {
        [BsonElement("timestamp")]
        public DateTime timestamp { get; set; }
        [BsonElement("value")]
        public int value { get; set; }
    }
}
