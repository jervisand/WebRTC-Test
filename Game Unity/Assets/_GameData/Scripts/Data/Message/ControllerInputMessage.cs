using Newtonsoft.Json.Linq;

namespace Nagih
{
    public class ControllerInputMessage
    {
        public int id;
        public string input;
        public string condition;
        public JObject content;
    }
}