using RealityPlus.Models.Interfaces;
using RealityPlus.Models.Player;

namespace RealityPlus.BusinessLayer.BusinessLayer
{
    internal class GameMatch : IGameMatch
    {
        private readonly IPlayer Player;

        public GameMatch(IPlayer player)
        {
            Player = player;
        }

        IEnumerable<string> IGameMatch.Quick(Guid sessonId)
        {
            var players = GetAvailableUsersFoRank(sessonId).ToList();
            players.AddRange(GetAllLoggedInUsers(sessonId));
            return players
                .Select(p => p.UserName)
                .Distinct().ToList(); 
        }

        IEnumerable<string> IGameMatch.Ranked(Guid sessonId)
        {
            return GetAvailableUsersFoRank(sessonId)
                .Select(p => p.UserName);
        }


        private IEnumerable<UserDetails> GetAvailableUsersFoRank(Guid sessionId)
        {
            var currentUser = Player.GetUserBySession(sessionId);
            var regionUsers = GetAllLoggedInUsers(sessionId)
                .Where(u => u.Region == currentUser.Region)
                .ToList();
            var closeMatch = GetGroupedMatchUsers(regionUsers, currentUser.Rank - 10, currentUser.Rank + 10);
            if (closeMatch.Any())
            {
                return closeMatch;
            }

            return GetGroupedMatchUsers(regionUsers, currentUser.Rank - 25, currentUser.Rank + 25);
        }

        private IEnumerable<UserDetails> GetGroupedMatchUsers(IEnumerable<UserDetails> regionUsers, int minRank, int maxRank)
        {
            return regionUsers
                .Where(u => u.Rank >= minRank)
                .Where(u => u.Rank <= maxRank)
                .ToList();
        }

        private IEnumerable<UserDetails> GetAllLoggedInUsers(Guid currentUser)
        {
            return Player.GetAlllAvaliableOpponents(currentUser)
                .Select(u => Player.GetUserByPlayerId(u))
                .ToList();
        }
    }
}
