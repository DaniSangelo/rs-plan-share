using FluentMigrator;

namespace PlanShare.Infrastructure.Migrations.Versions;

[Migration(DatabaseVersions.TABLE_USER_REGISTER, "Create table to save user data")]
public class Version0000001 : ForwardOnlyMigration
{
    public override void Up()
    {
        Create.Table("Users")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("Name").AsString(30).NotNullable()
            .WithColumn("Email").AsString(100).NotNullable().Unique()
            .WithColumn("Password").AsString(2048).NotNullable()
            .WithColumn("Active").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);
    }

    /*
        if I need to implement down logic, then I should use Migration instead of ForwardOnlyMigration
    */
}
