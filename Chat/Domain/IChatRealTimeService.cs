using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChatBackend.Chat.Domain
{
    /// <summary>
    /// Сервис, предоставляющий real-time операции чата
    /// </summary>
    public interface IChatRealTimeService
    {
        /// <summary>
        /// Возвращает тех получателей сообщения, которые подключены к сервису
        /// </summary>
        /// <param name="message">Сообщение</param>
        IEnumerable<RecieverInfo> GetRealTimeRecievers(Message message);

        /// <summary>
        /// Передаёт новое сообщение всем подключенным получателям
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="cancellationToken"></param>
        Task SendMessageRealTimeAsync(Message message, CancellationToken cancellationToken);

        /// <summary>
        /// Передаёт отправителю сообщения и подключенным получателям информацию о получении сообщения одним или
        /// несколькими получателями
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="recieverInfos">Пользователи, которые получили сообщение</param>
        /// <param name="cancellationToken"></param>
        Task SendMessageIsRecievedAsync(Message message, RecieverInfo[] recieverInfos, CancellationToken cancellationToken);

        /// <summary>
        /// Передаёт подключенным отправителям и получателям о получении одного или нескольких сообщений одним получателем
        /// </summary>
        /// <param name="newMessages">Полученные входящие сообщения</param>
        /// <param name="recieverGuid">Guid получателя</param>
        /// <param name="cancellationToken"></param>
        Task SendMessagesAreRecievedAsync(Message[] newMessages, Guid recieverGuid, CancellationToken cancellationToken);

        /// <summary>
        /// Передаёт всем подключенным получателям (кроме того, кто прочёл сообщение) и отправителю сведения
        /// о прочтении сообщения одним из получателей
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="recieverInfo">Получатель, отметивший сообщение как прочитанное</param>
        /// <param name="cancellationToken"></param>
        Task SendMessageIsReadAsync(Message message, RecieverInfo recieverInfo, CancellationToken cancellationToken);
    }
}