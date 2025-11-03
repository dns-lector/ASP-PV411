namespace ASP_PV411.Services.Kdf
{
    /// <summary>
    /// Key Derivation Function by RFC 2898
    /// https://datatracker.ietf.org/doc/html/rfc2898
    /// </summary>
    public interface IKdfService
    {
        /// <summary>
        /// Derived key form password and salt
        /// </summary>
        String Dk(String password, String salt);
    }
}
