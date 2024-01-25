using Azure;
using Azure.Data.Tables;
using Azure.Identity;
using CafeReadConf.Frontend.Models;
using CafeReadConf.Frontend.Service;
using Microsoft.Extensions.Configuration;
using System;

namespace CafeReadConf
{
    public class UserServiceTableStorage : IUserService
    {

        public UserServiceTableStorage(
            IConfiguration configuration,
            ILogger<UserServiceTableStorage> logger,
            UserEntityFactory userEntityFactory) : base(configuration, logger, userEntityFactory) { }


        /// <summary>
        /// Get TableClient from Azure Table Storage
        /// </summary>
        /// <returns></returns>
        private TableClient GetTableClient()
        {
            TableServiceClient serviceClient;

            if (string.IsNullOrEmpty(this._tableStorageConnectionString)) // mode MSI
            {
                serviceClient = new TableServiceClient(new Uri(this._tableStorageUri), new DefaultAzureCredential());
            }
            else // mode connection string
            {
                serviceClient = new TableServiceClient(this._tableStorageConnectionString);
            }

            var tableClient = serviceClient.GetTableClient(this._tableName);
            return tableClient;
        }


        /// <summary>
        /// Get all users from Azure Table Storage
        /// </summary>
        /// <returns></returns>
        public override async Task<List<UserEntity>> GetUsers()
        {
            var users = new List<UserEntity>();

            try
            {
                var tableClient = GetTableClient();
                users = tableClient.Query<UserEntity>().ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return users;
        }
        
        /// <summary>
        /// Add a user to Azure Table Storage
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>

        public override async Task AddUser(Usermodel input)
        {
            throw new NotImplementedException();
        }
    }
}
