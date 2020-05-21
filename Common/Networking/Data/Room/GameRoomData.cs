﻿namespace Common.Networking.Data.Room
{
    public class GameRoomData : IGameRoomData
    {
        public GameRoomData(string roomId, string name, int maxUsers, int currentUsers, bool privateRoom, int gameType)
        {
            RoomID = roomId;
            Name = name;
            MaxUsers = maxUsers;
            CurrentUsers = currentUsers;
            Private = privateRoom;
            GameType = gameType;
        }

        public string RoomID { get; }
        public string Name { get; }
        public int MaxUsers { get; }
        public int CurrentUsers { get; }
        public bool Private { get; }
        public int GameType { get; }
        public int PacketId => 5;
    }
}
