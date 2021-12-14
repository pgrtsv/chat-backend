using Microsoft.AspNetCore.SignalR;

namespace ChatBackend.Persons.Hubs
{
    /// <summary>
    /// Хаб военнослужащих, пользующихся чатом
    /// </summary>
    public sealed class PersonsHub : Hub<IPersonsHubClient>
    {
        
    }
}