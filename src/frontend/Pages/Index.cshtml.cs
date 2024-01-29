using Microsoft.AspNetCore.Mvc.RazorPages;
using CafeReadConf.Frontend.Models;
using CafeReadConf.Frontend.Service;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace CafeReadConf.Pages
{
    public class IndexModel : PageModel
    {
        // Services DI 
        private readonly ILogger<IndexModel> _logger;

        public IUserService _userService { get; set; }
        private readonly IConfiguration _configuration;

        public List<UserEntity> Users { get; set; }
        public string? Secret { get => _configuration.GetValue<string>("SECRET"); }

        public IndexModel(ILogger<IndexModel> logger,
        IUserService userService,
        IConfiguration configuration)
        {
            _logger = logger;
            _userService = userService;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // ClaimsPrincipal currentUser = this.User;
            // ViewData["UserName"] = currentUser.Identity.Name;
            // ViewData["UserEmail"] = currentUser.FindFirst(ClaimTypes.Email)?.Value;
            // ViewData["UserRoles"] = currentUser.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

            Users = await ReadItems();
            return Page();
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
