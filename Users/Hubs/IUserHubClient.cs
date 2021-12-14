using System.Threading.Tasks;
using ChatBackend.Users.DataTransfer;

namespace ChatBackend.Users.Hubs
{
    /// <summary>
    /// Клиент <see cref="UserHub"/>
    /// </summary>
    public interface IUserHubClient
    {
        /// <summary>
        /// Получает данные о зарегистрированном пользователе
        /// </summary>
        /// <param name="user">Новый пользователь</param>
        Task RecieveNewUser(UserDto user);
    }
}