namespace Nagih
{
    public class UserReturnData : ReturnData
    {
        public string UserId;
        public int TutorialStatus;
        public long LastPlaySeconds;

        public UserReturnData() : base() { }
    }
}