using FluentMigrator;
#pragma warning disable 1591

namespace ChatBackend.Chat.DatabaseMigrations
{
    [Migration(2)]
    public sealed class AddChatsMigration : Migration
    {
        public override void Up()
        {
            Create.Table("message")
                .WithColumn("guid").AsGuid().PrimaryKey()
                .WithColumn("content").AsString().NotNullable()
                .WithColumn("sender_guid").AsGuid().NotNullable().ForeignKey("chat_user", "guid")
                .WithColumn("send_datetime").AsDateTimeOffset().NotNullable()
                ;

            Create.Table("reciever_info")
                .WithColumn("message_guid").AsGuid().NotNullable().ForeignKey("message", "guid")
                .WithColumn("reciever_guid").AsGuid().NotNullable().ForeignKey("chat_user", "guid")
                .WithColumn("recieved_datetime").AsDateTimeOffset().NotNullable()
                .WithColumn("read_datetime").AsDateTimeOffset().NotNullable()
                ;
            Create.PrimaryKey().OnTable("reciever_info").Columns("message_guid", "reciever_guid");
        }

        public override void Down()
        {
            Delete.Table("reciever_info");
            Delete.Table("message");
        }
    }
}