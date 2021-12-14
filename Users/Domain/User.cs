using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatBackend.Users.Domain.Operations;
using ChatBackend.Users.Domain.Validators;
using FluentValidation;
using SecurityDriven.Inferno;
using SecurityDriven.Inferno.Hash;

namespace ChatBackend.Users.Domain
{
    /// <summary>
    /// Пользователь чата
    /// </summary>
    public sealed class User
    {
        /// <summary>
        /// Конструктор для сериализаторов
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="name"></param>
        /// <param name="shortName"></param>
        /// <param name="login"></param>
        /// <param name="passwordHash"></param>
        /// <param name="userRoles"></param>
        [ForbidUsage]
        public User(
            Guid guid,
            string name,
            string shortName,
            string login,
            string passwordHash,
            IEnumerable<UserRole> userRoles)
        {
            Guid = guid;
            Name = name;
            ShortName = shortName;
            Login = login;
            PasswordHash = passwordHash;
            _userRoles = userRoles.ToList();
        }

        /// <summary>
        /// Guid пользователя
        /// </summary>
        public Guid Guid { get; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Короткое имя пользователя
        /// </summary>
        public string ShortName { get; }

        /// <summary>
        /// Логин пользователя
        /// </summary>
        public string Login { get; }

        /// <summary>
        /// Хэш пароля пользователя
        /// </summary>
        public string PasswordHash { get; }

        /// <summary>
        /// Роли пользователя
        /// </summary>
        public IEnumerable<UserRole> UserRoles => _userRoles;

        private readonly List<UserRole> _userRoles;

        /// <summary>
        /// Создаёт нового пользователя
        /// </summary>
        /// <param name="createUser">Данные нового пользователя</param>
        /// <param name="userValidatorsService">Сервис валидаторов пользователей</param>
        /// <exception cref="ValidationException"></exception>
        public static async Task<User> NewAsync(
            CreateUser createUser,
            IUserValidatorsService userValidatorsService)
        {
            await new CreateUserValidator(userValidatorsService)
                .ValidateAndThrowAsync(createUser);
            return new User(
                Guid.NewGuid(),
                createUser.Name!,
                createUser.ShortName!,
                createUser.Login!,
                GetPasswordHash(createUser.Password!),
                createUser.Roles.Select(UserRole.GetById)
            );
        }

        /// <summary>
        /// Возвращает хэш пароля
        /// </summary>
        /// <param name="password">Пароль</param>
        public static string GetPasswordHash(string password) => BitConverter.ToString(HashFactories.SHA512()
            .ComputeHash(Utils.SafeUTF8.GetBytes(password)));
    }
}