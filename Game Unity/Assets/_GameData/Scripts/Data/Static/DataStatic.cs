using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Nagih
{
    public class DataStatic : Singleton<DataStatic>
    {
        public bool IsAndroidTv { get; private set; }
        public bool IsDeviceHasPlayServices { get; private set; }
#if ENABLE_INPUT_SYSTEM
        public InputActions InputActions { get; private set; }
#endif
        public GameDataSO GameDataSO { get; private set; }
        public Dictionary<Enum.Font, Font> FontData { get; private set; }
        public Dictionary<Enum.Icon, Sprite> IconSpriteData { get; private set; }

        public List<PlayerData> PlayerList { get; private set; }

        public IEnumerator LoadResource(YieldInstruction instruction = null)
        {
            GameDataSO = Resources.Load<GameDataSO>(Const.RESLOC_DATA_GAMEINIT);
            yield return instruction;
            GameDataSO.Initialize();
            yield return instruction;
        }

        public IEnumerator Initialize(YieldInstruction instruction = null)
        {
            IsDeviceHasPlayServices = Helper.IsPlayServicesAvailable();
            IsAndroidTv = Helper.IsAndroidTv();

#if ENABLE_INPUT_SYSTEM
            InputActions = new InputActions();
            InputActions.Enable();
#endif

            FontData = Helper.BuildDictionaryTypesFromEnum<Enum.Font, Font>(x => $"Font/{x}");
            yield return instruction;

            IconSpriteData = Helper.BuildDictionaryTypesFromEnum<Enum.Icon, Sprite>(x => $"Icon/{x}");
            yield return instruction;

            PlayerList = new List<PlayerData>();
        }

        public IEnumerable<int> GetAllPlayerIDs()
        {
            List<int> ids = new List<int>();
            foreach(PlayerData data in PlayerList)
            {
                ids.Add(data.Id);
            }
            return ids;
        }
    }
}