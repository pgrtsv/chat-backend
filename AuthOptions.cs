using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ChatBackend
{
    public sealed class AuthOptions
    {
        // TODO: изменить этот класс.
        public const string ISSUER = "chat-backend"; // издатель токена
        public const string AUDIENCE = "chat-backend"; // потребитель токена
        const string KEY = "mysupersecret_secretkey!123";   // ключ для шифрации
        public static SymmetricSecurityKey GetSymmetricSecurityKey() => new(Encoding.ASCII.GetBytes(KEY));
    }
}