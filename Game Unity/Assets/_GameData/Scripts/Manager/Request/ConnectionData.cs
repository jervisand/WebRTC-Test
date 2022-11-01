using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Nagih
{
    public class ConnectionData<T> : ConnectionData where T : IReturnData, new()
    {
        [JsonIgnore]
        public Action<T> Callback;

        public ConnectionData(string type, string userId, string token, IRequestData data, Action<T> callback) : base(type, userId, token, data)
        {
            Callback = callback;
        }

        public override void ExecuteCallback(JToken returnData)
        {
            if (Callback != null)
            {
                Action<T> temp = Callback;
                Callback = null;

                temp(returnData == null ? default : returnData.ToObject<T>());
            }
        }
    }

    public abstract class ConnectionData
    {
        public string req_type;
        public string user_id;
        public string token;
        public string game_id;
        public IRequestData data;

        public ConnectionData(string type, string userId, string token, IRequestData data)
        {
            req_type = type;
            user_id = userId;
            game_id = Const.GAME_ID;

            this.token = token;
            this.data = data;
        }

        public virtual void ExecuteCallback(JToken returnData) { }
    }
}