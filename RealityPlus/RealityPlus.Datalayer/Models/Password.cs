using System.Security;

namespace RealityPlus.DataLayer.Models
{
    public class Password: IDisposable
    {
        public Byte[] Salt { get; }
        public string Hash { get; private set; }

        public Password(Byte[] salt, string hash)
        {
            Salt = salt;
            Hash = hash;
        }

        public void Dispose()
        {
            for (var s=0;  s<Salt.Length; s++)
            {
                Salt[s] = 0;
            }

            Hash = new string(' ', Hash.Length);
            Hash = string.Empty;
        }
    }
}
