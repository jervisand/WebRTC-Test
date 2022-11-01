namespace Nagih
{
    public class PlayerDataMessage
    {
        public int id;
        public string name;

        public PlayerDataMessage() { }

        public PlayerDataMessage(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}