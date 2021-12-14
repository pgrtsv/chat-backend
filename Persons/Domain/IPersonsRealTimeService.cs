using System.Threading;
using System.Threading.Tasks;

namespace ChatBackend.Persons.Domain
{
    public interface IPersonsRealTimeService
    {
        Task SendPersonIsCreatedAsync(Person person, CancellationToken cancellationToken);
    }
}