namespace AkademiPlusSignalR.Api.DAL
{
    public class User
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public int RoomID { get; set; }
        public Room Room { get; set; }
    }
}
