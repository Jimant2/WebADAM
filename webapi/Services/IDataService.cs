using webapi.Models;

namespace webapi.Services
{
    public interface IDataService
    {
        public List<DataSet> GetDataSetFromService();
        public Task InsertDataSetFromService(List<DataSet> dataSet);
        public Task<List<DataSet>> GetDataSetsByDataTypeFromService(string dataType);
        public Task<List<DataSet>> UploadFileFromService(IFormFile file, string dataType);

    }
}
