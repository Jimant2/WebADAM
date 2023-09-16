using MongoDB.Driver;
using webapi.Models;

namespace webapi
{
    public class WebADAMDBRepo
    {
        private readonly IMongoCollection<Device> deviceCollection;

        public WebADAMDBRepo(IMongoDatabase database)
        {
            deviceCollection = database.GetCollection<Device>("Device");
        }

        public List<Device> GetAll()
        {
            return deviceCollection.Find(_ => true).ToList();
        }

        // Implement other CRUD methods as needed
    }
}
