namespace ASP_PV411.Services.Storage
{
    public interface IStorageService
    {
        byte[] Get(String filename);
        String Save(IFormFile formFile);
    }
}
