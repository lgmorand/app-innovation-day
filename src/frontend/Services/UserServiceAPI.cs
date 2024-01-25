using Azure;
using Azure.Data.Tables;
using Azure.Identity;
using CafeReadConf.Frontend.Models;
using CafeReadConf.Frontend.Service;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Text;
using System.Text.Json;

namespace CafeReadConf
{
    public class UserServiceAPI : IUserService
    {
        private readonly HttpClient _httpClient;
        public UserServiceAPI(
            IConfiguration configuration,
            ILogger<UserServiceAPI> logger,
            IHttpClientFactory httpClientFactory,
            UserEntityFactory userEntityFactory) : base(configuration, logger, userEntityFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiBaseAddress");

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
                var userApiResult = await _httpClient.GetAsync("/api/users");

                if (userApiResult.StatusCode != HttpStatusCode.OK)
                {
                    _logger.LogError($"Error: {userApiResult.StatusCode}");
                    return users;
                }

                users = JsonSerializer.Deserialize<List<UserEntity>>(
                    await userApiResult.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return users;
        }

        public override async Task AddUser(Usermodel input)
        {
            try
            {
                var userEntity = _userEntityFactory.CreateUserEntity(input.FirstName, input.LastName);

                //Serializing the userEntity object to JSON string
                var stringPayload = await Task.Run(() => JsonSerializer.Serialize(userEntity));

                // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                //Send POST request to the API endpoint to create a new user
                var userApiResult = await _httpClient.PostAsync("/api/users", httpContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

    }
}