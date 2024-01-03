using Azure;
using Azure.Data.Tables;

namespace CafeReadConf
{
    public class User : ITableEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
