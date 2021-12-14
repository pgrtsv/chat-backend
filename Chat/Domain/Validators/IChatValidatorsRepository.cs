using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChatBackend.Chat.Domain.Validators
{
    /// <summary>
    /// Репозиторий валидаторов чата
    /// </summary>
    public interface IChatValidatorsRepository
    {
        /// <summary>
        /// Возвращает true, если все пользователи с указанными Guid существуют
        /// </summary>
        /// <param name="usersGuids">Guid пользователей</param>
        /// <param name="cancellationToken"></param>
        Task<bool> DoUsersWithGuidsExistAsync(
            IEnumerable<Guid> usersGuids, 
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Возвращает true, если сообщение с указанным Guid существует
        /// </summary>
        /// <param name="messageGuid">Guid сообщения</param>
        /// <param name="cancellationToken"></param>
        Task<bool> DoesMessageWithGuidExistAsync(
            Guid messageGuid, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Возвращает true, если указанное сообщение получено указанным пользователем
        /// </summary>
        /// <param name="messageGuid">Guid сообщения</param>
        /// <param name="recieverGuid">Guid получателя</param>
        /// <param name="cancellationToken"></param>
        Task<bool> IsMessageRecievedAsync(
            Guid messageGuid,
            Guid recieverGuid,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Возвращает true, если указанное сообщение было послано указанному пользователю
        /// </summary>
        /// <param name="messageGuid">Guid сообщения</param>
        /// <param name="recieverGuid">Guid получателя</param>
        /// <param name="cancellationToken"></param>
        Task<bool> DoesMessageHaveReciever(
            Guid messageGuid,
            Guid recieverGuid,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Возвращает true, если военнослужащий с указанным Guid существует
        /// </summary>
        /// <param name="personGuid">Guid военнослужащего</param>
        /// <param name="cancellationToken"></param>
        Task<bool> DoesPersonWithGuidExistAsync(
            Guid personGuid,
            CancellationToken cancellationToken = default);
    }
}