using RealityPlus.Models.Interfaces;
using RealityPlus.Server.Interfaces;
using System.Net;
using System.Numerics;
using System.Text.Json;

namespace RealityPlus.Server.Server
{
    internal class GameMatchController : IServerController
    {
        private JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly IGameMatch GameMatch;

        public GameMatchController(IGameMatch gameMatch)
        {
            GameMatch = gameMatch;
        }


        public bool HandleRequest(HttpListenerRequest request, StreamWriter response)
        {
            if (string.IsNullOrWhiteSpace(request.RawUrl))
            {
                return false;
            }
            if (request.HttpMethod != HttpMethod.Get.ToString())
            {
                return false;
            }
  
            var sessionId = request.Headers.Get("sessionId");
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return false;
            }

            var url = request.RawUrl.Trim('/').Trim();


            switch (url)
            {
                case "rank":
                    GetRankMatches(new Guid(sessionId), response);
                    return true;
                case "quick":
                    GetQuickMatches(new Guid(sessionId), response);
                    return true;
            }
            return false; ;
        }

        private void GetRankMatches(Guid sessionId, StreamWriter response)
        {
            var matches = GameMatch.Ranked(sessionId);
            response.Write(JsonSerializer.Serialize(matches, JsonOptions));
        }

        private void GetQuickMatches(Guid sessionId, StreamWriter response)
        {
            var matches = GameMatch.Quick(sessionId);
            response.Write(JsonSerializer.Serialize(matches, JsonOptions));
        }

    }
}
