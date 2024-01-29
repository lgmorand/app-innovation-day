using Microsoft.AspNetCore.Authentication;

namespace CafeReadConf.EasyAuth
{
    public class EasyAuthOptions : AuthenticationSchemeOptions
    {
        public EasyAuthOptions()
        {
            Events = new object();
        }
    }
}