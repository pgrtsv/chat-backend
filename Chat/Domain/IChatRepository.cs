using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;

namespace ChatBackend.Chat.Domain
{
    /// <summary>
    /// Репозиторий чата
    /// </summary>
    public interface IChatRepository
    {
        /// <summary>
        /// Сообщения в чате
        /// </summary>
        public sealed class Messages
        {
            /// <summary>
            /// 
            /// </summary>
            public Messages(Message[] all, Message[] newIncoming)
            {
                All = all;
                NewIncoming = newIncoming;
            }

            /// <summary>
            /// Входящие и исходящие сообщения с учётом параметров пагинации
            /// </summary>
            public Message[] All { get; }
            
            /// <summary>
            /// Новые (ещё не полученные) входящие сообщения
            /// </summary>
            public Message[] NewIncoming { get; }
        }
        
        /// <summary>
        /// Возвращает входящие и исходящие сообщения для указанного пользователя
        /// </summary>
        /// <param name="userGuid">Guid пользователя</param>
        /// <param name="pagingSettings">Параметры пагинации</param>
        /// <param name="cancellationToken"></param>
        Task<Messages> GetMessagesAsync(
            Guid userGuid,
            PagingSettings pagingSettings,
            CancellationToken cancellationToken);

        /// <summary>
        /// Возвращает сообщение с указанным Guid
        /// </summary>
        /// <param name="messageGuid">Guid сообщения</param>
        /// <param name="cancellationToken"></param>
        Task<Message> GetMessageAsync(
            Guid messageGuid,
            CancellationToken cancellationToken);

        /// <summary>
        /// Создаёт в БД новое сообщение
        /// </summary>
        /// <param name="message">Новое сообщение</param>
        /// <param name="cancellationToken"></param>
        Task CreateMessageAsync(
            Message message, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Отмечает в БД все сообщения, отправленные указанным пользователям, как прочитанные
        /// </summary>
        /// <param name="recieverInfos">Данные о получателях</param>
        /// <param name="cancellationToken"></param>
        Task MarkMessagesAsRecievedAsync(
            IEnumerable<RecieverInfo> recieverInfos,
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Отмечает в БД сообщения как прочитанное одним или несколькими получателями
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="recieverInfos">Получатели, прочитавшие сообщение</param>
        /// <param name="cancellationToken"></param>
        Task MarkMessageAsReadAsync(
            Message message,
            IEnumerable<RecieverInfo> recieverInfos,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Возвращает количество входящих и исходящих сообщений пользователя с указанным Guid
        /// </summary>
        /// <param name="userGuid">Guid пользователя чата</param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="ValidationException"></exception>
        Task<int> GetMessagesCountAsync(Guid userGuid, CancellationToken cancellationToken);

        /// <summary>
        /// Возвращает стандартные шаблоны сообщений
        /// </summary>
        /// <param name="rolesIds">Id ролей пользователей</param>
        /// <param name="cancellationToken"></param>
        Task<IEnumerable<DefaultMessageTemplate>> GetDefaultMessageTemplatesAsync(
            IEnumerable<int> rolesIds,
            CancellationToken cancellationToken);

        /// <summary>
        /// Возвращает пользовательские шаблоны сообщений
        /// </summary>
        /// <param name="userGuid">Guid пользователя</param>
        /// <param name="cancellationToken"></param>
        Task<IEnumerable<MessageTemplate>> GetMessageTemplatesAsync(
            Guid userGuid,
            CancellationToken cancellationToken);
    }
}