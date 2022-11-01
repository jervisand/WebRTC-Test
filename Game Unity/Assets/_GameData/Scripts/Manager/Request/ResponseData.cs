using Newtonsoft.Json.Linq;

namespace Nagih
{
    public class ResponseData
    {
        public int error;
        public string message;
        public JToken data;

        public ResponseData() { }

        public ResponseData(int error)
        {
            this.error = error;
            data = new JObject();
            data["Error"] = error;
        }

        public ResponseData(int error, string message, JToken data)
        {
            this.error = error;
            this.message = message;

            this.data = data;
            if (this.data.Type == JTokenType.Null)
            {
                this.data = new JObject();
            }
            this.data["Error"] = error;
        }
    }
}