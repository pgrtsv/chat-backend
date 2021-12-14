using System;
using System.Threading.Tasks;
using ChatBackend.Chat.Domain;
using ChatBackend.Chat.Domain.Events;

namespace ChatBackend.Chat.Hubs
{
    /// <summary>
    /// Клиент <see cref="ChatHub"/>
    /// </summary>
    public interface IChatHubClient
    {
        /// <summary>
        /// Получает новое входящее сообщение
        /// </summary>
        /// <param name="message">Новое входящее сообщение</param>
        Task RecieveNewMessage(Message message);

        /// <summary>
        /// Вызывается у отправителя и получателей сообщения, когда оно отмечено как прочитанное
        /// Не вызывается у получателя, который отметил сообщение как прочитанное
        /// </summary>
        Task MessageIsMarkedAsRead(MessageIsRead messageIsRead);

        /// <summary>
        /// Вызывается у отправителей и получателей, когда одно или несколько сообщений из чата получены одним или
        /// несколькими пользователями
        /// Также вызывается и у тех пользователей, которые получили сообщение
        /// </summary>
        Task MessagesAreRecieved(MessageIsRecieved[] messagesAreRecieved);
        
        /// <summary>
        /// Вызывается при подключении или пользователя к хабу чата
        /// </summary>
        /// <param name="userGuid">Guid пользователя</param>
        Task UserHasConnected(Guid userGuid);

        /// <summary>
        /// Вызывается при отключении пользователя от хаба чата
        /// </summary>
        /// <param name="userGuid">Guid пользователя</param>
        Task UserHasDisconnected(Guid userGuid);
    }
}