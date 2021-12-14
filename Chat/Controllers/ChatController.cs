using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ChatBackend.Chat.Domain;
using ChatBackend.Chat.Domain.Events;
using ChatBackend.Chat.Domain.Operations;
using ChatBackend.Chat.Domain.Validators;
using ChatBackend.Chat.Hubs;
using ChatBackend.Persons.Domain;
using ChatBackend.Users.Domain;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ChatBackend.Chat.Controllers
{
    /// <summary>
    /// Контроллер чата
    /// </summary>
    [ApiController]
    [Route("chat")]
    [Authorize]
    public sealed class ChatController : Controller, IChatRealTimeService
    {
        private readonly IChatRepository _chatRepository;
        private readonly ILogger<ChatController> _logger;
        private readonly IChatValidatorsRepository _chatValidatorsRepository;
        private readonly IHubContext<ChatHub, IChatHubClient> _hubContext;
        private readonly IPersonsRepository _personsRepository;

        /// <summary>
        /// Создаёт контроллер чата
        /// </summary>
        /// <param name="chatRepository"></param>
        /// <param name="logger"></param>
        /// <param name="chatValidatorsRepository"></param>
        /// <param name="hubContext"></param>
        public ChatController(
            IChatRepository chatRepository,
            ILogger<ChatController> logger,
            IChatValidatorsRepository chatValidatorsRepository,
            IHubContext<ChatHub, IChatHubClient> hubContext,
            IPersonsRepository personsRepository)
        {
            _chatRepository = chatRepository;
            _logger = logger;
            _chatValidatorsRepository = chatValidatorsRepository;
            _hubContext = hubContext;
            _personsRepository = personsRepository;
        }

        private Domain.Chat? Chat()
        {
            if (!HttpContext.User.HasClaim(ClaimTypes.Role, UserRole.User.NormalizedName))
                return null;
            return new(
                Guid.Parse(HttpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value),
                HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.Role)
                    .Select(x => UserRole.GetByNormalizedName(x.Value).Id),
                _chatRepository,
                _chatValidatorsRepository,
                this,
                _personsRepository);
        }

        /// <summary>
        /// Возвращает входящие и исходящие сообщения текущего пользователя с учётом пагинации
        /// </summary>
        /// <param name="pagingSettings">Параметры пагинации</param>
        /// <param name="cancellationToken"></param>
        [HttpPost("messages/get")]
        [ProducesResponseType(typeof(List<Message>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<Message>>> GetMessages(
            PagingSettings pagingSettings,
            CancellationToken cancellationToken)
        {
            var chat = Chat();
            if (chat == null)
                return Unauthorized();
            return Ok(await chat.GetMessagesAsync(pagingSettings, cancellationToken));
        }

        /// <summary>
        /// Посылает сообщение
        /// </summary>
        /// <param name="createMessage">Сообщение</param>
        /// <param name="cancellationToken"></param>
        [HttpPost("messages/send")]
        [ProducesResponseType(typeof(Message), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Message>> SendMessageAsync(
            CreateMessageQuery createMessage,
            CancellationToken cancellationToken)
        {
            // TODO: исключить двойную валидацию входящих параметров запроса

            var chat = Chat();
            if (chat == null)
                return Unauthorized();
            try
            {
                var message = await chat.SendMessageAsync(createMessage, cancellationToken);
                return Created(message.Guid.ToString(), message);
            }
            catch (ValidationException validationException)
            {
                return BadRequest(validationException.Errors);
            }
        }

        /// <summary>
        /// Возвращает количество сообщений (исходящих и входящих) текущего пользователя
        /// </summary>
        /// <param name="cancellationToken"></param>
        [HttpGet("messages/count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<int>> GetMessagesCountAsync(CancellationToken cancellationToken)
        {
            var chat = Chat();
            if (chat == null)
                return Unauthorized();
            return await chat.GetMessagesCountAsync(cancellationToken);
        }

        /// <summary>
        /// Отмечает сообщение как прочитанное и возвращает его
        /// </summary>
        /// <param name="messageGuid">Guid сообщения</param>
        /// <param name="cancellationToken"></param>
        [HttpPost("message/mark-as-read")]
        [ProducesResponseType(typeof(RecieverInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<ValidationFailure>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<RecieverInfo>> MarkMessageAsReadAsync(Guid messageGuid,
            CancellationToken cancellationToken)
        {
            var chat = Chat();
            if (chat == null)
                return Unauthorized();
            try
            {
                return Ok(await chat.MarkMessageAsReadAsync(messageGuid, cancellationToken));
            }
            catch (ValidationException validationException)
            {
                return BadRequest(validationException.Errors);
            }
        }
        
        /// <summary>
        /// Возвращает список подключенных к чату пользователей
        /// </summary>
        /// <param name="cancellationToken"></param>
        [HttpGet("online-users")]
        [ProducesResponseType(typeof(IEnumerable<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IEnumerable<Guid>> GetOnlineUsers(CancellationToken cancellationToken)
        {
            var chat = Chat();
            if (chat == null)
                return Unauthorized();
            return Ok(ChatHub.GetConnectedUsers());
        }

        /// <summary>
        /// Возвращает стандартные шаблоны сообщений, доступные пользователю
        /// Шаблоны отсортированы по Id (по возрастанию)
        /// </summary>
        /// <param name="cancellationToken"></param>
        [HttpGet("message/default-templates")]
        [ProducesResponseType(typeof(IEnumerable<DefaultMessageTemplate>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<DefaultMessageTemplate>>> GetDefaultMessageTemplatesAsync(
            CancellationToken cancellationToken)
        {
            var chat = Chat();
            if (chat == null)
                return Unauthorized();
            return Ok(await chat.GetDefaultMessageTemplatesAsync(cancellationToken));
        }
        
        /// <summary>
        /// Возвращает пользовательские шаблоны сообщений
        /// Шаблоны отсортированы по ModifiedDateTime (по возрастанию)
        /// </summary>
        /// <param name="cancellationToken"></param>
        [HttpGet("message/user-templates")]
        [ProducesResponseType(typeof(IEnumerable<MessageTemplate>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<MessageTemplate>>> GetMessageTemplatesAsync(
            CancellationToken cancellationToken)
        {
            var chat = Chat();
            if (chat == null)
                return Unauthorized();
            return Ok(await chat.GetMessageTemplatesAsync(cancellationToken));
        }
        
        /// <summary>
        /// Возвращает доступные плейсхолдеры для шаблонов сообщений
        /// </summary>
        [HttpGet("message/placeholders")]
        [ProducesResponseType(typeof(Placeholder[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<Placeholder[]> GetPlaceholders()
        {
            var chat = Chat();
            if (chat == null)
                return Unauthorized();
            return Ok(chat.GetPlaceholders());
        }

        /// <summary>
        /// Возвращает текст шаблона сообщения, в котором все плейсхолдеры заменены на данные исходя из
        /// контекста посыльного
        /// </summary>
        [HttpPost("message/fill-placeholders")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<string>> FillPlaceholdersAsync(
            FillPlaceholders fillPlaceholders,
            CancellationToken cancellationToken)
        {
            var chat = Chat();
            if (chat == null)
                return Unauthorized();
            return Ok(await chat.FillPlaceholdersAsync(fillPlaceholders, cancellationToken));
        }

        /// <inheritdoc />
        IEnumerable<RecieverInfo> IChatRealTimeService.GetRealTimeRecievers(Message message)
        {
            var connectedUsersGuids = ChatHub.GetConnectedUsers().ToArray();
            return message.RecieversInfo.Where(
                x => connectedUsersGuids.Contains(x.RecieverGuid));
        }

        /// <inheritdoc />
        Task IChatRealTimeService.SendMessageRealTimeAsync(Message message, CancellationToken cancellationToken) =>
            _hubContext.Clients
                .Users(message.RecieversInfo.Select(x => x.RecieverGuid.ToString()))
                .RecieveNewMessage(message);

        /// <inheritdoc />
        async Task IChatRealTimeService.SendMessageIsRecievedAsync(Message message, RecieverInfo[] recieverInfos,
            CancellationToken cancellationToken)
        {
            await _hubContext.Clients
                .Users(message.RecieversInfo.Select(x => x.RecieverGuid.ToString())
                    .Append(message.SenderInfo.SenderGuid.ToString()))
                .MessagesAreRecieved(recieverInfos.Select(x => new MessageIsRecieved(message.Guid, x.RecieverGuid, x.RecievedDateTime)).ToArray());
        }

        /// <inheritdoc />
        Task IChatRealTimeService.SendMessagesAreRecievedAsync(Message[] newMessages, Guid recieverGuid,
            CancellationToken cancellationToken)
        {
            var recieversGuids = newMessages
                .SelectMany(x => x.RecieversInfo)
                .Select(x => x.RecieverGuid);
            var sendersGuids = newMessages
                .Select(x => x.SenderInfo.SenderGuid);
            var recievedEvents = newMessages.SelectMany(
                    x => x.RecieversInfo,
                    (message, recieverInfo) => new MessageIsRecieved(message.Guid, recieverInfo.RecieverGuid, recieverInfo.RecievedDateTime))
                .Where(x => x.RecieverGuid == recieverGuid)
                .ToArray();
            return _hubContext.Clients
                .Users(recieversGuids
                    .Union(sendersGuids)
                    .Distinct()
                    .Select(guid => guid.ToString()))
                .MessagesAreRecieved(recievedEvents);
        }

        /// <inheritdoc />
        Task IChatRealTimeService.SendMessageIsReadAsync(
            Message message,
            RecieverInfo recieverInfo,
            CancellationToken cancellationToken) =>
            _hubContext.Clients
                .Users(message.RecieversInfo
                    .Select(x => x.RecieverGuid)
                    .Where(x => x != recieverInfo.RecieverGuid)
                    .Append(message.SenderInfo.SenderGuid)
                    .Select(x => x.ToString()))
                .MessageIsMarkedAsRead(new MessageIsRead(message.Guid, recieverInfo.RecieverGuid, recieverInfo.ReadDateTime));
    }
}