using System;

namespace ChatBackend.Chat.Domain.Operations
{
    /// <summary>
    /// Отмечает сообщение как прочитанное
    /// </summary>
    public sealed class MarkMessageAsRead
    {
        /// <summary>
        /// Guid сообщения
        /// </summary>
        public Guid MessageGuid { get; set; }
        
        /// <summary>
        /// Guid получателя
        /// </summary>
        public Guid RecieverGuid { get; set; }
    }
}