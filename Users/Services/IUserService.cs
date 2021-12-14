using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ChatBackend.Users.DataTransfer;
using ChatBackend.Users.Domain.Operations;

namespace ChatBackend.Users.Services
{
    /// <summary>
    /// Сервис пользователей чата
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Регистрирует нового пользователя чата
        /// </summary>
        /// <param name="createUser">Данные нового пользователя</param>
        /// <param name="cancellationToken"></param>
        Task<UserDto> CreateAsync(CreateUser createUser, CancellationToken cancellationToken);

        /// <summary>
        /// Возвращает всех пользователей чата
        /// </summary>
        /// <param name="cancellationToken"></param>
        Task<IEnumerable<UserDto>> GetAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Возвращает пользователя с указанным Guid
        /// </summary>
        /// <param name="guid">Guid пользователя</param>
        /// <param name="cancellationToken"></param>
        Task<UserDto> GetByGuidAsync(Guid guid, CancellationToken cancellationToken);

        /// <summary>
        /// Возвращает true, если пользователь с указанным Guid существует
        /// </summary>
        /// <param name="guid">Guid пользователя</param>
        /// <param name="cancellationToken"></param>
        Task<bool> CheckIfGuidExistsAsync(Guid guid, CancellationToken cancellationToken);

        /// <summary>
        /// Аутентифицирует пользователя чата
        /// Возвращает null, если указанной пары логин-пароль не существует
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        /// <param name="cancellationToken"></param>
        Task<UserDto?> LogInAsync(string login, string password, CancellationToken cancellationToken);
    }
}