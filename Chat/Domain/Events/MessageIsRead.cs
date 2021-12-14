using System;

namespace ChatBackend.Chat.Domain.Events
{
    public sealed class MessageIsRead
    {
        public MessageIsRead(Guid messageGuid, Guid recieverGuid, DateTimeOffset readDateTime)
        {
            MessageGuid = messageGuid;
            RecieverGuid = recieverGuid;
            ReadDateTime = readDateTime;
        }

        public Guid MessageGuid { get; }
        
        public Guid RecieverGuid { get; }
        
        public DateTimeOffset ReadDateTime { get; }
    }
}