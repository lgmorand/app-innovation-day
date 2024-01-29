using CafeReadConf.Configure;
using Microsoft.AspNetCore.Authentication;
using System;

namespace CafeReadConf.EasyAuth
{
    public static class EasyAuthAuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddEasyAuthAuthentication(
            this AuthenticationBuilder builder,
            Action<EasyAuthOptions> configure) =>
                builder.AddScheme<EasyAuthOptions, EasyAuthAuthenticationHandler>(
                    "EasyAuth",
                    "EasyAuth",
                    configure);
    }
}
