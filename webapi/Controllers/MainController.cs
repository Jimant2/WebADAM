using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Globalization;
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
    public async Task<IActionResult> UploadFile(IFormFile file, string deviceName, string dataType) // Added dataType parameter
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
            // Converting from GMT to current Danish time due to system locale
            DateTime date1 = DateTime.Now;
            var currentTime = date1.AddHours(2);

            var existingDevice = await _repository.GetDeviceByNameAsync(deviceName);

            // Assuming the uploaded file contains the JSON data in string format
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var jsonString = await reader.ReadToEndAsync();

                // Log the JSON content for debugging purposes
                Console.WriteLine(jsonString);

                // Deserialize the JSON as an array
                var jsonArray = BsonSerializer.Deserialize<BsonArray>(jsonString);

                // Iterate over each element in the array
                foreach (var jsonDocument in jsonArray)
                {
                    // Extract Time and EcgWaveform from each JSON document
                    var time = jsonDocument["Time"].ToString();
                    var ecgWaveform = jsonDocument["EcgWaveform"].AsInt32;

                    // Create a new Data document for each element
                    var newDataDocument = new Data
                    {
                        timestamp = DateTime.UtcNow, // Set timestamp as current UTC time
                        value = ecgWaveform
                    };

                    // Create a new DataSet document for each element
                    var newDataSetDocument = new DataSet
                    {
                        deviceId = existingDevice._id,
                        timestamp = currentTime, // Set the time the file has been uploaded to the DB
                        dataType = dataType, // Set the dataType from the parameter
                        Data = new List<Data> { newDataDocument }
                    };

                    // Insert the new DataSet document into the DataSet collection
                    await _repository.InsertDataSetAsync(newDataSetDocument);
                }

                return Ok("File uploaded, and data added to the DataSet.");
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions here
            return StatusCode(500, "Internal server error.");
        }
    }



}