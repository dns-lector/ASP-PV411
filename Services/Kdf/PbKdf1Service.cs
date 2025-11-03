using ASP_PV411.Services.Hash;

namespace ASP_PV411.Services.Kdf
{
    /// <summary>
    /// Key Derivation Function by sec 5.1 RFC 2898
    /// </summary>
    public class PbKdf1Service(IHashService hashService) : IKdfService
    {
        private readonly int iterationsCount = 1000;
        private readonly int dkLength = 16;
        public string Dk(string password, string salt)
        {
            String t = hashService.Digest(password + salt);
            for (int i = 1; i < iterationsCount; i++)
            {
                t = hashService.Digest(t);
            }
            return t[..dkLength];
        }
    }
}
