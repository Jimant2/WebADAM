using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using webapi.Models;

namespace webapi.DataRepos
{
    public class WebADAMRepo : IWebADAMRepo
    {
        private readonly IMongoCollection<Device> deviceCollection;
        private readonly IMongoCollection<DataSet> dataSetCollection;
        private readonly IMongoCollection<DataLoader> dataLoaderCollection;
        private readonly IMongoCollection<Users> usersCollection;

        public WebADAMRepo(IMongoDatabase database)
        {
            deviceCollection = database.GetCollection<Device>("Device");
            dataSetCollection = database.GetCollection<DataSet>("DataSet");
            dataLoaderCollection = database.GetCollection<DataLoader>("DataLoader");
            usersCollection = database.GetCollection<Users>("Users");
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

        public async Task<Device> GetDeviceByValueTypeAsync(string valueType)
        {
            return await deviceCollection.Find(d => d.valueType == valueType).FirstOrDefaultAsync();
        }

        public List<DataSet> GetDataSet()
        {
            return dataSetCollection.Find(_ => true).ToList();
        }
        public async Task<List<DataSet>> GetAllDataSetsAsync()
        {
            return await dataSetCollection.Find(_ => true).ToListAsync();
        }

        public async Task AddLicenseXmlAsync(Users user)
        {
            await usersCollection.InsertOneAsync(user);
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
