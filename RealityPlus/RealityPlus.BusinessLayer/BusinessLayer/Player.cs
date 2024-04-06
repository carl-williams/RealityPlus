using RealityPlus.DataModels.Interfaces;
using RealityPlus.Models.Interfaces;
using RealityPlus.Models.Player;
using System.Collections.Concurrent;
using System.Security;

namespace RealityPlus.BusinessLayer.BusinessLayer
{
    internal class Player : IPlayer
    {
        private readonly IPlayerData DataAccess;
        private readonly IPasswordManageer PasswordManager;
        private readonly ConcurrentDictionary<Guid, Guid> Sessions = new ConcurrentDictionary<Guid, Guid>();

        public Player(IPlayerData dataAccess, IPasswordManageer passwordManager)
        {
            DataAccess = dataAccess;
            PasswordManager = passwordManager;
        }

        void IPlayer.ChangeUserPassword(Guid sessionId, SecureString password, SecureString newPassword)
        {
            if (!Sessions.TryGetValue(sessionId, out var playerId))
            {
                throw new UnauthorizedAccessException("Session not found");
            }
            if (!PasswordManager.ValidatePassword(playerId, password))
            {
                throw new UnauthorizedAccessException("Incorrect Password");
            }
            PasswordManager.SavePassword(playerId, newPassword);
        }

        Guid IPlayer.CreateUser(UserDetails newUser, SecureString password)
        {
            var playerId = DataAccess.CreatePlayer(newUser);
            PasswordManager.SavePassword(playerId, password);
            return playerId;
        }

        UserDetails IPlayer.GetUserByPlayerId(Guid playerId)
        {
            return DataAccess.GetPlayerByPlayerId(playerId);
        }

        UserDetails IPlayer.GetUserBySession(Guid sessionId)
        {
            if (Sessions.TryGetValue(sessionId, out var playerId))
            {
                return DataAccess.GetPlayerByPlayerId(playerId);
            }
            throw new UnauthorizedAccessException("Session not found");
        }

        Guid IPlayer.LoginUser(string loginName, SecureString password)
        {
            var playerId = DataAccess.GetPlayerIdByLoginName(loginName);
            if (playerId == null)
            {
                throw new UnauthorizedAccessException("user or password incorrect");
            }

            if (Sessions.Any(s => s.Value == playerId))
            {
                throw new UnauthorizedAccessException("User already logged in");
            }

            if (!PasswordManager.ValidatePassword(playerId.Value, password))
            {
                throw new UnauthorizedAccessException("user or password incorrect");
            }
            var sessionId = Guid.NewGuid();
            Sessions.TryAdd(sessionId, playerId.Value);
            return sessionId;
        }

        bool IPlayer.LogoutUser(Guid sessionId)
        {
            return Sessions.TryRemove(sessionId, out _);
        }

        IEnumerable<Guid> IPlayer.GetAlllAvaliableOpponents(Guid currentUser)
        {
            return Sessions
                .Where(s => s.Key != currentUser)
                .Select(s => s.Value)
                .ToList();
        }
    }
}
