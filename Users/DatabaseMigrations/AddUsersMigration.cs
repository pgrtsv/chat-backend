using System;
using System.Collections.Generic;
using ChatBackend.Users.Domain;
using FluentMigrator;
#pragma warning disable 1591

namespace ChatBackend.Users.DatabaseMigrations
{
    [Migration(1)]
    public sealed class AddUsersMigration : Migration
    {
        public override void Up()
        {
            Create.Table("chat_user")
                .WithColumn("guid").AsGuid().PrimaryKey()
                .WithColumn("name").AsString().NotNullable().Unique()
                .WithColumn("login").AsString().NotNullable().Unique()
                .WithColumn("password_hash").AsString().NotNullable()
            ;

            var adminUser = new User(
                Guid.NewGuid(), "Администратор",  "Администратор", "admin", User.GetPasswordHash("admin"), new[]
                {
                    UserRole.User, UserRole.Administrator,
                });
            Insert.IntoTable("chat_user").Row(new Dictionary<string, object>
            {
                ["guid"] = adminUser.Guid,
                ["name"] = adminUser.Name,
                ["login"] = adminUser.Login,
                ["password_hash"] = adminUser.PasswordHash
            });
            
            Create.Table("user_role")
                .WithColumn("user_guid").AsGuid().NotNullable().ForeignKey("chat_user", "guid")
                .WithColumn("role_id").AsInt32().NotNullable()
                ;
            Create.PrimaryKey().OnTable("user_role").Columns("user_guid", "role_id");

            foreach (var role in adminUser.UserRoles)
                Insert.IntoTable("user_role").Row(new Dictionary<string, object>
                {
                    ["user_guid"] = adminUser.Guid,
                    ["role_id"] = role.Id
                });
        }

        public override void Down()
        {
            Delete.Table("user_roles");
            Delete.Table("user");
        }
    }
}