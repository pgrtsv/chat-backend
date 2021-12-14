using System.Diagnostics.CodeAnalysis;
using ChatBackend.Persons.Domain;
using FluentMigrator;

#pragma warning disable 1591

namespace ChatBackend.Persons.DatabaseMigrations
{
    [SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Миграции используются фреймворком")]
    [Migration(202107271615)]
    public sealed class AddDummyPersonMigration : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("person")
                .Row(new
                {
                    guid = Person.Dummy.Guid,
                    first_name = Person.Dummy.FullName.FirstName,
                    last_name = Person.Dummy.FullName.LastName,
                    parent_name = Person.Dummy.FullName.ParentName,
                    rank_id = Person.Dummy.Rank.Id,
                });
        }

        public override void Down()
        {
            Delete.FromTable("person")
                .Row(new
                {
                    guid = Person.Dummy.Guid,
                });
        }
    }
}