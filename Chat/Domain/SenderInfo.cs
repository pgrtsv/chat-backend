using System;

namespace ChatBackend.Chat.Domain
{
    /// <summary>
    /// Информация об отправителе сообщения
    /// </summary>
    public sealed class SenderInfo
    {
        /// <summary>
        /// Конструктор для ORM
        /// </summary>
        /// <param name="senderGuid"></param>
        /// <param name="personGuid"></param>
        /// <param name="sendDateTime"></param>
        [ForbidUsage]
        public SenderInfo(Guid senderGuid, Guid personGuid, DateTimeOffset sendDateTime)
        {
            SenderGuid = senderGuid;
            PersonGuid = personGuid;
            SendDateTime = sendDateTime;
        }

        /// <summary>
        /// Guid отправителя сообщения
        /// </summary>
        public Guid SenderGuid { get; }
        
        /// <summary>
        /// Guid военнослужащего, отправившего сообщение
        /// </summary>
        public Guid PersonGuid { get; }
        
        /// <summary>
        /// Дата отправки сообщения
        /// </summary>
        public DateTimeOffset SendDateTime { get; }
    }
}