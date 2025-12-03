
namespace ASP_PV411.Services.Storage
{
    public class LocalStorageService : IStorageService
    {
        private readonly String _storagePath = @"C:\storage\ASP411";

        public byte[] Get(string filename)
        {
            String path = Path.Combine(_storagePath, filename);
            if (File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        public string Save(IFormFile formFile)
        {
            if (formFile == null || formFile.Length == 0)
            {
                throw new ArgumentNullException(nameof(formFile));
            }
            // файли неможна зберігати з тим іменем, що приходить по запиту,
            // оскільки можливі конфлікти (вже є такий файл), а також з міркувань безпеки
            // Генеруємо нове ім'я, зберігаючи розширення файлу
            int dotIndex = formFile.FileName.LastIndexOf('.');
            if (dotIndex == -1) {
                throw new ArgumentException("File item must have extension");
            }
            String ext = formFile.FileName[dotIndex..];
            String savedName = Guid.NewGuid().ToString() + ext;
            using var wStream = new StreamWriter(Path.Combine(_storagePath, savedName));
            formFile.CopyTo(wStream.BaseStream);
            return savedName;
        }
    }
}
