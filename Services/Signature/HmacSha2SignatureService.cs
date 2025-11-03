using Microsoft.AspNetCore.Authentication;

namespace ASP_PV411.Services.Signature
{
    /// <summary>
    /// Сервіс цифрового підпису на базі HMAC SHA-256 
    /// Appendix A.1.1. RFC 7515
    /// https://datatracker.ietf.org/doc/html/rfc7515#appendix-A.1.1
    /// </summary>
    public class HmacSha2SignatureService : ISignatureService
    {
        public string Sign(string data, string password)
        {
            return Base64UrlTextEncoder.Encode(
                System.Security.Cryptography.HMACSHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes(password),
                    System.Text.Encoding.UTF8.GetBytes(data)));

        }
    }
}
