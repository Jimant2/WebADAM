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
            Dictionary<string, List<string>> deviceData = new Dictionary<string, List<string>>();

            List<string> valueTypes = new List<string>();
            foreach (var numericChannel in definition.ChannelDefinition.Channels.NumericChannels)
            {
                valueTypes.Add(numericChannel.Name);
            }
            deviceData[definition.Name] = valueTypes;

            Device device = new Device
            {
                deviceName = definition.Name + " " + definition.Version,
                valueType = deviceData[definition.Name].ToArray(),
                channelXml = definition
            };
            await _repository.AddChannelXmlAsync(device);
        }
    }
}
