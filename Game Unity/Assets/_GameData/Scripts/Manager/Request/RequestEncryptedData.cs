using UnityEngine;
using System.Collections;

namespace Nagih
{
    public class RequestEncryptedData
    {
        public string game_id;
        public string data;

        public RequestEncryptedData()
        {
            game_id = Const.GAME_ID;
        }

        public RequestEncryptedData(string data)
        {
            game_id = Const.GAME_ID;
            this.data = data;
        }
    }
}