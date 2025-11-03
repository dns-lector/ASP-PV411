namespace ASP_PV411.Services.Salt
{
    /// <summary>
    /// Криптографічна сіль - рядок випадкових символів для 
    /// задач перетворення паролів при збереженні їх до БД
    /// </summary>
    public interface ISaltService
    {
        string GetSalt(int? length = null);
    }
}
