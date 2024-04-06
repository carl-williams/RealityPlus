using RealityPlus.Models.Player;

namespace RealityPlus.Test.Clients
{
    internal class UserClient: BaseClient
    {
        protected override string BaseUrl => "http://localhost:9999/user";

        public async Task<Guid?> CreateUser(UserDetailsWithPassword newUser)
        {
            return await PostMessage<UserDetailsWithPassword, Guid>("", newUser);
        }

        public async Task<UserDetails?> GetUserDetails(Guid userId)
        {
            return await GetMessage<UserDetails>($"/{userId}");
        }
    }
}
