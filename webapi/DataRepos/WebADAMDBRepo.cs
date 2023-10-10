using MongoDB.Driver;
using webapi.DataModels;

namespace webapi.DataRepos
{
    public class WebADAMDBRepo : IWebADAMRepo
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


        public List<DataLoader> GetDataLoader()
        {
            return dataLoaderCollection.Find(_ => true).ToList();
        }

        public List<Device> GetDevice()
        {
            return deviceCollection.Find(_ => true).ToList();
        }

        public async Task<List<Device>> GetAllDevicesAsync()
        {
            return await deviceCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Device> GetDeviceByNameAsync(string deviceName)
        {
            return await deviceCollection.Find(d => d.deviceName == deviceName).FirstOrDefaultAsync();
        }

        public List<DataSet> GetDataSet()
        {
            return dataSetCollection.Find(_ => true).ToList();
        }
        public async Task<List<DataSet>> GetAllDataSetsAsync()
        {
            return await dataSetCollection.Find(_ => true).ToListAsync();
        }

        public async Task InsertDataSetAsync(List<DataSet> dataSet)
        {
            await dataSetCollection.InsertManyAsync(dataSet);
        }

        public async Task<List<DataSet>> GetDataSetsByDeviceNameAsync(string deviceName)
        {
            var device = await deviceCollection.Find(d => d.deviceName == deviceName).FirstOrDefaultAsync();
            if (device == null) return new List<DataSet>();

            return await dataSetCollection.Find(ds => ds.deviceId == device._id).ToListAsync();
        }
    }
}
