using System.Threading.Tasks;
using ChatBackend.Persons.Domain;

namespace ChatBackend.Persons.Hubs
{
    /// <summary>
    /// Клиент хаба военнослужащих
    /// </summary>
    public interface IPersonsHubClient
    {
        /// <summary>
        /// Вызывается, когда добавляется новая запись о военнослужащем
        /// </summary>
        /// <param name="newPerson"></param>
        Task NewPersonCreated(Person newPerson);
    }
}