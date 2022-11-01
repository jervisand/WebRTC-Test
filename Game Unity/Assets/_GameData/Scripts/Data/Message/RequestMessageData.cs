using Newtonsoft.Json.Linq;

namespace Nagih
{
    public class RequestMessageData
    {
        public string type;
        public JObject data;

        public RequestMessageData(string type)
        {
            this.type = type;
            data = new JObject();
        }
    }
}