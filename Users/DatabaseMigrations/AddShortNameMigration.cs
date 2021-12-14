using FluentMigrator;

#pragma warning disable 1591

namespace ChatBackend.Users.DatabaseMigrations
{
    [Migration(202108171200)]
    public sealed class AddShortNameMigration: Migration
    {
        public override void Up()
        {
            Create.Column("short_name")
                .OnTable("chat_user")
                .AsString()
                .Nullable()
                ;

            Execute.Sql("UPDATE chat_user SET short_name = substr(md5(random()::text), 0, 10);");
            
            Alter.Column("short_name").OnTable("chat_user")
                .AsString()
                .NotNullable()
                .Unique()
                ;
        }

        public override void Down()
        {
            Delete.Column("short_name").FromTable("chat_user");
        }
    }
}