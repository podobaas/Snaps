using Newtonsoft.Json;

namespace Snaps.Settings
{
    public class SnapsSettings
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        
        [JsonProperty("maxDegreeOfConcurrency")]
        public int MaxDegreeOfConcurrency { get; set; }
    }
}