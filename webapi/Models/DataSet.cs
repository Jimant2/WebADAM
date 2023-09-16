namespace webapi.Models
{
    public class DataSet
    {
        public int dataSetId { get; set; }
        public int deviceId { get; set; }

        public DateTime timestamp { get; set; }

        public List<Data>? dataList {  get; set; }  
    }
}
