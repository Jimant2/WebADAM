using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using webapi.DefinitionModels;
using webapi.Models;

namespace webapi.Services
{
    public interface IDeviceService
    {
        //public Task<List<Device>> GetAllDevicesFromService();

        public Task<List<string>> GetAllDeviceNamesFromService();

        public Task<Device> GetDeviceByNameFromService(string deviceName);
        public Task AddDeviceDefinitionFromService(Definition definition);
        public Task<List<GroupChannelDTO>> GetDefinitionByDeviceName(string deviceName, ClaimsPrincipal claimsPrincipal);

    }
}
