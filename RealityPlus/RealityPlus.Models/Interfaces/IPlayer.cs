using RealityPlus.Models.Player;
using System.Security;

namespace RealityPlus.Models.Interfaces
{
    public interface IPlayer
    {
        public Guid LoginUser(string loginName, SecureString password);
        public bool LogoutUser(Guid sessionId);
        public UserDetails GetUserBySession(Guid sessionId);
        public UserDetails GetUserByPlayerId(Guid playerId);
        public void ChangeUserPassword(Guid sessionId, SecureString password, SecureString newPassword);
        public Guid CreateUser(UserDetails newUser, SecureString password);
        IEnumerable<Guid> GetAlllAvaliableOpponents(Guid currentUser);
    }
}
