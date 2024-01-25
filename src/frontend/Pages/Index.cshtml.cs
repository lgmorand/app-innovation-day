using Microsoft.AspNetCore.Mvc.RazorPages;
using CafeReadConf.Frontend.Models;
using CafeReadConf.Frontend.Service;

namespace CafeReadConf.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public string? Secret { get; set; }
        public List<UserEntity> Users { get; set; }
        public IUserService _userService { get; set; }

        public IndexModel(ILogger<IndexModel> logger,
        IUserService userService,
        IConfiguration configuration)
        {
            _logger = logger;
            _userService = userService;

            Secret = configuration.GetValue<string>("secret");
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
