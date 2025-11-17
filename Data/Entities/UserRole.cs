namespace ASP_PV411.Data.Entities
{
    public class UserRole
    {
        public String Id { get; set; } = null!;
        public String Description { get; set; } = null!;
        public int CanCreate { get; set; }
        public int CanRead { get; set; }
        public int CanUpdate { get; set; }
        public int CanDelete { get; set; }
    }
}
/* Задача роботи з користувачами: реєстрація, автентифікація, авторизація
 * автентифікація: перевірка особи і видача токена (посвідчення)
 * авторизація: перевірка токена та прийняття рішення про допуск чи відмову у допуску
 * 
 * [UserRoles]            [Users]
 *  Id            ---      Id
 *  Description     |      Name            кумулятивне поле для персональних даних
 *  CanCreate  int  |      Email           ... для обов'язкових даних
 *  CanRead    int  |      Birthdate       ... для опціональних даних
 *  CanUpdate  int  |      Login (Unique)
 *  CanDelete  int  |      Salt
 *                  |      Dk
 * [Tokens]         |      RegisterAt          Д.З. Додати класи-сутності з розширеними даними користувачів, включити їх до контексту
 *  Id              |      DeleteAt            [ExtraData]                 [UserData]
 *  UserId          |----  RoleId               Id                           UserId
 *  IssuedAt                                    Name (Ex,розмір взуття)      ExtraId  
 *  ExpiredAt                                                                Value 
 */
