using System.Security;

namespace RealityPlus.BusinessLayer
{
    internal interface IPasswordManageer
    {
        public bool ValidatePassword(Guid playerId, SecureString password);
        public void SavePassword(Guid playerId, SecureString password);
    }
}
