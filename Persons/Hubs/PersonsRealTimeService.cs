using System.Threading;
using System.Threading.Tasks;
using ChatBackend.Persons.Domain;
using Microsoft.AspNetCore.SignalR;

namespace ChatBackend.Persons.Hubs
{
    /// <inheritdoc />
    public sealed class PersonsRealTimeService : IPersonsRealTimeService
    {
        private readonly IHubContext<PersonsHub, IPersonsHubClient> _hubContext;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hubContext"></param>
        public PersonsRealTimeService(IHubContext<PersonsHub, IPersonsHubClient> hubContext) => _hubContext = hubContext;

        /// <inheritdoc />
        public Task SendPersonIsCreatedAsync(Person person, CancellationToken cancellationToken) =>
            _hubContext.Clients.All.NewPersonCreated(person);

    }
}