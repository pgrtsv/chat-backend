using System.Collections.Generic;

namespace ChatBackend.Users.Domain.Operations
{
    /// <summary>
    /// Создаёт посыльного
    /// </summary>
    public sealed class CreateMessenger
    {
        /// <summary>
        /// Номер роты.
        /// </summary>
        /// <example>1</example>
        public int? Company { get; set; }

        /// <summary>
        /// Пароль пользователя.
        /// </summary>
        public string? Password { get; set; }

        public CreateUser ToCreateUser() => new()
        {
            Login = $"era{Company}",
            Roles = new List<int> {UserRole.User.Id, UserRole.Messenger.Id},
            Name = $"Посыльный {Company} научной роты",
            ShortName = $"{Company} НР",
            Password = Password,
        };
    }
}