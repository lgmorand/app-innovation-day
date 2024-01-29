using System.Net;
using System.Text.Json;
using CafeReadConf.Backend.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;
using System;
using Azure.Identity;

namespace CafeReadConf.Backend.API
{
    public class CreateUserOutput
    {
        [TableOutput("%AZURE_TABLE_SOURCE%", Connection = "AZURE_TABLE_STORAGE_ACCOUNT")]
        public UserEntity User { get; set; }
        public HttpResponseData Response { get; set; }

        public CreateUserOutput(UserEntity user, HttpResponseData response)
        {
            User = user;
            Response = response;
        }
    }

    public class UsersAPI
    {
        private readonly ILogger _logger;
        private readonly string _partitionkey;
        private readonly UserEntityFactory _userEntityFactory;
        private readonly string _sourceTable;

        public UsersAPI(ILoggerFactory loggerFactory, IConfiguration configuration, UserEntityFactory userEntityFactory)
        {
            //Creating singletons and injecting dependencies
            _logger = loggerFactory.CreateLogger<UsersAPI>();
            _userEntityFactory = userEntityFactory;

            //Hydrating configurations from environment variables
            _partitionkey = configuration.GetValue<string>("AZURE_TABLE_PARTITION_KEY");
            _sourceTable = configuration.GetValue<string>("AZURE_TABLE_SOURCE");
        }

        [Function(nameof(CreateUser))]
        public CreateUserOutput CreateUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Users")] HttpRequestData req,
            [FromBody] UserEntity userEntity)
        {
            _logger.LogInformation("Adding a new User in the table : {}", JsonSerializer.Serialize<UserEntity>(userEntity));

            var user = _userEntityFactory.CreateUserEntity(userEntity.FirstName, userEntity.LastName);

            var newUser = new
            {
                newUserUrl = $"{req.Url.Scheme}://{req.Url.Authority}/api/Users/{user.RowKey}"
            };

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            response.WriteString(JsonSerializer.Serialize(newUser));

            return new CreateUserOutput(
                user,
                response);
        }

        [Function(nameof(GetUsersById))]
        public HttpResponseData GetUsersById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Users/{userId}")] HttpRequestData req,
            [TableInput(
                tableName: "%AZURE_TABLE_SOURCE%",
                partitionKey: "%AZURE_TABLE_PARTITION_KEY%",
                rowKey: "{userId}",
                Connection = "AZURE_TABLE_STORAGE_ACCOUNT")] TableEntity userEntity,
                string userId)
        {
            _logger.LogInformation("Retrieving user with id: {UserId} in the table : {SourceTable}", userId, _sourceTable);

            HttpResponseData response;

            if (!userEntity.ContainsKey("firstname") || !userEntity.ContainsKey("lastname"))
            {
                response = req.CreateResponse(HttpStatusCode.InternalServerError);
                response.Headers.Add("Content-Type", "plain/text; charset=utf-8");
                response.WriteString($"Internal Server Error : user {userId} cannot be fetched");
                return response;
            }

            UserEntity fetchedUser = _userEntityFactory.CreateUserEntity(
                    userEntity["firstname"].ToString(),
                    userEntity["lastname"].ToString(),
                    userEntity["RowKey"].ToString());

            //Preparing user entity to be returned
            response = req.CreateResponse(HttpStatusCode.OK);

            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            response.WriteString($"{JsonSerializer.Serialize(fetchedUser)}");

            return response;
        }

        [Function(nameof(GetUsers))]
        public HttpResponseData GetUsers(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Users")] HttpRequestData req,
            [TableInput(
                tableName: "%AZURE_TABLE_SOURCE%",
                partitionKey: "%AZURE_TABLE_PARTITION_KEY%",
                Connection = "AZURE_TABLE_STORAGE_ACCOUNT",
                Take = 50)] IEnumerable<UserEntity> users)
        {
            _logger.LogInformation("Retrieving users in the table : {SourceTable}", _sourceTable);

            //Preparing user entity to be returned
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            var usersList = JsonSerializer.Serialize(users);
            response.WriteString(usersList);

            return response;
        }
    }
}
