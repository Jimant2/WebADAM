using MongoDB.Bson.Serialization.Attributes;

namespace webapi.DataModels
{
    public class Data
    {
        [BsonElement("timestamp")]
        public DateTime timestamp { get; set; }
        [BsonElement("value")]
        public int value { get; set; }
    }
}
