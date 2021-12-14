using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChatBackend.Users.DataTransfer;
using ChatBackend.Users.Domain;
using ChatBackend.Users.Domain.Operations;
using ChatBackend.Users.Domain.Validators;
using ChatBackend.Users.Hubs;
using Dapper;
using Microsoft.AspNetCore.SignalR;
using Npgsql;

namespace ChatBackend.Users.Services
{
    /// <summary>
    /// Сервис пользователей чата
    /// </summary>
    public sealed class UserService : 
        IUserService, 
        IUserValidatorsService
    {
        private readonly NpgsqlConnection _dataConnection;
        private readonly IHubContext<UserHub, IUserHubClient> _userHub;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataConnection"></param>
        /// <param name="userHub"></param>
        public UserService(NpgsqlConnection dataConnection, IHubContext<UserHub, IUserHubClient> userHub)
        {
            _dataConnection = dataConnection;
            _userHub = userHub;
        }

        /// <inheritdoc />
        public async Task<UserDto> CreateAsync(CreateUser createUser, CancellationToken cancellationToken)
        {
            var user = await User.NewAsync(createUser, this);
            await _dataConnection.ExecuteAsync(new CommandDefinition(@"
INSERT INTO chat_user (guid, name, short_name, login, password_hash) VALUES(@Guid, @Name, @ShortName, @Login, @PasswordHash);
INSERT INTO user_role (user_guid, role_id)  VALUES 
" + user.UserRoles.Aggregate(string.Empty, (text, role) => text + $"(@Guid, {role.Id}),")[..^1], 
                user, 
                cancellationToken: cancellationToken));
            var userDto = new UserDto
            {
                Guid = user.Guid,
                Login = user.Login,
                Name =  user.Name, 
                ShortName = user.ShortName,
                Roles = user.UserRoles,
            };
            await _userHub.Clients.All.RecieveNewUser(userDto);
            return userDto;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<UserDto>> GetAsync(CancellationToken cancellationToken)
        {
            using var multiQuery = await _dataConnection.QueryMultipleAsync(new CommandDefinition(@"
SELECT guid, login, short_name AS ShortName, name FROM chat_user;
SELECT user_guid, role_id FROM user_role;"));
            var users = multiQuery.Read<UserDto>().ToList();
            var roles = multiQuery.Read<(Guid, int)>().ToList();
            foreach (var group in roles.GroupBy(x => x.Item1, x => x.Item2))
                users.First(x => x.Guid == group.Key).Roles = group.Select(UserRole.GetById).ToArray();
            return users;
        }

        /// <inheritdoc />
        public async Task<UserDto> GetByGuidAsync(Guid guid, CancellationToken cancellationToken)
        {
            using var multiQuery = await _dataConnection.QueryMultipleAsync(new CommandDefinition(@"
SELECT guid, login, short_name AS ShortName, name FROM chat_user WHERE guid = @guid;
SELECT user_guid, role_id FROM user_role WHERE user_guid = @guid;",
            new { guid },
            cancellationToken: cancellationToken));
            var user = multiQuery.Read<UserDto>().Single();
            var roles = multiQuery.Read<(Guid, int)>().ToList();
            user.Roles = roles.Select(role => UserRole.GetById(role.Item2)).ToArray();
            return user;
        }

        /// <inheritdoc />
        public Task<bool> CheckIfGuidExistsAsync(Guid guid, CancellationToken cancellationToken) => 
            _dataConnection.QueryFirstAsync<bool>(new CommandDefinition(
                @"SELECT COUNT(guid) > 0 FROM chat_user WHERE guid = @guid",
                new { guid },
                cancellationToken: cancellationToken
                ));

        /// <inheritdoc />
        public async Task<UserDto?> LogInAsync(string login, string password, CancellationToken cancellationToken)
        {
            using var multiQuery = await _dataConnection.QueryMultipleAsync(new CommandDefinition(@"
SELECT guid, name, short_name AS ShortName, login, password_hash
FROM chat_user
WHERE login = @login AND password_hash = @passwordHash;
SELECT role_id FROM user_role, chat_user WHERE guid = user_guid AND login = @login ",
                new {login, passwordHash = User.GetPasswordHash(password)},
                cancellationToken: cancellationToken));
            var chatUser = multiQuery.Read<UserDto>().SingleOrDefault();
            if (chatUser == null) return null;
            var roles = multiQuery.Read<int>().ToList().Select(UserRole.GetById).ToList();
            chatUser.Roles = roles;
            return chatUser;
        }

        /// <inheritdoc />
        public Task<bool> CheckIfLoginExistsAsync(string login, CancellationToken cancellationToken) =>
            _dataConnection.QueryFirstAsync<bool>(new CommandDefinition(
                @"SELECT COUNT(login) > 0 FROM chat_user WHERE login = @login",
                new {login},
                cancellationToken: cancellationToken));

        /// <inheritdoc />
        public Task<bool> CheckIfUsernameExistsAsync(string username, CancellationToken cancellationToken) => 
            _dataConnection.QueryFirstAsync<bool>(new CommandDefinition(
                @"SELECT COUNT(name) > 0 FROM chat_user WHERE name = @username",
                new {username},
                cancellationToken: cancellationToken
            ));
        
        /// <inheritdoc />
        public Task<bool> CheckIfShortNameExistsAsync(string shortName, CancellationToken cancellationToken) => 
            _dataConnection.QueryFirstAsync<bool>(new CommandDefinition(
                @"SELECT COUNT(short_name) > 0 FROM chat_user WHERE short_name = @shortName",
                new {shortName},
                cancellationToken: cancellationToken
            ));
    }
}
