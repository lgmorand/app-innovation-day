using CafeReadConf.Frontend.Models;

namespace CafeReadConf.Frontend.Service
{
    public abstract class IUserService
    {
        private const string DEFAULT_TABLE_NAME = "users";
        internal readonly string _tableName;
        internal readonly string? _tableStorageConnectionString;
        internal readonly string? _tableStorageUri;
        internal readonly ILogger<IUserService> _logger;
        internal readonly UserEntityFactory _userEntityFactory;

        public IUserService(IConfiguration config, ILogger<IUserService> logger, UserEntityFactory userEntityFactory)
        {
            _logger = logger;
            _userEntityFactory = userEntityFactory;
            _tableStorageConnectionString = config.GetValue<string>("secret");
            _tableStorageUri = config.GetValue<string>("AZURE_TABLE_STORAGE_URI");
            _tableName = config.GetValue<string>("AZURE_TABLE_STORAGE_TABLENAME") ?? DEFAULT_TABLE_NAME;
        }

        /// <summary>
        /// Get all users from Azure Table Storage
        /// </summary>
        /// <returns></returns>
        public abstract Task<List<UserEntity>> GetUsers();
        public abstract Task AddUser(Usermodel user);
    }
}
