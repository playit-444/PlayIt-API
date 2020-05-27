namespace PlayIt_Api.Models.GameServer
{
    public class RoomData : IRoomData
    {
        public string RoomID { get; }
        public string Name { get; }
        public int MaxUsers { get; }
        public int CurrentUsers { get; set; }
        public bool PrivateRoom { get; }
        public int GameType { get; }

        public RoomData(string roomId, string name, int maxUsers, int currentUsers, bool privateRoom, int gameType)
        {
            RoomID = roomId;
            Name = name;
            MaxUsers = maxUsers;
            CurrentUsers = currentUsers;
            PrivateRoom = privateRoom;
            GameType = gameType;
        }
    }
}
