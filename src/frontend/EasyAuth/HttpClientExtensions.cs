using System.Net.Http.Headers;

namespace CafeReadConf.EasyAuth.HttpClientExtensions
{
    public static class HttpClientExtensions
    {
        public static void HydrateAccessToken(this HttpClient httpClient, IHttpContextAccessor _httpClientAccessor, ILogger _logger)
        {
            var accessToken = _httpClientAccessor
                    .HttpContext.User.FindFirst("access_token")?.Value;

            if (accessToken == null)
            {
                accessToken = _httpClientAccessor.HttpContext.Request.Headers["X-MS-TOKEN-AAD-ACCESS-TOKEN"].ToString();

                _logger.LogInformation("Found X-MS-TOKEN-AAD-ACCESS-TOKEN in http context : setting accessToken");
            }

            if (accessToken != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                _logger.LogInformation("Set Authorization Header with the access token");

                // Kept for debug purposes 
                // _logger.LogInformation("Set Authorization Header with the access token : {accessToken}", httpClient.DefaultRequestHeaders.Authorization.ToString());

            }
        }
    }
}