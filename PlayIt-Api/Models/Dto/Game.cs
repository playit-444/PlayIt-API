using Common.Networking.Data.Room;

namespace PlayIt_Api.Models.Dto
{
    public class Game : IRoomData
    {
        public string RoomID { get; }
        public string Name { get; }
        public int MaxUsers { get; }
        public int CurrentUsers { get; }
        public bool Private { get; }

        public Game(string roomId, string name, int maxUsers, int currentUsers, bool @private)
        {
            RoomID = roomId;
            Name = name;
            MaxUsers = maxUsers;
            CurrentUsers = currentUsers;
            Private = @private;
        }
    }
}
