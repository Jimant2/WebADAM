using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace webapi.Models
{
    public class DataSet
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public int deviceId { get; set; }

        public DateTime timestamp { get; set; }

        public List<Data>? dataList {  get; set; }  
    }
}
