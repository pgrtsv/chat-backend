using FluentMigrator;

#pragma warning disable 1591

namespace ChatBackend.Persons.DatabaseMigrations
{
    [Migration(202107121400)]
    public sealed class AddPersonsMigration : Migration
    {
        public override void Up()
        {
            Create.Table("person")
                .WithColumn("guid").AsGuid().PrimaryKey()
                .WithColumn("first_name").AsString().NotNullable()
                .WithColumn("last_name").AsString().NotNullable()
                .WithColumn("parent_name").AsString().NotNullable()
                .WithColumn("rank_id").AsInt32().NotNullable()
                ;

        }

        public override void Down()
        {
            Delete.Table("person");
        }
    }
}