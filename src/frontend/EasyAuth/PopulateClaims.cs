using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

public class PopulateClaims
{
    private readonly RequestDelegate _next;

    public PopulateClaims(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity.IsAuthenticated)
        {
            var claimsIdentity = context.User.Identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                var accessTokenClaim = claimsIdentity.FindFirst("access_token");
                if (accessTokenClaim == null)
                {
                    // Access token claim is not present, add it
                    claimsIdentity.AddClaim(new Claim("access_token",
                    context.GetTokenAsync("access_token").Result));
                }
            }
        }

        // Call the next middleware in the pipeline
        await _next(context);
    }
}