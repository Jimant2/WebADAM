using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace webapi.Models
{
    public class ECG
    {
        [BsonId]
        public ObjectId _id { get; set; }
        [BsonElement("Time")]
        public string Time {  get; set; }
        [BsonElement("EcgWaveform")]
        public int EcgWaveform { get; set; }
    }
}
