using webapi.Models;

namespace webapi.Services
{
    public interface IDataService
    {
        public List<DataSet> GetDataSetFromService();
        public Task InsertDataSetFromService(List<DataSet> dataSet);
        public Task<List<DataSet>> GetDataSetsByDataTypeFromService(string dataType);
        public Task<IEnumerable<DateTime>> GetTimestampsByDataTypeFromServiceAsync(string dataType);
        public Task<IEnumerable<DataSet>> GetDataSetByTimestampFromServiceAsync(DateTime timestamp);
        public Task<List<DataSet>> UploadFileFromService(IFormFile file, string dataType);

    }
}
