﻿using webapi.DefinitionModels;
using webapi.Models;

namespace webapi.DataRepos
{
    public interface IWebADAMRepo
    {
        public List<DataLoader> GetDataLoader();
        public Task<Device> GetDeviceByName(string deviceName);
        public Task<List<Device>> GetAllDevicesAsync();
        public Task<Device> GetDeviceByNameAsync(string deviceName);
        public List<DataSet> GetDataSet();
        public Task<List<DataSet>> GetAllDataSetsAsync();
        public Task InsertDataSetAsync(List<DataSet> dataSet);
        public Task<List<DataSet>> GetDataSetsByDeviceNameAsync(string deviceName);
        public Task AddLicenseXmlAsync(Users user);
        public Task<Device> GetDeviceByValueTypeAsync(string valueType);
        public Task<Users> FindByUsernameAsync(string username);
        public Task AddChannelXmlAsync(Device deviceDefinition);
        public Task<Definition> GetDefinitionByDeviceNameAsync(string deviceName);
    }
}
