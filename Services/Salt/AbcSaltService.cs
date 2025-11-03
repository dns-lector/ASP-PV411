using ASP_PV411.Services.Random;

namespace ASP_PV411.Services.Salt
{
    // Сервіс генерації солі, що складається з малих літер абетики (коди 97 - 122)
    public class AbcSaltService(IRandomService randomService) : ISaltService
    {
        // сервіси можуть інжектувати інші сервіси
        private readonly IRandomService _randomService = randomService;

        public string GetSalt(int? length = null)
        {
            // ентропія за замовчанням - не менше 64 біти, що відповідає 13-14 символів
            // беремо 16
            length ??= 16;
            if(length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }
            char[] chars = new char[length.Value];
            for (int i = 0; i < length; i++)
            {
                chars[i] = (char)(97 + _randomService.RandomInt() % 26);
            }
            return new string(chars);
        }
    }
}
