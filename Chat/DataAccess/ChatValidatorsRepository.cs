using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChatBackend.Chat.Domain.Validators;
using Dapper;
using Npgsql;

namespace ChatBackend.Chat.DataAccess
{
    /// <summary>
    /// Репозиторий валидаторов чата
    /// </summary>
    public sealed class ChatValidatorsRepository : IChatValidatorsRepository
    {
        private readonly NpgsqlConnection _dataConnection;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataConnection"></param>
        public ChatValidatorsRepository(NpgsqlConnection dataConnection) => _dataConnection = dataConnection;

        /// <inheritdoc />
        public Task<bool> DoUsersWithGuidsExistAsync(
            IEnumerable<Guid> usersGuids,
            CancellationToken cancellationToken = default)
        {
            var guids = usersGuids.ToArray();
            return _dataConnection.QueryFirstAsync<bool>(new CommandDefinition(@"
SELECT COUNT(guid) = @count FROM chat_user WHERE guid = ANY(@usersGuids);
", new {usersGuids = guids, count = guids.Length}, cancellationToken: cancellationToken));
        }

        /// <inheritdoc />
        public Task<bool> DoesMessageWithGuidExistAsync(
            Guid messageGuid,
            CancellationToken cancellationToken = default) =>
            _dataConnection.QueryFirstAsync<bool>(new CommandDefinition(@"
SELECT count(guid) > 0 FROM message WHERE guid = @messageGuid
", new {messageGuid}, cancellationToken: cancellationToken));

        /// <inheritdoc />
        public Task<bool> IsMessageRecievedAsync(
            Guid messageGuid,
            Guid recieverGuid,
            CancellationToken cancellationToken = default) =>
            _dataConnection.QueryFirstAsync<bool>(new CommandDefinition(@"
SELECT recieved_datetime <> @defaultDateTime 
FROM reciever_info 
WHERE message_guid = @messageGuid AND reciever_guid = @recieverGuid",
                new
                {
                    defaultDateTime = default(DateTimeOffset),
                    messageGuid,
                    recieverGuid,
                },
                cancellationToken: cancellationToken));

        /// <inheritdoc />
        public Task<bool> DoesMessageHaveReciever(
            Guid messageGuid,
            Guid recieverGuid,
            CancellationToken cancellationToken = default) =>
            _dataConnection.QueryFirstAsync<bool>(new CommandDefinition(@"
SELECT COUNT(reciever_guid) > 0
FROM reciever_info 
WHERE message_guid = @messageGuid AND reciever_guid = @recieverGuid",
                new {messageGuid, recieverGuid},
                cancellationToken: cancellationToken));

        /// <inheritdoc />
        public Task<bool> DoesPersonWithGuidExistAsync(
            Guid personGuid,
            CancellationToken cancellationToken = default) => _dataConnection.QueryFirstAsync<bool>(
            new CommandDefinition(@"
SELECT COUNT(guid) > 0 FROM person WHERE guid = @personGuid",
                new
                {
                    personGuid,
                },
                cancellationToken: cancellationToken));
    }
}