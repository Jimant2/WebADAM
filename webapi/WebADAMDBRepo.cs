using MongoDB.Driver;
using webapi.Models;

namespace webapi
{
    public class WebADAMDBRepo
    {
        private readonly IMongoCollection<Device> deviceCollection;
        private readonly IMongoCollection<DataSet> dataSetCollection;
        private readonly IMongoCollection<DataLoader> dataLoaderCollection;

        public WebADAMDBRepo(IMongoDatabase database)
        {
            deviceCollection = database.GetCollection<Device>("Device");
            dataSetCollection = database.GetCollection<DataSet>("DataSet");
            dataLoaderCollection = database.GetCollection<DataLoader>("DataLoader");
        }

        public List<Device> GetDevice()
        {
            return deviceCollection.Find(_ => true).ToList();
        }

        public List<DataSet> GetDataSet()
        {
            return dataSetCollection.Find(_ => true).ToList();
        }

        public List<DataLoader> GetDataLoader()
        {
            return dataLoaderCollection.Find(_ => true).ToList();
        }
        // Implement other CRUD methods as needed
    }
}
