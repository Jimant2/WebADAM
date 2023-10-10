using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace webapi.DataModels
{
    public class DataLoader
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string? dataLoaderName { get; set; }
        public string? dataLoaderDescription { get; set; }

    }
}
