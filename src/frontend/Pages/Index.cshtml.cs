using Azure.Data.Tables;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Concurrent;
using System.Configuration;

namespace CafeReadConf.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public string Secret { get; set; }
        public List<User> Users { get; set; }

        public void OnGet(){}

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            Secret = GetConfig("secret");
            ReadItems(Secret);
        }

        /// <summary>
        /// Read data from Azure Table Storage
        /// </summary>
        private async void ReadItems(string connString)
        {
            Users = await new TableStorageService(connString).GetUsers();
        }

        private string GetConfig(string key)
        {
            string value =  System.Configuration.ConfigurationManager.AppSettings[key];
            if(string.IsNullOrEmpty(value))
            {
                value = Environment.GetEnvironmentVariable(key);
            }
            return value;
        }
    }
}
