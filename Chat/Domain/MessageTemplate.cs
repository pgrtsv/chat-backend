using System;

namespace ChatBackend.Chat.Domain
{
    /// <summary>
    /// Пользовательский шаблон сообщения
    /// </summary>
    public sealed class MessageTemplate
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="userGuid"></param>
        /// <param name="modifiedDateTime"></param>
        /// <param name="content"></param>
        [ForbidUsage]
        public MessageTemplate(Guid guid, Guid userGuid, DateTimeOffset modifiedDateTime, string content)
        {
            Guid = guid;
            UserGuid = userGuid;
            ModifiedDateTime = modifiedDateTime;
            Content = content;
        }

        /// <summary>
        /// Guid шаблона
        /// </summary>
        public Guid Guid { get; }
        
        /// <summary>
        /// Guid пользователя, создавшего шаблон
        /// </summary>
        public Guid UserGuid { get; }
        
        /// <summary>
        /// Дата и время последнего изменения шаблона
        /// </summary>
        public DateTimeOffset ModifiedDateTime { get; }
        
        /// <summary>
        /// Содержимое шаблона
        /// </summary>
        public string Content { get; }
    }
}