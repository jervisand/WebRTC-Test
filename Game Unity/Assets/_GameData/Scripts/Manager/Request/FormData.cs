using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;

namespace Nagih
{
    public class FormData
    {
        public string req_type;
        public string name;
        public string app_id;

        public FormData(string reqType)
        {
            req_type = reqType;
            name = "Apk";
            app_id = "nagihgames.com";
        }

        public virtual void ExecuteCallback(JToken returnData) { }
    }

    public class FormData<T> : FormData where T : IReturnData, new()
    {
        [JsonIgnore]
        public Action<T> Callback;

        public FormData(string type, Action<T> callback) : base(type)
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
}