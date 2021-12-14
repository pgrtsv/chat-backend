using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChatBackend.Chat.Domain.Operations;
using ChatBackend.Chat.Domain.Validators;
using ChatBackend.Persons.Domain;
using FluentValidation;

namespace ChatBackend.Chat.Domain
{
    /// <summary>
    /// Чат пользователя
    /// </summary>
    public sealed class Chat
    {
        private readonly IChatRepository _chatRepository;
        private readonly IChatValidatorsRepository _chatValidatorsRepository;
        private readonly IChatRealTimeService _chatRealTimeService;
        private readonly IPersonsRepository _personsRepository;

        /// <summary>
        /// Guid пользователя
        /// </summary>
        public Guid UserGuid { get; }

        private readonly int[] _rolesIds;

        /// <summary>
        /// Id ролей пользователя
        /// </summary>
        public IReadOnlyCollection<int> RolesIds => _rolesIds;

        /// <summary>
        /// Создаёт чат пользователя
        /// </summary>
        /// <param name="userGuid">Guid пользователя</param>
        /// <param name="rolesIds">Id ролей пользователя</param>
        /// <param name="chatRepository">Репозиторий чата</param>
        /// <param name="chatValidatorsRepository">Репозиторий валидаторов чата</param>
        /// <param name="chatRealTimeService"></param>
        public Chat(
            Guid userGuid,
            IEnumerable<int> rolesIds,
            IChatRepository chatRepository,
            IChatValidatorsRepository chatValidatorsRepository,
            IChatRealTimeService chatRealTimeService,
            IPersonsRepository personsRepository)
        {
            _chatRepository = chatRepository;
            _chatValidatorsRepository = chatValidatorsRepository;
            _chatRealTimeService = chatRealTimeService;
            _personsRepository = personsRepository;
            UserGuid = userGuid;
            _rolesIds = rolesIds.ToArray();
        }

        /// <summary>
        /// Отправляет новое сообщение
        /// </summary>
        /// <param name="createMessageQuery">Новое сообщение</param>
        /// <param name="cancellationToken"></param>
        public async Task<Message> SendMessageAsync(
            CreateMessageQuery createMessageQuery,
            CancellationToken cancellationToken)
        {
            var createMessage = new CreateMessage
            {
                Query = createMessageQuery,
                SenderGuid = UserGuid,
            };
            await new CreateMessageValidator(_chatValidatorsRepository)
                .ValidateAndThrowAsync(createMessage, cancellationToken);
            var message = new Message(
                Guid.NewGuid(),
                new SenderInfo(UserGuid, createMessage.Query.PersonGuid, DateTimeOffset.UtcNow),
                createMessage.Query.RecieversGuids!.Select(recieverGuid => new RecieverInfo(
                    recieverGuid,
                    default,
                    default)),
                createMessage.Query.Content!);
            await _chatRepository.CreateMessageAsync(message, cancellationToken);
            var recievers = _chatRealTimeService.GetRealTimeRecievers(message).ToArray();
            if (recievers.Length != 0)
                await MarkMessageAsRecievedAsync(message, recievers, cancellationToken);
            await _chatRealTimeService.SendMessageRealTimeAsync(message, cancellationToken);
            return message;
        }

        /// <summary>
        /// Возвращает последние входящие и исходящие сообщения пользователя
        /// с учетом параметров пагинации
        /// </summary>
        /// <param name="pagingSettings">Параметры пагинации</param>
        /// <param name="cancellationToken"></param>
        public async Task<Message[]> GetMessagesAsync(
            PagingSettings pagingSettings,
            CancellationToken cancellationToken)
        {
            var messages = await _chatRepository.GetMessagesAsync(
                UserGuid,
                pagingSettings,
                cancellationToken);
            if (messages.NewIncoming.Length != 0)
            {
                await MarkMessagesAsRecievedAsync(messages.NewIncoming,
                    cancellationToken);
            }

            return messages.All;
        }

        /// <summary>
        /// Возвращает количество всех входящих и исходящих сообщений в чате
        /// </summary>
        /// <param name="cancellationToken"></param>
        public Task<int> GetMessagesCountAsync(
            CancellationToken cancellationToken) =>
            _chatRepository.GetMessagesCountAsync(
                UserGuid,
                cancellationToken);

        /// <summary>
        /// Отмечает одно или несколько сообщений как полученные текущим пользователем в real-time
        /// </summary>
        private async Task MarkMessagesAsRecievedAsync(
            IEnumerable<Message> newMessages,
            CancellationToken cancellationToken)
        {
            var messages = newMessages as Message[] ?? newMessages.ToArray();
            var recieverInfos = messages.SelectMany(x => x.RecieversInfo)
                .Where(x => x.RecieverGuid == UserGuid)
                .ToArray();
            var recievedDateTime = DateTimeOffset.UtcNow;
            foreach (var recieverInfo in recieverInfos) recieverInfo.MarkAsRecieved(recievedDateTime);
            if (recieverInfos.Length != 0)
                await _chatRepository.MarkMessagesAsRecievedAsync(recieverInfos, cancellationToken);
            await _chatRealTimeService.SendMessagesAreRecievedAsync(messages, UserGuid, cancellationToken);
        }
        
        /// <summary>
        /// Отмечает одно сообщение как полученное одним или несколькими пользователями в real-time
        /// </summary>
        private async Task MarkMessageAsRecievedAsync(
            Message newMessage,
            IEnumerable<RecieverInfo> recieverInfos,
            CancellationToken cancellationToken)
        {
            var infos = recieverInfos as RecieverInfo[] ?? recieverInfos.ToArray();
            var recievedDateTime = DateTimeOffset.UtcNow;
            foreach (var recieverInfo in infos) recieverInfo.MarkAsRecieved(recievedDateTime);
            await _chatRepository.MarkMessagesAsRecievedAsync(infos, cancellationToken);
            await _chatRealTimeService.SendMessageIsRecievedAsync(newMessage, infos, cancellationToken);
        }

        /// <summary>
        /// Отмечает входящее сообщение как прочитанное пользователем
        /// </summary>
        /// <param name="messageGuid">Guid сообщения</param>
        /// <param name="cancellationToken"></param>
        public async Task<RecieverInfo> MarkMessageAsReadAsync(Guid messageGuid, CancellationToken cancellationToken)
        {
            var operation = new MarkMessageAsRead
            {
                MessageGuid = messageGuid,
                RecieverGuid = UserGuid,
            };
            await new MarkMessageAsReadValidator(_chatValidatorsRepository)
                .ValidateAndThrowAsync(operation, cancellationToken);
            var message = await _chatRepository.GetMessageAsync(messageGuid, cancellationToken);
            var recieverInfo = message.RecieversInfo.First(x => x.RecieverGuid == UserGuid);
            if (recieverInfo.ReadDateTime != default) return recieverInfo;
            recieverInfo.MarkAsRead();
            await _chatRepository.MarkMessageAsReadAsync(message, new[] {recieverInfo}, cancellationToken);
            await _chatRealTimeService.SendMessageIsReadAsync(message, recieverInfo, cancellationToken);
            return recieverInfo;
        }

        /// <summary>
        /// Возвращает стандартные шаблоны сообщений, доступные пользователю
        /// Шаблоны отсортированы по Id (по возрастанию)
        /// </summary>
        /// <param name="cancellationToken"></param>
        public Task<IEnumerable<DefaultMessageTemplate>> GetDefaultMessageTemplatesAsync(
            CancellationToken cancellationToken) =>
            _chatRepository.GetDefaultMessageTemplatesAsync(_rolesIds, cancellationToken);

        /// <summary>
        /// Возвращает пользовательские шаблоны сообщений
        /// Шаблоны отсортированы по ModifiedDateTime (по возрастанию)
        /// </summary>
        /// <param name="cancellationToken"></param>
        public Task<IEnumerable<MessageTemplate>> GetMessageTemplatesAsync(
            CancellationToken cancellationToken) =>
            _chatRepository.GetMessageTemplatesAsync(UserGuid, cancellationToken);

        /// <summary>
        /// Возвращает все доступные посыльному плейсхолдеры шаблона сообщения
        /// </summary>
        public Placeholder[] GetPlaceholders() => Placeholder.GetAll();

        /// <summary>
        /// Возвращает результат применения замен плейсхолдеров к шаблону сообщения
        /// </summary>
        public async Task<string> FillPlaceholdersAsync(
            FillPlaceholders fillPlaceholders,
            CancellationToken cancellationToken)
        {
            var messengerContext = await MessengerContext.NewAsync(
                fillPlaceholders.MessengerContext,
                _chatValidatorsRepository,
                _personsRepository, 
                cancellationToken);
            return Placeholder.GetAll().Aggregate(fillPlaceholders.MessageTemplateContent,
                (text, placeholder) => placeholder.FillPlaceholder(text, messengerContext));
        }
    }
}