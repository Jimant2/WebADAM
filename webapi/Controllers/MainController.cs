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

    //[HttpPost("upload")]
    //public async Task<IActionResult> UploadFile(IFormFile file, string deviceName)
    //{
    //    if (file == null || file.Length == 0)
    //    {
    //        return BadRequest("File is not provided.");
    //    }

    //    if (string.IsNullOrEmpty(deviceName))
    //    {
    //        return BadRequest("Device name is required.");
    //    }

    //    try
    //    {
    //        // Assuming the uploaded file contains the JSON data in string format
    //        using (var reader = new StreamReader(file.OpenReadStream()))
    //        {
    //            //Converting from GMT to current Danish time due to system locale
    //            DateTime date1 = DateTime.Now;
    //            var currentTime = date1.AddHours(2);

    //            var jsonString = await reader.ReadToEndAsync();
    //            var jsonDocument = BsonDocument.Parse(jsonString);

    //            var existingDevice = await _repository.GetDeviceByNameAsync(deviceName);

    //            if (existingDevice != null)
    //            {
    //                // Convert BsonArray to List<Data>
    //                var dataBsonArray = jsonDocument.GetValue("Data").AsBsonArray;
    //                var dataList = dataBsonArray.Select(d => new Data
    //                {
    //                    timestamp = d["timestamp"].ToUniversalTime(),
    //                    value = d["value"].AsInt32
    //                }).ToList();

    //                // Create a new data document and set the deviceId reference
    //                var newDataDocument = new DataSet
    //                {
    //                    timestamp = currentTime,
    //                    deviceId = existingDevice._id, // Assuming DeviceId is the reference field
    //                    Data = dataList
    //                };

    //                // Insert the new data document into the DataSet collection
    //                await _repository.InsertDataSetAsync(newDataDocument);

    //                return Ok("File uploaded and associated with the device.");
    //            }
    //            else
    //            {
    //                return BadRequest("Device with the specified name does not exist.");
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        // Handle exceptions here
    //        return StatusCode(500, "Internal server error.");
    //    }



    //    // Implement other controller actions as needed
    //}

    //[HttpPost("upload")]
    //public async Task<IActionResult> UploadFile(IFormFile file, string deviceName)
    //{
    //    if (file == null || file.Length == 0)
    //    {
    //        return BadRequest("File is not provided.");
    //    }

    //    if (string.IsNullOrEmpty(deviceName))
    //    {
    //        return BadRequest("Device name is required.");
    //    }

    //    try
    //    {
    //        // Assuming the uploaded file contains the JSON data in string format
    //        using (var reader = new StreamReader(file.OpenReadStream()))
    //        {
    //            var jsonString = await reader.ReadToEndAsync();
    //            var jsonDocument = BsonDocument.Parse(jsonString);

    //            var existingDevice = await _repository.GetDeviceByNameAsync(deviceName);

    //            if (existingDevice != null)
    //            {
    //                // Extract Time and EcgWaveform from the JSON
    //                var time = jsonDocument.GetValue("Time").ToString();
    //                var ecgWaveform = jsonDocument.GetValue("EcgWaveform").AsInt32;

    //                // Create a new Data document
    //                var newDataDocument = new Data
    //                {
    //                    timestamp = DateTime.UtcNow, // Set timestamp as current UTC time
    //                    value = ecgWaveform
    //                };

    //                // Create a new DataSet document
    //                var newDataSetDocument = new DataSet
    //                {
    //                    deviceId = existingDevice._id,
    //                    timestamp = DateTime.UtcNow, // Set timestamp as current UTC time
    //                    Data = new List<Data> { newDataDocument }
    //                };

    //                // Insert the new DataSet document into the DataSet collection
    //                await _repository.InsertDataSetAsync(newDataSetDocument);

    //                return Ok("File uploaded, and data added to the DataSet.");
    //            }
    //            else
    //            {
    //                return BadRequest("Device with the specified name does not exist.");
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        // Handle exceptions here
    //        return StatusCode(500, "Internal server error.");
    //    }
    //}

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file, string deviceName)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is not provided.");
        }

        if (string.IsNullOrEmpty(deviceName))
        {
            return BadRequest("Device name is required.");
        }

        try
        {
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
                        timestamp = DateTime.UtcNow, // Set timestamp as current UTC time
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