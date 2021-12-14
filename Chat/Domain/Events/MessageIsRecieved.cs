using System;

namespace ChatBackend.Chat.Domain.Events
{
    public sealed class MessageIsRecieved
    {
        public MessageIsRecieved(Guid messageGuid, Guid recieverGuid, DateTimeOffset recievedDateTime)
        {
            MessageGuid = messageGuid;
            RecieverGuid = recieverGuid;
            RecievedDateTime = recievedDateTime;
        }

        public Guid MessageGuid { get; }

        public Guid RecieverGuid { get; }
        
        public DateTimeOffset RecievedDateTime { get; }
    }
}