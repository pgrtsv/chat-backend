using System.Collections.Generic;

namespace ChatBackend.Users.Domain.Operations
{
    /// <summary>
    /// Создаёт нового пользователя чата.
    /// </summary>
    public sealed class CreateUser
    {
        /// <summary>
        /// Имя пользователя.
        /// </summary>
        /// <example>Посыльный 1 научной роты</example>
        public string? Name { get; set; }
        
        /// <summary>
        /// Короткое имя пользователя.
        /// </summary>
        /// <example>1 НР</example>
        public string? ShortName { get; set; }
        
        /// <summary>
        /// Логин пользователя.
        /// </summary>
        /// <example>era1</example>
        public string? Login { get; set; }
        
        /// <summary>
        /// Пароль пользователя.
        /// </summary>
        public string? Password { get; set; }
        
        /// <summary>
        /// Роли пользователя.
        /// </summary>
        public List<int> Roles { get; set; }
    }
}