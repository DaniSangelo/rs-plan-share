using Dapper;
using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using PlanShare.Domain.Enums;

namespace PlanShare.Infrastructure.Migrations;
public static class DataBaseMigration
{
    public static void Migrate(DatabaseType databaseType, string connectionString, IServiceProvider serviceProvider)
    {
        if (databaseType == DatabaseType.SqlServer)
            EnsureDatabaseCreatedForSQLServer(connectionString);
        else
            EnsureDatabaseCreatedForMySql(connectionString);

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
        if (!records.Any()) dbConnection.Execute($"CREATE DATABASE [{dbName}]");
    }

    private static void EnsureDatabaseCreatedForMySql(string connectionString)
    {
        var connStringBuilder = new MySqlConnectionStringBuilder(connectionString);
        var dbName = connStringBuilder.Database;
        connStringBuilder.Remove("Database");
        using var dbConnection = new MySqlConnection(connStringBuilder.ConnectionString);
        dbConnection.Execute($"CREATE DATABASE IF NOT EXISTS {dbName}");
    }

    private static void MigrateDatabase(IServiceProvider serviceProvider)
    {
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
        runner.ListMigrations();
        runner.MigrateUp();
    }
}
