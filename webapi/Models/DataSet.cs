using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace webapi.Models
{
    public class DataSet
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string deviceId { get; set; }

        public DateTime timestamp { get; set; }

        public List<Data>? Data {  get; set; }  
    }
}
