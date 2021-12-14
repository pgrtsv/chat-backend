using System;

namespace ChatBackend.Chat.Domain
{
    /// <summary>
    /// Информация о получателе сообщения
    /// </summary>
    public sealed class RecieverInfo
    {
        /// <summary>
        /// Конструктор для сериализаторов
        /// </summary>
        /// <param name="recieverGuid"></param>
        /// <param name="recievedDateTime"></param>
        /// <param name="readDateTime"></param>
        [ForbidUsage]
        public RecieverInfo(Guid recieverGuid, DateTimeOffset recievedDateTime, DateTimeOffset readDateTime)
        {
            RecieverGuid = recieverGuid;
            RecievedDateTime = recievedDateTime;
            ReadDateTime = readDateTime;
        }

        /// <summary>
        /// Guid получателя
        /// </summary>
        public Guid RecieverGuid { get; }
        
        /// <summary>
        /// Дата и время получения сообщения. Default, если сообщение ещё не было получено
        /// </summary>
        public DateTimeOffset RecievedDateTime { get; private set; }

        /// <summary>
        /// Помечает сообщение как полученное
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        [ForbidUsage]
        public void MarkAsRecieved(DateTimeOffset recievedDateTime)
        {
            if (RecievedDateTime != default)
                throw new InvalidOperationException("One message cannot be marked as recieved twice");
            RecievedDateTime = recievedDateTime;
        }
        
        /// <summary>
        /// Дата и время отметки сообщения как прочтённого. Default, если сообщение ещё не было прочтено
        /// </summary>
        public DateTimeOffset ReadDateTime { get; private set; }

        /// <summary>
        /// Помечает сообщение как прочитанное получателем
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void MarkAsRead()
        {
            if (ReadDateTime != default)
                throw new InvalidOperationException("One message cannot be marked as read twice");
            ReadDateTime = DateTimeOffset.UtcNow;
        }
    }
}