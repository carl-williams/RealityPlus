using RealityPlus.DataLayer.Models;
using RealityPlus.Models.Interfaces;
using RealityPlus.Models.Player;
using RealityPlus.Server.Interfaces;
using System.Net;
using System.Security;
using System.Text.Json;

namespace RealityPlus.Server.Server
{
    internal class UserController: IServerController
    {
        private JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly IPlayer Player;

        public UserController(IPlayer player)
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
            if (url == "user" && request.HttpMethod == HttpMethod.Post.ToString())
            {
                CreateUser(request, response);
                return true;
            }
            if (url.StartsWith("user/") && request.HttpMethod == HttpMethod.Get.ToString())
            {
                GetUser(url, response);
                return true;
            }

            return false;
        }

        private void GetUser(string url, StreamWriter response)
        {
            var playerId = new Guid(url.Split("/").Last());

            var player = Player.GetUserByPlayerId(playerId);

            response.Write(JsonSerializer.Serialize(player, JsonOptions));
        }

        private void CreateUser(HttpListenerRequest request, StreamWriter response)
        {
            using var reader = new StreamReader(request.InputStream);
            var json = reader.ReadToEnd();


            var newUser = JsonSerializer.Deserialize<UserDetailsWithPassword>(json, JsonOptions);
            if (newUser != null)
            {
                var password = ConvertToSecureString(newUser.Password);
                newUser.Password = "";
                var playerId = Player.CreateUser(newUser, password);
                response.Write(JsonSerializer.Serialize(playerId));
            }
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
