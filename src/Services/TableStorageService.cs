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

        /// <summary>
        /// Get TableClient from Azure Table Storage
        /// </summary>
        /// <returns></returns>
        private async Task<TableClient> GetTableClient()
        {
            TableServiceClient serviceClient;

            if(string.IsNullOrEmpty(_connString)) // mode MSI
            {
                serviceClient = new TableServiceClient(new Uri("https://stoappinnoday.table.core.windows.net/"), new DefaultAzureCredential());
            }
            else // mode connection string
            {
                serviceClient = new TableServiceClient(_connString);
            }

            var tableClient = serviceClient.GetTableClient(TableName);
            return tableClient;
        }


        /// <summary>
        /// Get all users from Azure Table Storage
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetUsers()
        {
            var users = new List<User>();

            try
            {
                var tableClient = await GetTableClient();
                Pageable<User> queryResultsFilter = tableClient.Query<User>();
                foreach (User qEntity in queryResultsFilter)
                {
                    users.Add(qEntity);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return users;
        }
    }
}
