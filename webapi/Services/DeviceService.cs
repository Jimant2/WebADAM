using webapi.DefinitionModels;
using webapi.DataRepos;
using webapi.Models;
using webapi.Services;
using System;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace webapi.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IWebADAMRepo _repository;
        private readonly IAuthService _authService;

        public DeviceService(IWebADAMRepo repository, IAuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }

        //public async Task<List<Device>> GetAllDevicesFromService()
        //{
        //    return await _repository.GetAllDevicesAsync();
        //}

        public async Task<List<string>> GetAllDeviceNamesFromService()
        {
            var devices = await _repository.GetAllDevicesAsync();
            return devices.Select(device => device.deviceName).ToList();
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

        //public async Task<List<GroupChannelDTO>> GetDefinitionByDeviceName(string deviceName, ClaimsPrincipal claimsPrincipal)
        //{
        //    var definition = await _repository.GetDefinitionByDeviceNameAsync(deviceName);

        //    var definitionGroups = new List<GroupChannelDTO>();

        //    foreach (var group in definition.ChannelDefinition.Groups)
        //    {
        //         var user = await _repository.FindByUsernameAsync(claimsPrincipal.Identity.Name);

        //            var dto = new GroupChannelDTO
        //            {
        //                GroupName = group.Name,
        //                Channels = _authService.GetAuthorizedChannels(group.Channels, user.LicenseXml)
        //            };
        //            definitionGroups.Add(dto);
        //    }

        //   return definitionGroups;
        //}
        public async Task<List<GroupChannelDTO>> GetDefinitionByDeviceName(string deviceName, ClaimsPrincipal claimsPrincipal)
        {
            var definition = await _repository.GetDefinitionByDeviceNameAsync(deviceName);

            var definitionGroups = new List<GroupChannelDTO>();

            foreach (var group in definition.ChannelDefinition.Groups)
            {
                var user = await _repository.FindByUsernameAsync(claimsPrincipal.Identity.Name);

                var dto = new GroupChannelDTO
                {
                    GroupName = group.Name,
                };

                if (claimsPrincipal.IsInRole("Administrator"))
                {
                    dto.Channels = group.Channels.ToList();
                }
                else
                {
                    dto.Channels = _authService.GetAuthorizedChannels(group.Channels, user.LicenseXml);
                }

                definitionGroups.Add(dto);
            }

            return definitionGroups;
        }


    }
}
