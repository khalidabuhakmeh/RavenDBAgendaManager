using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.Exceptions;
using Raven.Client.Exceptions.Database;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;

namespace AgendaManager.Infrastructure;

public static class RavenDbHelper
{
    // https://ravendb.net/docs/article-page/6.0/csharp/client-api/operations/server-wide/create-database#example---ensuredatabaseexists
    public static async Task EnsureDatabaseExistsAsync(
        IDocumentStore store,
        string? database = null,
        bool createDatabaseIfNotExists = true)
    {
        database ??= store.Database;

        if (string.IsNullOrWhiteSpace(database))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(database));

        try
        {
            await store.Maintenance.ForDatabase(database).SendAsync(new GetStatisticsOperation());
        }
        catch (DatabaseDoesNotExistException)
        {
            if (createDatabaseIfNotExists == false)
                throw;

            try
            {
                await store.Maintenance.Server.SendAsync(new CreateDatabaseOperation(new DatabaseRecord(database)));
            }
            catch (ConcurrencyException)
            {
            }
        }
    }

    // https://ravendb.net/docs/article-page/6.0/csharp/client-api/operations/server-wide/get-database-names#example
    public static bool DatabaseExists(
        this IDocumentStore store,
        string databaseName)
    {
        var operation = new GetDatabaseNamesOperation(0, 25);
        string[] databaseNames = store.Maintenance.Server.Send(operation);
        return databaseNames.Contains(databaseName);
    }
}