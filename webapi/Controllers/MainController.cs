using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Text;
using webapi.Models;
using webapi.Services;
using webapi.LicenseModels;
using System.Xml.Serialization;
using webapi.DefinitionModels;


namespace webapi.Controllers;

[ApiController]
[Route("MainController")]
public class MainController : ControllerBase
{
    private ILicenseService licenseService { get; set; }
    private IDeviceService deviceService { get; set; }
    private IDataService dataService { get; set; }

    public MainController(ILicenseService _licenseService, IDeviceService _deviceService, IDataService _dataService)
    {
        licenseService = _licenseService;
        deviceService = _deviceService;
        dataService = _dataService;
    }

    [HttpPost]
    [Route ("addDeviceDefinitions")]
    public ActionResult<Device> AddDeviceDefinitions(IFormFile deviceFile)
    {
        try
        {
            if (deviceFile != null && deviceFile.Length > 0 && deviceFile.ContentType == "text/xml")
            {
                using (var streamReader = new StreamReader(deviceFile.OpenReadStream()))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Definition));
                    Definition definition = (Definition)serializer.Deserialize(streamReader);

                    deviceService.AddDeviceDefinitionFromService(definition);
                    return Ok("XML uploaded successfully");
                }
            }
            return BadRequest("Invalid or missing XML file.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpGet]
    [Route("dataSetByDataType/{dataType}")]
    public async Task<ActionResult<IEnumerable<DataSet>>> GetDataSetsByDataType(string dataType)
    {
        try
        {
            var data = await dataService.GetDataSetsByDataTypeFromService(dataType);
            Console.WriteLine($"Retrieved {data.Count} datasets for dataType: {dataType}");

            return Ok(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Route("timestampByDataType/{dataType}")]
    public async Task<ActionResult<IEnumerable<DateTime>>> GetTimestampByDataType(string dataType)
    {
        try
        {
            var data = await dataService.GetTimestampsByDataTypeFromServiceAsync(dataType);

            return Ok(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Route("dataSetByTimestamp/{timestamp}")]
    public async Task<ActionResult<IEnumerable<DataSet>>> GetDataSetByTimestamp(DateTime timestamp)
    {
        try
        {
            var data = await dataService.GetDataSetByTimestampFromServiceAsync(timestamp);

            return Ok(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("uploadFile")]
    public async Task<IActionResult> UploadFile(IFormFile file, string dataType)
    {
        Console.WriteLine($"Received file: {file?.FileName}");
        Console.WriteLine($"Received dataType: {dataType}");
        try { 
        await dataService.UploadFileFromService(file, dataType);
        return Ok("File uploaded");
        }
        catch (Exception ex) 
        { 
            Console.WriteLine(ex.Message );
            return BadRequest(ex.Message );
        }
    }

    [HttpPost]
    [Route("uploadLicense")]
    public async Task<IActionResult> UploadLicense(IFormFile licenseFile)
    {
        try
        {
            if (licenseFile != null && licenseFile.Length > 0 && licenseFile.ContentType == "text/xml")
            {
                using (var streamReader = new StreamReader(licenseFile.OpenReadStream()))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(License));
                    License license = (License)serializer.Deserialize(streamReader);

                    await licenseService.SaveLicensedUserFromService(license);
                    return Ok("XML uploaded successfully");
                }
            }
            return BadRequest("Invalid or missing XML file.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpGet("exportData/{dataType}")]
    public async Task<IActionResult> ExportData(string dataType)
    {
        try
        {
            if (string.IsNullOrEmpty(dataType))
            {
                return BadRequest("Data type name is required.");
            }

            var dataSets = await dataService.GetDataSetsByDataTypeFromService(dataType);

            if (!dataSets.Any())
            {
                return NotFound($"No data found for data type named {dataType}.");
            }
          
            var jsonString = System.Text.Json.JsonSerializer.Serialize(dataSets);

            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);

            return File(jsonBytes, "application/json", $"{dataType}_dataSets.json");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpGet("getGroupsAndChannels")]
    public async Task<IActionResult> GetGroupsAndChannels(string deviceName)
    {
        if (string.IsNullOrEmpty(deviceName))
        {
            return BadRequest("Device name is required.");
        }

        var requestingUser = HttpContext.User;
        var groupsAndChannels = await deviceService.GetDefinitionByDeviceName(deviceName, requestingUser);

        return Ok(groupsAndChannels);
    }
    [HttpGet("getAllDeviceNames")]
    public async Task<IActionResult> GetAllDeviceNames()
        {
        try
        {
            var deviceNames = await deviceService.GetAllDeviceNamesFromService();
            if (!deviceNames.Any())
            {
                return NotFound("No Device found");
            }
            return Ok(deviceNames);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, "Internal server error.");
        }
    }
    //THIS IS BEING USED IN THE API CONTROLLER TO RETRIEVE NAMES FOR THE LIST IN THE PROJECT TYPES
    [HttpGet("getAllDevices")]
    public async Task<IActionResult> GetAllDevices()
    {
        try
        {
            var device = await deviceService.GetAllDeviceNamesFromService();
            if (!device.Any())
            {
                return NotFound("Device Not Found");
            }
            return Ok(device);
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, "Internal server error.");
        }
    }
    [HttpGet("getDeviceByName")]
    public async Task<IActionResult> GetDeviceByName(string deviceName)
    {
        try
        {
            var device = await deviceService.GetDeviceByNameFromService(deviceName);
            if (device == null)
            {
                return NotFound("Device Not Found");
            }
            return Ok(device);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, "Internal server error.");
        }
    }
}
