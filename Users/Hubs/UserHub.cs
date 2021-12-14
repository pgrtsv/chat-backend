using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ChatBackend.Users.Hubs
{
    /// <summary>
    /// Хаб пользователей чата
    /// </summary>
    public sealed class UserHub : Hub<IUserHubClient>
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
        public override Task OnConnectedAsync()
        {
            if (Context.UserIdentifier == null) return base.OnConnectedAsync();
            AuthenticatedConnections.Add(Context.ConnectionId, Guid.Parse(Context.UserIdentifier));
            return base.OnConnectedAsync();
        }

        /// <inheritdoc />
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if (Context.UserIdentifier == null) return base.OnConnectedAsync();
            AuthenticatedConnections.Remove(Context.ConnectionId);
            return base.OnConnectedAsync();
        }
    }
}