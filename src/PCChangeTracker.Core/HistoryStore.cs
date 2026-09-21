using System.Globalization;
using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace PCChangeTracker.Core;

public sealed record SnapshotSummary(Guid Id, DateTimeOffset CapturedAt, string? Label, int ItemCount, int CompleteSources)
{
    public CollectionScope Scope { get; init; }
    public bool Elevated { get; init; }
    public string Context => CollectionScopes.Context(Scope, Elevated);
    public string Display => $"{CapturedAt.ToLocalTime():g}  {(string.IsNullOrWhiteSpace(Label) ? "Manual check" : Label)}  ({Context})";
    public string PickerDisplay => $"{CapturedAt.ToLocalTime():HH:mm:ss.fff}  |  {(string.IsNullOrWhiteSpace(Label) ? "Manual check" : Label)}  |  {Context}";
}

public sealed class HistoryStore
{
    private readonly string connectionString;

    public HistoryStore(string databasePath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(databasePath))!);
        connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = databasePath, ForeignKeys = true, Pooling = false, DefaultTimeout = 5
        }.ToString();
        using var connection = Open();
        using var version = connection.CreateCommand();
        version.CommandText = "PRAGMA user_version;";
        if (Convert.ToInt32(version.ExecuteScalar(), CultureInfo.InvariantCulture) > 2)
            throw new InvalidOperationException("This history was created by a newer app. It has not been changed.");
        using var schema = connection.CreateCommand();
        schema.CommandText = """
            PRAGMA journal_mode=WAL;
            CREATE TABLE IF NOT EXISTS snapshots (
                id TEXT PRIMARY KEY, captured_at TEXT NOT NULL, label TEXT,
                item_count INTEGER NOT NULL, complete_sources INTEGER NOT NULL, payload TEXT NOT NULL
            );
            CREATE INDEX IF NOT EXISTS snapshots_date ON snapshots(captured_at);
            CREATE TABLE IF NOT EXISTS preferences (key TEXT PRIMARY KEY, value TEXT NOT NULL);
            CREATE TABLE IF NOT EXISTS expected_changes (id TEXT PRIMARY KEY, marked_at TEXT NOT NULL);
            PRAGMA user_version=2;
            """;
        schema.ExecuteNonQuery();
    }

    public void Save(Snapshot snapshot)
    {
        if (!snapshot.Results.Any(result => result.Status is CollectionStatus.Success or CollectionStatus.Partial))
            throw new InvalidOperationException("No source could be collected. History was not changed.");
        using var connection = Open();
        using var transaction = connection.BeginTransaction();
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "INSERT INTO snapshots VALUES ($id, $date, NULL, $count, $sources, $payload);";
        command.Parameters.AddWithValue("$id", snapshot.Id.ToString());
        command.Parameters.AddWithValue("$date", snapshot.FinishedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture));
        command.Parameters.AddWithValue("$count", snapshot.Results.Sum(result => result.Items.Count));
        command.Parameters.AddWithValue("$sources", snapshot.Results.Count(result => result.Status == CollectionStatus.Success));
        command.Parameters.AddWithValue("$payload", JsonSerializer.Serialize(snapshot));
        command.ExecuteNonQuery();
        using var baseline = connection.CreateCommand();
        baseline.Transaction = transaction;
        baseline.CommandText = "INSERT OR IGNORE INTO preferences (key, value) VALUES ($key, $id);";
        baseline.Parameters.AddWithValue("$key", BaselineKey(snapshot.Scope, snapshot.Elevated));
        baseline.Parameters.AddWithValue("$id", snapshot.Id.ToString());
        baseline.ExecuteNonQuery();
        transaction.Commit();
    }

    public IReadOnlyList<SnapshotSummary> List()
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, captured_at, label, item_count, complete_sources,
                COALESCE(json_extract(payload, '$.Scope'), 0), COALESCE(json_extract(payload, '$.Elevated'), 0)
            FROM snapshots ORDER BY captured_at DESC;
            """;
        using var reader = command.ExecuteReader();
        var result = new List<SnapshotSummary>();
        while (reader.Read())
            result.Add(new(Guid.Parse(reader.GetString(0)), DateTimeOffset.Parse(reader.GetString(1), CultureInfo.InvariantCulture),
                reader.IsDBNull(2) ? null : reader.GetString(2), reader.GetInt32(3), reader.GetInt32(4))
                { Scope = (CollectionScope)reader.GetInt32(5), Elevated = reader.GetBoolean(6) });
        return result;
    }

    public Snapshot? Load(Guid id)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT payload FROM snapshots WHERE id=$id;";
        command.Parameters.AddWithValue("$id", id.ToString());
        var payload = command.ExecuteScalar() as string;
        return payload is null ? null : JsonSerializer.Deserialize<Snapshot>(payload);
    }

    public Guid? BaselineId => GetBaselineId(CollectionScope.Legacy);

    public Guid? GetBaselineId(CollectionScope scope, bool elevated = false) =>
        Guid.TryParse(GetPreference(BaselineKey(scope, elevated)), out var value) ? value : null;

    private static string BaselineKey(CollectionScope scope, bool elevated) => scope == CollectionScope.Legacy
        ? "baseline" : $"baseline.{scope}.{(elevated ? "administrator" : "standard")}";

    public void SetBaseline(Guid id)
    {
        var snapshot = Load(id) ?? throw new ArgumentException("The selected snapshot no longer exists.");
        SetPreference(BaselineKey(snapshot.Scope, snapshot.Elevated), id.ToString());
    }

    public void Rename(Guid id, string label)
    {
        label = label.Trim();
        if (label.Length is < 1 or > 120) throw new ArgumentException("Use a checkpoint name between 1 and 120 characters.");
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE snapshots SET label=$label WHERE id=$id;";
        command.Parameters.AddWithValue("$label", label);
        command.Parameters.AddWithValue("$id", id.ToString());
        if (command.ExecuteNonQuery() != 1) throw new ArgumentException("The selected snapshot no longer exists.");
    }

    public void Delete(Guid id)
    {
        var snapshot = Load(id);
        if (snapshot is not null && id == GetBaselineId(snapshot.Scope, snapshot.Elevated))
            throw new InvalidOperationException("Choose a different baseline for this scope and access before deleting this snapshot.");
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM snapshots WHERE id=$id;";
        command.Parameters.AddWithValue("$id", id.ToString());
        command.ExecuteNonQuery();
    }

    public void Clear()
    {
        using var connection = Open();
        using var transaction = connection.BeginTransaction();
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "DELETE FROM snapshots; DELETE FROM expected_changes; DELETE FROM preferences WHERE key='baseline' OR key GLOB 'baseline.*';";
        command.ExecuteNonQuery();
        transaction.Commit();
    }

    public string? GetPreference(string key)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT value FROM preferences WHERE key=$key;";
        command.Parameters.AddWithValue("$key", key);
        return command.ExecuteScalar() as string;
    }

    public void SetPreference(string key, string value)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO preferences (key,value) VALUES ($key,$value) ON CONFLICT(key) DO UPDATE SET value=$value;";
        command.Parameters.AddWithValue("$key", key);
        command.Parameters.AddWithValue("$value", value);
        command.ExecuteNonQuery();
    }

    public HashSet<string> ExpectedChanges()
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT id FROM expected_changes;";
        using var reader = command.ExecuteReader();
        var result = new HashSet<string>(StringComparer.Ordinal);
        while (reader.Read()) result.Add(reader.GetString(0));
        return result;
    }

    public void SetExpected(string id, bool expected)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = expected ? "INSERT OR IGNORE INTO expected_changes VALUES ($id,$date);" : "DELETE FROM expected_changes WHERE id=$id;";
        command.Parameters.AddWithValue("$id", id);
        command.Parameters.AddWithValue("$date", DateTimeOffset.UtcNow.ToString("O", CultureInfo.InvariantCulture));
        command.ExecuteNonQuery();
    }

    private SqliteConnection Open()
    {
        var connection = new SqliteConnection(connectionString);
        try { connection.Open(); return connection; }
        catch { connection.Dispose(); throw; }
    }
}