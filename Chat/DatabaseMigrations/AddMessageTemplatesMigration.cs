using FluentMigrator;
#pragma warning disable 1591

namespace ChatBackend.Chat.DatabaseMigrations
{
    [Migration(202107091630)]
    public sealed class AddMessageTemplatesMigration : Migration
    {
        public override void Up()
        {
            Create.Table("default_message_template")
                .WithColumn("id").AsInt32().PrimaryKey()
                .WithColumn("role_id").AsInt32().NotNullable()
                .WithColumn("content").AsString().NotNullable()
                ;
            Create.Table("user_message_template")
                .WithColumn("guid").AsGuid().PrimaryKey()
                .WithColumn("user_guid").AsGuid().NotNullable().ForeignKey("chat_user", "guid")
                .WithColumn("modified_datetime").AsDateTimeOffset().NotNullable()
                .WithColumn("content").AsString().NotNullable()
                ;
            Insert.IntoTable("default_message_template")
                .Row(new
                {
                    id = 1,
                    role_id = 3,
                    content = "В {рота}НР обход произведён. В кроватях {налицо} человек, наряд - {наряд} человек.",
                })
                .Row(new
                {
                    id = 2,
                    role_id = 3,
                    content = "В {рота}НР ",
                })
                
                ;
        }

        public override void Down()
        {
            Delete.Table("default_message_template");
            Delete.Table("user_message_template");
        }
    }
}