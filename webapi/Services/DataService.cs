using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System.Globalization;
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
        public async Task<List<DataSet>> UploadFileFromService(IFormFile file, string dataType)
        {
            try
            {
                DateTime date1 = DateTime.Now;
                var currentTime = date1.AddHours(2);

             //   var existingDevice = await _repository.GetDeviceByValueTypeAsync(dataType);
                var dataSetList = new List<DataSet>();

                using (var reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<dynamic>();

                    foreach (var record in records)
                    {
                        var timestampValue = DateTime.Parse(record.Time, CultureInfo.InvariantCulture);

                        int value = 0;
                        foreach (var property in record as IDictionary<string, object>)
                        {
                            if (property.Value is string stringValue)
                            {
                                if (int.TryParse(stringValue, out int parsedIntValue))
                                {
                                    value = parsedIntValue;
                                    break;
                                }
                                else if (double.TryParse(stringValue, out double parsedDoubleValue))
                                {
                                    value = (int)parsedDoubleValue;
                                    break;
                                }
                            }
                            else if (property.Value is int intValue)
                            {
                                value = intValue;
                                break;
                            }
                            else if (property.Value is long longValue)
                            {
                                value = (int)longValue;
                                break;
                            }
                            else if (property.Value is double doubleValue)
                            {
                                value = (int)doubleValue;
                                break;
                            }
                        }

                        var newDataDocument = new Data
                        {
                            timestamp = timestampValue,
                            value = value
                        };

                        var newDataSetDocument = new DataSet
                        {
                   //         deviceId = existingDevice._id,
                            timestamp = currentTime,
                            dataType = dataType,
                            Data = new List<Data> { newDataDocument }
                        };

                        dataSetList.Add(newDataSetDocument);
                    }
                }

                await _repository.InsertDataSetAsync(dataSetList);
                return dataSetList;
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
