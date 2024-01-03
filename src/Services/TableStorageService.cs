using Azure;
using Azure.Data.Tables;
using Azure.Identity;
using System;

namespace CafeReadConf
{
    public class TableStorageService
    {
        private const string TableName = "users";
        private readonly string _connString;

        public TableStorageService(string connString)
        {
            _connString = connString;
        }

        private async Task<TableClient> GetTableClient()
        {
            //var tableServiceClient = new TableServiceClient(uri, new DefaultAzureCredential());

            var serviceClient = new TableServiceClient(_connString);
            var tableClient = serviceClient.GetTableClient(TableName);
            return tableClient;
        }


        public async Task<List<User>> GetUsers()
        {
            var tableClient = await GetTableClient();
            var users = new List<User>();
            Pageable<User> queryResultsFilter = tableClient.Query<User>();
            foreach (User qEntity in queryResultsFilter)
            {
              users.Add(qEntity);
            }

            return users;
        }


        //var tableClient = new TableClient("DefaultEndpointsProtocol=https;AccountName=stoappinnoday;AccountKey=Mb9x7U5QmMclkjjMtvVabO7FzThE1Ylk2mKmhh5phZjPWtjMxgh8gpVN5/m7csDTnufoDQw0HNeO+AStX4xvrA==;EndpointSuffix=core.windows.net", "users");

        //Pageable<TableEntity> queryResultsFilter = tableClient.Query<TableEntity>();

        //    // Iterate the <see cref="Pageable"> to access all queried entities.
        //    foreach (TableEntity qEntity in queryResultsFilter)
        //    {
        //        Console.WriteLine($"{qEntity.GetString("FirstName")}: {qEntity.GetString("LastName")}");
        //    }

        //    return null;
    }
}
