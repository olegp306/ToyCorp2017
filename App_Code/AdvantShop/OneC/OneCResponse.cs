using Newtonsoft.Json;

namespace AdvantShop.OneC
{
    public class BaseOneCResponse
    {
        public BaseOneCResponse()
        {
            errors = "";
            warnings = "";
        }

        public string errors { get; set; }

        public string warnings { get; set; }
    }

    public class OneCResponse : BaseOneCResponse
    {
        public OneCResponse(){}
        public OneCResponse(string status, string errors)
        {
            this.status = status;
            this.errors = errors;
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string packageid { get; set; }
        public string status { get; set; }
    }

    public class OneCDeletedItemsResponse : BaseOneCResponse
    {
        public string ids { get; set; }
        public string status { get; set; }
    }
}