using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using webapi.Models;
using webapi.DefinitionModels;
using MongoDB.Bson;

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

        public async Task<Device> GetDeviceByName(string deviceName)
        {
             return await deviceCollection.Find(device => device.deviceName == deviceName).FirstOrDefaultAsync();
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
            return await deviceCollection.Find(d => d.valueType.Contains(valueType)).FirstOrDefaultAsync();
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

        public async Task AddChannelXmlAsync(Device deviceDefinition)
        {
            await deviceCollection.InsertOneAsync(deviceDefinition);
        }

        public async Task InsertDataSetAsync(List<DataSet> dataSet)
        {
            await dataSetCollection.InsertManyAsync(dataSet);
        }

        public async Task<List<DataSet>> GetDataSetsByDataTypeAsync(string dataType)
        {
            var lowerCaseDataType = dataType.ToLower();
            var devices = await deviceCollection.Find(d => d.valueType.Any(vt => vt.ToLower() == lowerCaseDataType)).ToListAsync();

            if (devices == null || devices.Count == 0)
            {
                return new List<DataSet>();
            }

            var deviceIds = devices.Select(d => d._id);
            var dataSets = await dataSetCollection.Find(ds => ds.dataType.ToLower() == lowerCaseDataType).ToListAsync();

            return dataSets;
        }

        public async Task<IEnumerable<DateTime>> GetTimestampsByDataTypeAsync(string dataType)
        {
            var filter = Builders<DataSet>.Filter.Eq("dataType", dataType);

            var dataSetList = await dataSetCollection.Find(filter).ToListAsync();

            var uniqueTimestamps = dataSetList.Select(d => d.timestamp).Distinct();

            return uniqueTimestamps;
        }

        public async Task<IEnumerable<DataSet>> GetDataSetsByTimestampAsync(DateTime timestamp)
        {
            var filter = Builders<DataSet>.Filter.Eq("timestamp", timestamp);

            var dataSetList = await dataSetCollection.Find(filter).ToListAsync();

            return dataSetList;
        }


        public async Task<Users> FindByUsernameAsync(string username)
        {
            return await usersCollection.Find(u => u.Username == username).FirstOrDefaultAsync();
        }
        public async Task<Definition> GetDefinitionByDeviceNameAsync(string deviceName)
        {
            var filter = Builders<Device>.Filter.Eq(d => d.deviceName, deviceName);
            var device = await deviceCollection.Find(filter).FirstOrDefaultAsync();

            return device?.channelXml;
        }
    }
}
