using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Nagih
{
    public class UserRequestSetting
    {
        private UserRequestData _data;

        public UserRequestSetting()
        {
            _data = new UserRequestData();
        }

        public UserRequestData Build()
        {
            return _data;
        }

        public UserRequestSetting SetTutorialStatus()
        {
            _data.TutorialStatus = DataSelf.GetInstance().User.TutorialStatus;
            return this;
        }

        public UserRequestSetting SetLastPlaySeconds()
        {
            _data.LastPlaySeconds = DataSelf.GetInstance().User.LastPlaySeconds;
            return this;
        }
    }
}