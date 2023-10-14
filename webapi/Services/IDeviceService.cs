using webapi.Models;

namespace webapi.Services
{
    public interface IDeviceService
    {

        public List<Device> GetDeviceNameFromService();

        public Task<List<Device>> GetAllDevicesFromService();

        public Task<Device> GetDeviceByNameFromService(string deviceName);
    }
}
