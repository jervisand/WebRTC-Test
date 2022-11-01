using UnityEngine;
using System.Collections;

namespace Nagih
{
    public class TokenRequestData : RequestData
    {
        public string token;

        public TokenRequestData(string token) : base()
        {
            this.token = token;
        }
    }
}