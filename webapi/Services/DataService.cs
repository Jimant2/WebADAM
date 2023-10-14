using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using webapi.DataRepos;
using webapi.Models;

namespace webapi.Services
{
    public class DataService : IDataService
    {
        private readonly IWebADAMRepo _repository;
        public DataService(IWebADAMRepo repository) 
        {
            _repository = repository;
        }
        public List<DataSet> GetDataSetFromService()
        {
            return _repository.GetDataSet();
        }
        public async Task InsertDataSetFromService(List<DataSet> dataSet)
        {

            await _repository.InsertDataSetAsync(dataSet);
        }
        public async Task<List<DataSet>> GetDataSetsByDeviceNameFromService(string deviceName)
        {
           return await _repository.GetDataSetsByDeviceNameAsync(deviceName);
        }
        public async Task<List<DataSet>> UploadFileFromService(IFormFile file, string deviceName, string dataType)
        {
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
                        var timestampValue = jsonDocument["Time"].ToObject<DateTime>();

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
                            timestamp = timestampValue,
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
                    return dataSetList;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
