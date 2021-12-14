using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChatBackend.Persons.Domain
{
    /// <summary>
    /// Репозиторий военнослужащих
    /// </summary>
    public interface IPersonsRepository
    {
        /// <summary>
        /// Возвращает всех военнослужащих
        /// </summary>
        /// <param name="cancellationToken"></param>
        Task<IEnumerable<Person>> GetAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Возвращает военнослужащего с указанным Guid
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="cancellationToken"></param>
        Task<Person> GetByGuidAsync(Guid guid, CancellationToken cancellationToken);

        /// <summary>
        /// Возвращает true, если военнослужащий с указанным Guid существует
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="cancellationToken"></param>
        Task<bool> DoesGuidExistAsync(Guid guid, CancellationToken cancellationToken);

        /// <summary>
        /// Добавляет военнослужащего в БД
        /// </summary>
        /// <param name="person"></param>
        /// <param name="cancellationToken"></param>
        Task InsertAsync(Person person, CancellationToken cancellationToken);
    }
}