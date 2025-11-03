namespace ASP_PV411.Services.Signature
{
    /// <summary>
    /// Сервіс цифрового підпису 
    /// </summary>
    public interface ISignatureService
    {
        String Sign(String data, String password);
    }
}
