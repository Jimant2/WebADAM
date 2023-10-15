using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Globalization;
using System.Text.Json;
using System.Text;
using webapi.Models;
using webapi.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Swashbuckle.AspNetCore.SwaggerGen;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Xml;
using webapi.LicenseModels;
using System.Xml.Serialization;
using System.Security.Cryptography;


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

    [HttpGet("device")]
    public ActionResult<List<Device>> GetDevice()
    {
       var device = deviceService.GetDeviceNameFromService();
       return Ok(device);
}

    [HttpGet("dataSetByName/{deviceName}")]
    public async Task<ActionResult<IEnumerable<DataSet>>> GetDataSet(string deviceName)
    {
        var data = await dataService.GetDataSetsByDeviceNameFromService(deviceName);
        return Ok(data);
    }

    //[HttpGet("dataLoader")]
    //public ActionResult<IEnumerable<DataLoader>> GetDataLoader()
    //{
    //    var data = _repository.GetDataLoader();
    //    return Ok(data); 
    //}

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
            // Check if a file is uploaded
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

    [HttpGet("exportData/{deviceName}")]
    public async Task<IActionResult> ExportData(string deviceName)
    {
        try
        {
            if (string.IsNullOrEmpty(deviceName))
            {
                return BadRequest("Device name is required.");
            }

            // Retrieve the data from the database based on deviceName
            var dataSets = await dataService.GetDataSetsByDeviceNameFromService(deviceName);

            if (!dataSets.Any())
            {
                return NotFound($"No data found for device named {deviceName}.");
            }

            // Serialize the data to JSON
            var jsonString = System.Text.Json.JsonSerializer.Serialize(dataSets);

            // Create a byte array of the JSON string
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);

            // Return as a file with a device-specific name
            return File(jsonBytes, "application/json", $"{deviceName}_dataSets.json");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error.");
        }
    }



}
