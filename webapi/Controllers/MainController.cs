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

    [HttpGet("dataSet")]
    public ActionResult<IEnumerable<DataSet>> GetDataSet()
    {
        var data = _repository.GetDataSet();
        return Ok(data);
    }

    [HttpGet("dataLoader")]
    public ActionResult<IEnumerable<DataLoader>> GetDataLoader()
    {
        var data = _repository.GetDataLoader();
        return Ok(data);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file, string deviceName, string dataType)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is not provided.");
        }

        if (string.IsNullOrEmpty(deviceName))
        {
            return BadRequest("Device name is required.");
        }

        if (string.IsNullOrEmpty(dataType))
        {
            return BadRequest("Data type is required.");
        }

        try
        {
            DateTime date1 = DateTime.Now;
            var currentTime = date1.AddHours(2);

            var existingDevice = await _repository.GetDeviceByNameAsync(deviceName);

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var jsonString = await reader.ReadToEndAsync();

                // Deserialize the JSON as an array
                var jsonArray = BsonSerializer.Deserialize<BsonArray>(jsonString);

                // Create a list to store all the DataSet documents
                var dataSetList = new List<DataSet>();

                // Iterate over each element in the array
                foreach (var jsonDocument in jsonArray)
                {
                    var time = jsonDocument["Time"].ToString();
                    var ecgWaveform = jsonDocument["EcgWaveform"].AsInt32;

                    var newDataDocument = new Data
                    {
                        timestamp = DateTime.UtcNow,
                        value = ecgWaveform
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

                // Insert all the DataSet documents in a single database call
                await _repository.InsertDataSetAsync(dataSetList);

                return Ok("File uploaded, and data added to the DataSet.");
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions here
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
