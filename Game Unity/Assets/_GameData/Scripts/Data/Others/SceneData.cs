namespace Nagih
{
    [System.Serializable]
    public class SceneData
    {
        public Enum.Scene Scene;
        public bool ShowLoading;
        public bool IsLoadBundle;

        public string GetAddressableKey()
        {
            return "Scene_" + Scene;
        }
    }
}