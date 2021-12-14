using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChatBackend.Chat.Domain;
using Dapper;
using Npgsql;

namespace ChatBackend.Chat.DataAccess
{
    /// <summary>
    /// Репозиторий чата
    /// </summary>
    public sealed class ChatRepository : IChatRepository
    {
        private readonly NpgsqlConnection _dataConnection;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataConnection"></param>
        public ChatRepository(NpgsqlConnection dataConnection)
        {
            _dataConnection = dataConnection;
        }

        private async Task<Dictionary<Guid, List<RecieverInfo>>> GetRecieversInfosAsync(
            List<Guid> messagesGuids,
            CancellationToken cancellationToken)
        {
            if (messagesGuids.Count == 0) return new();
            var recieverInfos = await _dataConnection
                .QueryAsync(new CommandDefinition(@"
SELECT message_guid, reciever_guid, read_datetime, recieved_datetime 
FROM reciever_info
WHERE message_guid = ANY(@messagesGuids)",
                    new {messagesGuids},
                    cancellationToken: cancellationToken));
            return recieverInfos.GroupBy(x => (Guid) x.message_guid).ToDictionary(
                group => group.Key,
                group => group.Select(x => new RecieverInfo(
                        x.reciever_guid,
                        x.recieved_datetime,
                        x.read_datetime))
                    .ToList()
            );
        }

        /// <inheritdoc />
        public async Task<IChatRepository.Messages> GetMessagesAsync(
            Guid userGuid,
            PagingSettings pagingSettings,
            CancellationToken cancellationToken)
        {
            var messages = (await _dataConnection.QueryAsync(new CommandDefinition(@"
SELECT DISTINCT guid, content, sender_guid, send_datetime, person_guid
FROM message, reciever_info
WHERE message_guid = message.guid AND (sender_guid = @userGuid OR reciever_guid = @userGuid)
ORDER BY send_datetime DESC
LIMIT @pageSize OFFSET (@page - 1) * @pageSize;",
                new {userGuid, pageSize = pagingSettings.PageSize, page = pagingSettings.Page},
                cancellationToken: cancellationToken))).AsList();
            var recieverInfos = await GetRecieversInfosAsync(
                messages.Select(x => (Guid) x.guid).ToList(),
                cancellationToken);
            var ret = messages.Select(x => new Message(
                    x.guid,
                    new SenderInfo(x.sender_guid,  x.person_guid, x.send_datetime),
                    recieverInfos[x.guid],
                    x.content
                ))
                .ToArray();
            var newMessages = await GetNewIncomingMessagesAsync(userGuid, ret, cancellationToken);
            return new IChatRepository.Messages(ret, newMessages);
        }

        /// <inheritdoc />
        public async Task<Message> GetMessageAsync(Guid messageGuid, CancellationToken cancellationToken)
        {
            var message = await _dataConnection.QueryFirstAsync(new CommandDefinition(@"
SELECT DISTINCT guid, content, sender_guid, send_datetime, person_guid
FROM message
WHERE guid = @messageGuid",
                new {messageGuid},
                cancellationToken: cancellationToken));
            var recieverInfos = await GetRecieversInfosAsync(
                new List<Guid> {messageGuid},
                cancellationToken);
            return new Message(
                message.guid,
                new SenderInfo(message.sender_guid, message.person_guid, message.send_datetime),
                recieverInfos[message.guid],
                message.content
            );
        }

        private async Task<Message[]> GetNewIncomingMessagesAsync(Guid userGuid,
            Message[] alreadyRecievedMessages,
            CancellationToken cancellationToken)
        {
            var messages = (await _dataConnection.QueryAsync(new CommandDefinition(@"
SELECT DISTINCT guid, content, sender_guid, send_datetime, person_guid
FROM message, reciever_info
WHERE message_guid = message.guid AND reciever_guid = @userGuid
  AND recieved_datetime = @defaultDateTime
ORDER BY send_datetime DESC",
                new {userGuid, defaultDateTime = default(DateTimeOffset)},
                cancellationToken: cancellationToken))).AsList();
            var recieverInfos = await GetRecieversInfosAsync(
                messages.Select(x => (Guid) x.guid).ToList(),
                cancellationToken);
            return messages.Select(x => new Message(
                    x.guid,
                    new SenderInfo(x.sender_guid, x.person_guid, x.send_datetime),
                    recieverInfos[x.guid],
                    x.content
                ))
                .Select(message => alreadyRecievedMessages.FirstOrDefault(x => x.Guid == message.Guid) ?? message)
                .ToArray();
        }

        /// <inheritdoc />
        public async Task CreateMessageAsync(Message message, CancellationToken cancellationToken = default)
        {
            await _dataConnection.ExecuteAsync(new CommandDefinition(@"
INSERT INTO message (guid, content, sender_guid, send_datetime, person_guid) 
VALUES (@Guid, @Content, @SenderGuid, @SendDateTime, @PersonGuid);
", new
            {
                message.Guid,
                message.Content,
                message.SenderInfo.SenderGuid,
                message.SenderInfo.SendDateTime,
                message.SenderInfo.PersonGuid,
            }, cancellationToken: cancellationToken));
            await _dataConnection.ExecuteAsync(new CommandDefinition(@"
INSERT INTO reciever_info (message_guid, reciever_guid, recieved_datetime, read_datetime) 
VALUES (@Guid, @RecieverGuid, @RecievedDateTime, @ReadDateTime)",
                message.RecieversInfo.Select(x => new
                {
                    message.Guid,
                    x.RecieverGuid,
                    x.RecievedDateTime,
                    x.ReadDateTime,
                }).ToArray(),
                cancellationToken: cancellationToken));
        }

        /// <inheritdoc />
        public async Task MarkMessagesAsRecievedAsync(
            IEnumerable<RecieverInfo> recieverInfos,
            CancellationToken cancellationToken = default)
        {
            var recievers = recieverInfos as RecieverInfo[] ?? recieverInfos.ToArray();
            if (recievers.Length == 0) throw new InvalidOperationException();
            var recievedDateTime = recievers[0].RecievedDateTime;
            if (recievedDateTime == default || recievers.Any(x => x.RecievedDateTime != recievedDateTime))
                throw new InvalidOperationException();
            await _dataConnection.ExecuteAsync(new CommandDefinition(@"
UPDATE reciever_info SET recieved_datetime = @datetime WHERE reciever_guid = ANY(@recieversGuids) AND recieved_datetime = @defaultDateTime",
                new
                {
                    datetime = recievedDateTime,
                    recieversGuids = recievers.Select(x => x.RecieverGuid).Distinct().ToArray(),
                    defaultDateTime = default(DateTimeOffset),
                },
                cancellationToken: cancellationToken));
        }

        /// <inheritdoc />
        public async Task MarkMessageAsReadAsync(
            Message message,
            IEnumerable<RecieverInfo> recieverInfos,
            CancellationToken cancellationToken = default)
        {
            var recievers = recieverInfos as RecieverInfo[] ?? recieverInfos.ToArray();
            if (recievers.Length == 0) return;
            var readDateTime = recievers[0].ReadDateTime;
            if (readDateTime == default || recievers.Any(x => x.ReadDateTime == default))
                throw new InvalidOperationException("readDateTime is default or different in provided recieverInfos.");
            foreach (var recieverInfo in recievers)
            {
                await _dataConnection.ExecuteAsync(new CommandDefinition(@"
UPDATE reciever_info SET read_datetime = @datetime WHERE reciever_guid = @recieverGuid AND message_guid = @messageGuid",
                    new
                    {
                        datetime = readDateTime,
                        recieverGuid = recieverInfo.RecieverGuid,
                        messageGuid = message.Guid,
                    },
                    cancellationToken: cancellationToken));
            }
        }

        /// <inheritdoc />
        public Task<int> GetMessagesCountAsync(Guid userGuid, CancellationToken cancellationToken) =>
            _dataConnection.QueryFirstAsync<int>(new CommandDefinition(@"
SELECT COUNT(x.guid) FROM (SELECT DISTINCT guid 
FROM message, reciever_info
WHERE guid = reciever_info.message_guid AND (sender_guid = @userGuid OR reciever_guid = @userGuid)) AS x",
                new {userGuid},
                cancellationToken: cancellationToken));

        /// <inheritdoc />
        public Task<IEnumerable<DefaultMessageTemplate>> GetDefaultMessageTemplatesAsync(
            IEnumerable<int> rolesIds,
            CancellationToken cancellationToken) =>
            _dataConnection.QueryAsync<DefaultMessageTemplate>(new CommandDefinition(@"
SELECT id, role_id as roleId, content 
FROM default_message_template 
WHERE role_id = ANY(@rolesIds)
ORDER BY id",
                new {rolesIds = rolesIds.ToArray()},
                cancellationToken: cancellationToken));

        /// <inheritdoc />
        public Task<IEnumerable<MessageTemplate>> GetMessageTemplatesAsync(
            Guid userGuid,
            CancellationToken cancellationToken) =>
            _dataConnection.QueryAsync<MessageTemplate>(new CommandDefinition(@"
SELECT guid, user_guid, modified_datetime, content 
FROM user_message_template 
WHERE user_guid = @userGuid
ORDER BY modified_datetime",
                new {userGuid},
                cancellationToken: cancellationToken));
    }
}