using RealityPlus.DataModels.Interfaces;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Text;

namespace RealityPlus.BusinessLayer.BusinessLayer
{
    internal class PasswordManager : IPasswordManageer
    {
        private readonly IPlayerData DataAccess;
        private const int keySize = 64;
        private const int iterations = 350000;
        private HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

        public PasswordManager(IPlayerData dataAccess)
        {
            DataAccess = dataAccess;
        }
        void IPasswordManageer.SavePassword(Guid playerId, SecureString password)
        {
            var enteredPasswordPtr = IntPtr.Zero;
            var enteredPassword = string.Empty;
           
            try
            {
                enteredPasswordPtr = Marshal.SecureStringToGlobalAllocUnicode(password);
                enteredPassword = Marshal.PtrToStringUni(enteredPasswordPtr);
                if (enteredPassword == null)
                {
                    return;
                }

                var salt = RandomNumberGenerator.GetBytes(keySize);
                var hash = Rfc2898DeriveBytes.Pbkdf2(
                    Encoding.UTF8.GetBytes(enteredPassword),
                    salt,
                    iterations,
                    hashAlgorithm,
                    keySize);
               
                DataAccess.UpdatePassword(playerId, new DataLayer.Models.Password(salt, Convert.ToHexString(hash)));
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(enteredPasswordPtr);
                if (!string.IsNullOrWhiteSpace(enteredPassword))
                {
                    enteredPassword = new string(' ', enteredPassword.Length);
                }
            }
        }

        bool IPasswordManageer.ValidatePassword(Guid playerId, SecureString password)
        {
            var stroedPassword = DataAccess.GetStoredPasswordForPlayer(playerId);
            if (stroedPassword == null)
            {
                return false;
            }

            var enteredPasswordPtr = IntPtr.Zero;
            var enteredPassword = string.Empty;

            try
            {
                enteredPasswordPtr = Marshal.SecureStringToGlobalAllocUnicode(password);
                enteredPassword = Marshal.PtrToStringUni(enteredPasswordPtr);
                if (enteredPassword == null)
                {
                    return false;
                }
 
                var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(enteredPassword, stroedPassword.Salt, iterations, hashAlgorithm, keySize);
                return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(stroedPassword.Hash));
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(enteredPasswordPtr);
                if (!string.IsNullOrWhiteSpace(enteredPassword))
                {
                    enteredPassword = new string(' ', enteredPassword.Length);
                }
            }
        }
    }
}
