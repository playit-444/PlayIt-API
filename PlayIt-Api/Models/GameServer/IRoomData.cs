namespace PlayIt_Api.Models.GameServer
{
    public interface IRoomData
    {
        public string RoomID { get; }
        public string Name { get; }
        public int MaxUsers { get; }
        public int CurrentUsers { get; set; }
        public bool PrivateRoom { get; }
        public int GameType { get; }

    }
}
