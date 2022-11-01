namespace Nagih
{
    public class PlayerData
    {
        public int Id;
        public string Name;
        public bool IsMaster;

        public string Team;

        public PlayerData(int id, string name)
        {
            Id = id;
            Name = name;
            IsMaster = false;
        }
    }

}
