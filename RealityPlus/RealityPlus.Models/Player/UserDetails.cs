using RealityPlus.Models.Common;

namespace RealityPlus.Models.Player
{
    public class UserDetails
    {
        public string UserName { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public ERegion Region { get; set; }
        public int Rank { get; set; } = 0;
        public string Email { get; set; } = "";
    }
}
