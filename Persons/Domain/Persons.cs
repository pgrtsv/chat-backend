using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ChatBackend.Persons.Domain.Operations;

namespace ChatBackend.Persons.Domain
{
    /// <summary>
    /// Военнослужащие, пользующиеся чатом
    /// </summary>
    public sealed class Persons
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly IPersonsRealTimeService _realTimeService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="personsRepository"></param>
        /// <param name="realTimeService"></param>
        public Persons(IPersonsRepository personsRepository, IPersonsRealTimeService realTimeService)
        {
            _personsRepository = personsRepository;
            _realTimeService = realTimeService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        public Task<IEnumerable<Person>> GetAsync(CancellationToken cancellationToken) => 
            _personsRepository.GetAsync(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="cancellationToken"></param>
        public Task<Person> GetByGuidAsync(Guid guid, CancellationToken cancellationToken) =>
            _personsRepository.GetByGuidAsync(guid, cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="cancellationToken"></param>
        public Task<bool> DoesGuidExistAsync(Guid guid, CancellationToken cancellationToken) =>
            _personsRepository.DoesGuidExistAsync(guid, cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createPerson"></param>
        /// <param name="cancellationToken"></param>
        public async Task<Person> CreateAsync(CreatePerson createPerson, CancellationToken cancellationToken)
        {
            var person = Person.New(createPerson);
            await _personsRepository.InsertAsync(person, cancellationToken);
            await _realTimeService.SendPersonIsCreatedAsync(person, cancellationToken);
            return person;
        }
    }
}