using RealityPlus.DataLayer.Models;
using RealityPlus.Models.Player;

namespace RealityPlus.DataModels.Interfaces
{
    public interface IPlayerData
    {
        Guid? GetPlayerIdByLoginName(string userName);
        UserDetails GetPlayerByPlayerId(Guid playerId);
        Password GetStoredPasswordForPlayer(Guid playerId);
        void UpdatePassword(Guid playerId, Password passwordDetails);
        Guid CreatePlayer(UserDetails userDetails);
    }
}
