namespace Nagih
{
    public class UserRequestData : IRequestData
    {
        public string UserId;
        public int? TutorialStatus;
        public long? LastPlaySeconds;

        public UserRequestData() : base() { }
    }
}