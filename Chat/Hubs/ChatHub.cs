using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ChatBackend.Chat.Hubs
{
    /// <summary>
    /// Хаб чата
    /// </summary>
    public sealed class ChatHub : Hub<IChatHubClient>
    {
        /// <summary>
        /// Guid пользователей в неанонимных подключениях
        /// </summary>
        private static Dictionary<string, Guid> AuthenticatedConnections { get; } = new();

        /// <summary>
        /// Возвращает Guid всех подключенных пользователей
        /// </summary>
        public static IEnumerable<Guid> GetConnectedUsers() => AuthenticatedConnections.Values.Distinct();
        
        /// <inheritdoc />
        public override async Task OnConnectedAsync()
        {
            if (Context.UserIdentifier == null)
            {
                await base.OnConnectedAsync();
                return;
            }
            var guid = Guid.Parse(Context.UserIdentifier);
            await Clients.Users(GetConnectedUsers().Select(x => x.ToString()))
                .UserHasConnected(guid);
            AuthenticatedConnections.Add(Context.ConnectionId, guid);
            await base.OnConnectedAsync();
        }

        /// <inheritdoc />
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (Context.UserIdentifier == null)
            {
                await base.OnConnectedAsync();
                return;
            }
            await Clients.Users(GetConnectedUsers().Select(x => x.ToString()))
                .UserHasDisconnected(Guid.Parse(Context.UserIdentifier));
            AuthenticatedConnections.Remove(Context.ConnectionId);
            await base.OnConnectedAsync();
        }
    }
}