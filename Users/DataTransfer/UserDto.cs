using System;
using System.Collections.Generic;
using ChatBackend.Users.Domain;

namespace ChatBackend.Users.DataTransfer
{
    /// <summary>
    /// Данные о пользователе чата
    /// </summary>
    public sealed class UserDto
    {
        /// <summary>
        /// Guid пользователя
        /// </summary>
        public Guid Guid { get; set; }
        
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string? Name { get; set; }
        
        /// <summary>
        /// Короткое имя пользователя
        /// </summary>
        public string? ShortName { get; set; }
        
        /// <summary>
        /// Логин пользователя
        /// </summary>
        public string? Login { get; set; }
        
        /// <summary>
        /// Роли пользователя
        /// </summary>
        public IEnumerable<UserRole>? Roles { get; set; }
    }
}