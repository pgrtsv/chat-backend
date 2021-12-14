using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatBackend.Chat.Domain
{
    /// <summary>
    /// Сообщение
    /// </summary>
    public sealed class Message
    {
        /// <summary>
        /// Конструктор для сериализаторов
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="senderInfo"></param>
        /// <param name="recieversInfo"></param>
        /// <param name="content"></param>
        [ForbidUsage]
        public Message(
            Guid guid,
            SenderInfo senderInfo, 
            IEnumerable<RecieverInfo> recieversInfo, 
            string content)
        {
            Guid = guid;
            SenderInfo = senderInfo;
            _recieversInfo = recieversInfo.ToList();
            Content = content;
        }
        
        /// <summary>
        /// Guid сообщения
        /// </summary>
        public Guid Guid { get; }

        /// <summary>
        /// Информация об отправителе сообщения
        /// </summary>
        public SenderInfo SenderInfo { get; }
        
        /// <summary>
        /// Информация о получателях сообщения
        /// </summary>
        public IEnumerable<RecieverInfo> RecieversInfo => _recieversInfo;

        private readonly List<RecieverInfo> _recieversInfo;
        
        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string Content { get; }
        
        private bool Equals(Message other) => Guid.Equals(other.Guid);

        /// <inheritdoc />
        public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Message other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Guid.GetHashCode();
    }
}