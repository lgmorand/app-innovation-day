using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Net.Http.Headers;

namespace CafeReadConf.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private IHttpClientFactory _clientFactory;
        private IConfiguration _configuration;

        public void OnGet()
        {
        }

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnPostAsync()
        {

            
            var request = new HttpRequestMessage(
            HttpMethod.Get,
            new Uri(baseUri: new Uri(_configuration["BACKEND_API_URL"]), relativeUri: "api/secret")){
                Headers ={{ HeaderNames.Accept, "application/json"}}
            };

            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);
            // if (response.IsSuccessStatusCode)
            // {
            //     ViewData["Secret"] = await response.Content.ReadAsStringAsync();
            // }
            // else
            // {
            //     ViewData["Secret"] = "Error";
            // }
            return Page();
        }

        
    }
}