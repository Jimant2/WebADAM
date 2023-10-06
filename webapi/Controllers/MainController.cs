using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Globalization;
using System.Text.Json;
using System.Text;
using webapi.DataRepos;
using webapi.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Swashbuckle.AspNetCore.SwaggerGen;
using Newtonsoft.Json.Linq;

namespace webapi.Controllers;

[ApiController]
[Route("MainController")]
public class MainController : ControllerBase
{
    private readonly WebADAMDBRepo _repository;

    public MainController(WebADAMDBRepo repository)
    {
        _repository = repository;
    }

    [HttpGet("device")]
    public ActionResult<IEnumerable<Device>> GetDevice()
    {
        var data = _repository.GetDevice();
        return Ok(data);
    }

    [HttpGet("dataSetByName/{deviceName}")]
    public async Task<ActionResult<IEnumerable<DataSet>>> GetDataSet(string deviceName)
    {
        var data = await _repository.GetDataSetsByDeviceNameAsync(deviceName);
        return Ok(data);
    }

    [HttpGet("dataLoader")]
    public ActionResult<IEnumerable<DataLoader>> GetDataLoader()
    {
        var data = _repository.GetDataLoader();
        return Ok(data);
    }


    [HttpPost("uploadFile")]
    [RequestSizeLimit(100_000_000)]
    public async Task<IActionResult> UploadFile(IFormFile file, string deviceName, string dataType)
    {

        Console.WriteLine($"Received file: {file?.FileName}");
        Console.WriteLine($"Received deviceName: {deviceName}");
        Console.WriteLine($"Received dataType: {dataType}");

        try
        {
            DateTime date1 = DateTime.Now;
            var currentTime = date1.AddHours(2);

            var existingDevice = await _repository.GetDeviceByNameAsync(deviceName);

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var jsonString = await reader.ReadToEndAsync();

                // Parse JSON using JArray 
                var jsonArray = JArray.Parse(jsonString);

                var dataSetList = new List<DataSet>();

                // Iterate over elements in the JSON array
                foreach (var jsonDocument in jsonArray)
                {
                    // Convert JSON object to dictionary
                    var properties = jsonDocument.ToObject<Dictionary<string, object>>();

                    // Iterate through the properties and find the first integer value
                    int value = 0;
                    foreach (var property in properties.Values)
                    {
                        if (property is int intValue)
                        {
                            value = intValue;
                            break;
                        }
                        else if (property is long longValue)
                        {
                            value = (int)longValue;
                            break;
                        }
                        else if (property is double doubleValue)
                        {
                            value = (int)doubleValue;
                            break;
                        }
                    }

                    // Create a new Data object
                    var newDataDocument = new Data
                    {
                        timestamp = DateTime.UtcNow,
                        value = value
                    };

                    var newDataSetDocument = new DataSet
                    {
                        deviceId = existingDevice._id,
                        timestamp = currentTime,
                        dataType = dataType,
                        Data = new List<Data> { newDataDocument }
                    };

                    // Add the new DataSet document to the list
                    dataSetList.Add(newDataSetDocument);
                }

                await _repository.InsertDataSetAsync(dataSetList);

                return Ok("File uploaded, and data added to the DataSet.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            return StatusCode(500, "Internal server error. Please check server logs for more details.");
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
            var dataSets = await _repository.GetDataSetsByDeviceNameAsync(deviceName);

            if (!dataSets.Any())
            {
                return NotFound($"No data found for device named {deviceName}.");
            }

            // Serialize the data to JSON
            var jsonString = JsonSerializer.Serialize(dataSets);

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
