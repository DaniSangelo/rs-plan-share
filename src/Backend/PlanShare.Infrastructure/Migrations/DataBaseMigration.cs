using Dapper;
using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;

namespace PlanShare.Infrastructure.Migrations;
public static class DataBaseMigration
{
    public static void Migrate(string connectionString, IServiceProvider serviceProvider)
    {
        EnsureDatabaseCreatedForSQLServer(connectionString);
        MigrateDatabase(serviceProvider);
    }

    private static void EnsureDatabaseCreatedForSQLServer(string connectionString)
    {
        var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
        var dbName = connectionStringBuilder.InitialCatalog;
        connectionStringBuilder.Remove("Initial Catalog");
        var dbConnection = new SqlConnection(connectionStringBuilder.ConnectionString);
        var parameters = new DynamicParameters();
        parameters.Add("name", dbName);
        var records = dbConnection.Query("SELECT TOP 1 * FROM sys.databases WHERE name = @name", parameters);
        if (!records.Any())
        {
            dbConnection.Execute($"CREATE DATABASE [{dbName}]");
        }
    }

    private static void MigrateDatabase(IServiceProvider serviceProvider)
    {
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
        runner.ListMigrations();
        runner.MigrateUp();
    }
}
