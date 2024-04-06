using RealityPlus.Models.Player;

namespace RealityPlus.Test.Clients
{
    internal class UserClient: BaseClient
    {
        protected override string BaseUrl => "http://localhost:9999/user";

        public async Task<Guid?> CreateUser(UserDetailsWithPassword newUser)
        {
            return await Post<UserDetailsWithPassword, Guid>("", newUser, null);
        }

        public async Task<UserDetails?> GetUserDetails(Guid userId)
        {
            return await Get<UserDetails>($"/{userId}", null);
        }
    }
}
