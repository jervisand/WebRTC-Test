using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nagih
{
    public interface ISaveData
    {
        int Version { get; }
        public JObject Migrate(JObject data, int currentVersion);
    }
}
