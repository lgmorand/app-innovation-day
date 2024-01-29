using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace CafeReadConf.Pages.Headers
{
    public class IndexModel : PageModel
    {
        // Services DI 
        private readonly ILogger<IndexModel> _logger;

        public IndexModel() { }

        public async Task<IActionResult> OnGetAsync()
        {
            throw new Exception("Expected Exception to take a look at all the headers added by App Service Easy Auth");
        }
    }
}
