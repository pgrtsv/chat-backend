using System;

namespace ChatBackend.Chat.Domain.Operations
{
    /// <summary>
    /// Данные для отправки нового сообщения
    /// </summary>
    public sealed class CreateMessage
    {
        /// <summary>
        /// Guid отправителя
        /// </summary>
        public Guid SenderGuid { get; set; }
        
        /// <summary>
        /// Новое сообщение
        /// </summary>
        public CreateMessageQuery? Query { get; set; }
    }
}