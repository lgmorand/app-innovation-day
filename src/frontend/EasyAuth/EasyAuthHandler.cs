using CafeReadConf.Configure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace CafeReadConf.EasyAuth
{
    public class EasyAuthAuthenticationHandler : AuthenticationHandler<EasyAuthOptions>
    {
        public EasyAuthAuthenticationHandler(
            IOptionsMonitor<EasyAuthOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                bool easyAuthEnabled = String.Equals(Environment.GetEnvironmentVariable("WEBSITE_AUTH_ENABLED", EnvironmentVariableTarget.Process), "True", StringComparison.InvariantCultureIgnoreCase);
                if (!easyAuthEnabled) return Task.FromResult(AuthenticateResult.NoResult());

                string easyAuthProvider = Context.Request.Headers["X-MS-CLIENT-PRINCIPAL-IDP"].FirstOrDefault();
                string msClientPrincipalEncoded = Context.Request.Headers["X-MS-CLIENT-PRINCIPAL"].FirstOrDefault();
                if (String.IsNullOrWhiteSpace(msClientPrincipalEncoded)) return Task.FromResult(AuthenticateResult.NoResult());

                byte[] decodedBytes = Convert.FromBase64String(msClientPrincipalEncoded);
                string msClientPrincipalDecoded = System.Text.Encoding.Default.GetString(decodedBytes);
                MsClientPrincipal clientPrincipal = JsonSerializer.Deserialize<MsClientPrincipal>(msClientPrincipalDecoded);

                ClaimsPrincipal principal = new ClaimsPrincipal();
                IEnumerable<Claim> claims = clientPrincipal.Claims.Select(x => new Claim(x.Type, x.Value));
                principal.AddIdentity(new ClaimsIdentity(claims, clientPrincipal.AuthenticationType, clientPrincipal.NameType, clientPrincipal.RoleType));

                AuthenticationTicket ticket = new AuthenticationTicket(principal, easyAuthProvider);
                AuthenticateResult success = AuthenticateResult.Success(ticket);
                Context.User = principal;

                return Task.FromResult(success);
            }
            catch (Exception ex)
            {
                return Task.FromResult(AuthenticateResult.Fail(ex));
            }
        }
    }
}
