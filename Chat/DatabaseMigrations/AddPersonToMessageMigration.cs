using FluentMigrator;
#pragma warning disable 1591

namespace ChatBackend.Chat.DatabaseMigrations
{
    [Migration(202107121428)]
    public sealed class AddPersonToMessageMigration : Migration
    {
        public override void Up()
        {
            Create.Column("person_guid").OnTable("message").AsGuid().Nullable().ForeignKey("person", "guid");
        }

        public override void Down()
        {
            Delete.ForeignKey().FromTable("message").ForeignColumn("person_guid");
            Delete.Column("person_guid").FromTable("message");
        }
    }
}