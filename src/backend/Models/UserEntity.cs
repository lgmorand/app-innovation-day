using System.Text.Json.Serialization;
using System.Text.Json;
using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace CafeReadConf.Backend.Models
{
    public class UserEntity : ITableEntity
    {
        [JsonPropertyName("FirstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("LastName")]
        public string LastName { get; set; }

        [JsonPropertyName("PartitionKey")]
        // [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string PartitionKey { get; set; }

        [JsonPropertyName("RowKey")]
        // [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string RowKey { get; set; }

        [JsonPropertyName("Timestamp")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public DateTimeOffset? Timestamp { get; set; }

        [JsonPropertyName("odata.etag")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public ETag ETag { get; set; }

        public UserEntity(string firstName, string lastName, string partitionKey, string rowKey,
        DateTimeOffset? timestamp,
        ETag eTag)
        {
            FirstName = firstName;
            LastName = lastName;
            PartitionKey = partitionKey;
            RowKey = rowKey;
            Timestamp = timestamp;
            ETag = eTag;
        }
    }

    public class UserEntityFactory
    {
        private readonly IConfiguration _configuration;

        public UserEntityFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public UserEntity CreateUserEntity(string firstName, string lastName, string? partitionKey = null, string? rowKey = null, DateTimeOffset? timestamp = null, ETag? eTag = null)
        {
            return new UserEntity(
                    firstName,
                    lastName,
                    partitionKey ?? _configuration.GetValue<string>("AZURE_TABLE_PARTITION_KEY"),
                    rowKey ?? Guid.NewGuid().ToString(),
                    timestamp ?? DateTimeOffset.Now,
                    ETag.All);
        }
    }

}

