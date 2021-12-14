using System;
using System.Collections.Generic;

namespace ChatBackend.Chat.Domain.Operations
{
    /// <summary>
    /// Данные для отправки сообщения
    /// </summary>
    public sealed class CreateMessageQuery
    {
        /// <summary>
        /// Список Guid получателей сообщения
        /// </summary>
        public List<Guid>? RecieversGuids { get; set; }
        
        /// <summary>
        /// Guid военнослужащего, отправляющего сообщение
        /// </summary>
        public Guid PersonGuid { get; set; }
        
        /// <summary>
        /// Текст сообщения
        /// </summary>
        /// <example>Текст сообщения.</example>
        public string? Content { get; set; }
    }
}