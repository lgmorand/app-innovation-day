using Microsoft.AspNetCore.Mvc.RazorPages;
using CafeReadConf.Frontend.Models;
using CafeReadConf.Frontend.Service;

namespace CafeReadConf.Pages
{
    public class IndexModel : PageModel
    {
        // Services DI 
        private readonly ILogger<IndexModel> _logger;

        public IUserService _userService { get; set; }
        private readonly IConfiguration _configuration;

        public List<UserEntity> Users { get; set; }

        public IndexModel(ILogger<IndexModel> logger,
        IUserService userService,
        IConfiguration configuration)
        {
            _logger = logger;
            _userService = userService;
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {

            Users = await ReadItems();
        }

        /// <summary>
        /// Read data from Azure Table Storage or the API based on the configuration
        /// </summary>
        private async Task<List<UserEntity>> ReadItems()
        {
            return await _userService.GetUsers();
        }
    }
}
