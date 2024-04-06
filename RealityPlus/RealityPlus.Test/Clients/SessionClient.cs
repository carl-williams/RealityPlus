using RealityPlus.Models.Player;

namespace RealityPlus.Test.Clients
{
    internal class SessionClient : BaseClient
    {
        protected override string BaseUrl => "http://localhost:9999/session";

        public async Task<Guid?> Login(string username, string password)
        {
            var credentials = new LoginUser
            {
                UserName = username,
                Password = password,
            };
            return await Post<LoginUser, Guid?>("", credentials, null);
        }

        public async Task<UserDetails?> GetUserDetails(Guid sessionId)
        {
            var headers =  new Dictionary<string, string>();
            headers.Add("SessionId", sessionId.ToString());

            return await Get<UserDetails>("", headers);
        }

        public async Task Logout(Guid sessionId)
        {
            var headers = new Dictionary<string, string>();
            headers.Add("SessionId", sessionId.ToString());

            await Delete("", headers);
        }
    }
}
