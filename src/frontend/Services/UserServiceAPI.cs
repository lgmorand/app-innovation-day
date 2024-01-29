using Azure;
using Azure.Data.Tables;
using Azure.Identity;
using CafeReadConf.Frontend.Models;
using CafeReadConf.Frontend.Service;
using Microsoft.Extensions.Configuration;
using CafeReadConf.EasyAuth.HttpClientExtensions;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CafeReadConf
{
    public class UserServiceAPI : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpClientAccessor;
        public UserServiceAPI(
            IConfiguration configuration,
            ILogger<UserServiceAPI> logger,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            UserEntityFactory userEntityFactory) : base(configuration, logger, userEntityFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiBaseAddress");
            _httpClientAccessor = httpContextAccessor;
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
                _httpClient.HydrateAccessToken(_httpClientAccessor, _logger);

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
                // throw ex;
            }

            return users;
        }

        public override async Task AddUser(Usermodel input)
        {
            try
            {
                var userEntity = _userEntityFactory.CreateUserEntity(input.FirstName, input.LastName);

                //Serializing the userEntity object to JSON string
                var stringPayload = JsonSerializer.Serialize(userEntity);

                _logger.LogInformation("Serialized User before sending to Azure Function : {stringUser}", stringPayload);

                // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                _httpClient.HydrateAccessToken(_httpClientAccessor, _logger);

                _logger.LogInformation("Http Content sent to Add Users API: {httpContent}", httpContent);

                //Send POST request to the API endpoint to create a new user
                var userApiResult = await _httpClient.PostAsync("/api/users", httpContent);

            }
            catch (Exception ex)
            {
                _logger.LogError(
                    @"
                        Error while trying to add new users : {exception}\n
                        Full Stack Trace : {trace}
                    ",
                    ex.Message,
                    ex.StackTrace);
            }
        }
    }
}