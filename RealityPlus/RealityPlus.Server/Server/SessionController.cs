using RealityPlus.Models.Interfaces;
using RealityPlus.Models.Player;
using RealityPlus.Server.Interfaces;
using System.Net;
using System.Security;
using System.Text.Json;

namespace RealityPlus.Server.Server
{
    internal class SessionController: IServerController
    {
        private JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly IPlayer Player;

        public SessionController(IPlayer player)
        {
            Player = player;
        }

        bool IServerController.HandleRequest(HttpListenerRequest request, StreamWriter response)
        {
            if (string.IsNullOrWhiteSpace(request.RawUrl))
            {
                return false;
            }
            var url = request.RawUrl.Trim('/').Trim();
            if (url != "session")
            {
                return false;
            }
            var sessionId = request.Headers.Get("sessionId");
            if (string.IsNullOrWhiteSpace(sessionId) && request.HttpMethod != HttpMethod.Post.ToString())
            {
                return false;
            }

            switch (request.HttpMethod.ToLowerInvariant())
            {
                case "get":
                    GetSessionUser(new Guid(sessionId), response);
                    return true;
                case "delete":
                    LogoutSession(new Guid(sessionId), response);
                    return true;
                case "post":
                    LoginUser(request, response);
                    return true;
            }

            return false;
        }

        private void GetSessionUser(Guid sessionId, StreamWriter response)
        {
            var player = Player.GetUserBySession(sessionId);
            response.Write(JsonSerializer.Serialize(player, JsonOptions));
        }

        private void LogoutSession(Guid sessionId, StreamWriter response)
        {
            Player.LogoutUser(sessionId);
        }

        private void LoginUser(HttpListenerRequest request, StreamWriter response)
        {
            using var reader = new StreamReader(request.InputStream);
            var json = reader.ReadToEnd();
            var credentials = JsonSerializer.Deserialize<LoginUser>(json, JsonOptions);
            if (credentials == null)
            {
                return;
            }
            var sessionId = Player.LoginUser(credentials.UserName, ConvertToSecureString(credentials.Password));

            response.Write(JsonSerializer.Serialize(sessionId));
        }

        private SecureString ConvertToSecureString(string password)
        {
            var securePassword = new SecureString();

            foreach (char c in password)
                securePassword.AppendChar(c);

            securePassword.MakeReadOnly();
            return securePassword;
        }
    }
}
