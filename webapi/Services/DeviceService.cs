using webapi.DefinitionModels;
using webapi.DataRepos;
using webapi.Models;

namespace webapi.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IWebADAMRepo _repository;

        public DeviceService(IWebADAMRepo repository) 
        {
        _repository = repository;
        }

        public List<Device> GetDeviceNameFromService()
        {
            return _repository.GetDevice();
        }

        public async Task<List<Device>> GetAllDevicesFromService()
        {
            return await _repository.GetAllDevicesAsync();
        }
        public async Task<Device> GetDeviceByNameFromService(string deviceName)
        {
            return await _repository.GetDeviceByNameAsync(deviceName);
        }
        public async Task AddDeviceDefinitionFromService(Definition definition)
        {
            Device device = new Device
            {
                deviceName = definition.Name,
                //TODO: Implement channel extracter method to value type
              //  valueType = definition.
                channelXml = definition
            };
             await _repository.AddChannelXmlAsync(device);
        }
    }
}
