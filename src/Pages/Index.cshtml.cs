using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Configuration;

namespace CafeReadConf.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public string Message { get; set; }
        public string Secret { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            Message = GetProperty("message");
            Secret = GetProperty("secret");
        }


        private string GetProperty(string key)
        {
            string value =  System.Configuration.ConfigurationManager.AppSettings[key];
            if(string.IsNullOrEmpty(value))
            {
                value = Environment.GetEnvironmentVariable(key);
            }
            return value;
        }
        
        public void OnGet()
        {

        }
    }
}
