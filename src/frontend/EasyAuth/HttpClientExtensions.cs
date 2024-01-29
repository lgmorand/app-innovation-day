using System.Net.Http.Headers;

namespace CafeReadConf.EasyAuth.HttpClientExtensions
{
    public static class HttpClientExtensions
    {
        public static void HydrateAccessToken(this HttpClient httpClient, IHttpContextAccessor _httpClientAccessor, ILogger _logger)
        {
            var accessToken = _httpClientAccessor
                    .HttpContext.User.FindFirst("access_token")?.Value;

            _logger.LogInformation("Claimsprincipals : {claims}", _httpClientAccessor
                    .HttpContext.User.Claims.ToList().ToString());

            if (accessToken == null)
            {
                accessToken = _httpClientAccessor.HttpContext.Request.Headers["X-MS-TOKEN-AAD-ACCESS-TOKEN"].ToString();

                _logger.LogInformation("Found X-MS-TOKEN-AAD-ACCESS-TOKEN : {accessToken}", accessToken);
            }

            if (accessToken != null)
            {
                _logger.LogInformation("Found Access Token : {accessToken}", accessToken);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                _logger.LogInformation("Set Authorization Header : {accessToken}", httpClient.DefaultRequestHeaders.Authorization.ToString());
            }
        }
    }
}