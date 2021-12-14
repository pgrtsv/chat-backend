using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChatBackend.Persons.Domain;
using Dapper;
using Npgsql;

namespace ChatBackend.Persons.DataAccess
{
    /// <inheritdoc />
    public sealed class PersonsRepository : IPersonsRepository
    {
        private readonly NpgsqlConnection _connection;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        public PersonsRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Person>> GetAsync(CancellationToken cancellationToken) => (await _connection
                .QueryAsync(new CommandDefinition(@"
SELECT guid, first_name, last_name, parent_name, rank_id FROM person",
                    cancellationToken: cancellationToken)))
            .Select(x => new Person(x.guid, new FullName(x.first_name, x.last_name, x.parent_name),
                MilitaryRank.GetById(x.rank_id)));


        /// <inheritdoc />
        public async Task<Person> GetByGuidAsync(Guid guid, CancellationToken cancellationToken)
        {
            var x = await _connection
                .QueryFirstAsync(new CommandDefinition(@"
SELECT guid, first_name, last_name, parent_name, rank_id FROM person",
                    cancellationToken: cancellationToken));
            return new Person(x.guid, new FullName(x.first_name, x.last_name, x.parent_name),
                MilitaryRank.GetById(x.rank_id));
        }

        /// <inheritdoc />
        public Task<bool> DoesGuidExistAsync(Guid guid, CancellationToken cancellationToken) => _connection
            .QueryFirstAsync<bool>(new CommandDefinition(@"
SELECT COUNT(guid) > 0 FROM person WHERE guid = @guid",
                new
                {
                    guid,
                },
                cancellationToken: cancellationToken));

        /// <inheritdoc />
        public Task InsertAsync(Person person, CancellationToken cancellationToken) => _connection
            .ExecuteAsync(new CommandDefinition(@"
INSERT INTO person (guid, first_name, last_name, parent_name, rank_id) VALUES (@Guid, @FirstName, @LastName, @ParentName, @Id)",
                new
                {
                    person.Guid,
                    person.FullName.FirstName,
                    person.FullName.LastName,
                    person.FullName.ParentName,
                    person.Rank.Id,
                }));
    }
}