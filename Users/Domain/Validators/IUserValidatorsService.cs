using System.Threading;
using System.Threading.Tasks;

namespace ChatBackend.Users.Domain.Validators
{
    /// <summary>
    /// Сервис валидаторов пользователей
    /// </summary>
    public interface IUserValidatorsService
    {
        /// <summary>
        /// Возвращает true, если пользователь с указанным логином существует
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <param name="cancellationToken"></param>
        Task<bool> CheckIfLoginExistsAsync(string login, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Возвращает true, если пользователь с указанным именем существует
        /// </summary>
        /// <param name="username">Имя пользователя</param>
        /// <param name="cancellationToken"></param>
        Task<bool> CheckIfUsernameExistsAsync(string username, CancellationToken cancellationToken = default);

        /// <summary>
        /// Возвращает true, если пользователь с указанным коротким именем существует
        /// </summary>
        /// <param name="shortName">Короткое имя пользователя</param>
        /// <param name="cancellationToken"></param>
        Task<bool> CheckIfShortNameExistsAsync(string shortName, CancellationToken cancellationToken = default);
    }
}