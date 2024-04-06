using RealityPlus.DataLayer.Models;
using RealityPlus.DataModels.Interfaces;
using RealityPlus.Models.Player;
using System.Collections.Concurrent;

namespace RealityPlus.InMemoryDataAccess.DataAccess
{
    internal class Player : IPlayerData
    {
        private ConcurrentDictionary<Guid, UserDetails> Players = new ConcurrentDictionary<Guid, UserDetails>();
        private ConcurrentDictionary<Guid, Password> Passwords = new ConcurrentDictionary<Guid, Password>();

        Guid IPlayerData.CreatePlayer(UserDetails userDetails)
        {
            if (Players.Any(p => p.Value.UserName == userDetails.UserName))
            {
                throw new Exception($"Player found with the user name of {userDetails.UserName}");
            }
            var playerId = Guid.NewGuid();
            Players.TryAdd(playerId, userDetails);
            return playerId;
        }

        UserDetails IPlayerData.GetPlayerByPlayerId(Guid playerId)
        {
            return Players[playerId];
        }

        Guid? IPlayerData.GetPlayerIdByLoginName(string userName)
        {
            return Players
                .Where(p => p.Value.UserName == userName)
                .FirstOrDefault()
                .Key;
        }

        Password IPlayerData.GetStoredPasswordForPlayer(Guid playerId)
        {
            return Passwords[playerId];
        }

        void IPlayerData.UpdatePassword(Guid playerId, Password passwordDetails)
        {
            Passwords.TryRemove(playerId, out var oldPassword);
            if (oldPassword != null)
            {
                oldPassword.Dispose();
            }
            Passwords.TryAdd(playerId, passwordDetails);
        }
    }
}
