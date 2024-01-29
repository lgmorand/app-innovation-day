using System.Text.Json.Serialization;

namespace CafeReadConf.EasyAuth
{
    public class UserClaim
    {
        [JsonPropertyName("typ")]
        public string Type { get; set; }
        [JsonPropertyName("val")]
        public string Value { get; set; }
    }

}
