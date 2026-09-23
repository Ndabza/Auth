namespace Auth.Infrastructure.Persistence;

public sealed class DataAccess(IConfiguration configuration) : IDataAccess, IDisposable
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
                                                ?? throw new ArgumentNullException(nameof(configuration));

    private IDbConnection? _connection;

    public IDbConnection Connection
    {
        get
        {
            if (_connection is { State: ConnectionState.Open }) return _connection;
            _connection = new NpgsqlConnection(_connectionString);
            _connection.Open();

            return _connection;
        }
    }

    public IDbTransaction? Transaction { get; set; }

    public Task BeginTransactionAsync()
    {
        Transaction = Connection.BeginTransaction();
        return Task.CompletedTask;
    }

    public Task CommitAsync()
    {
        if (Transaction == null)
            throw new InvalidOperationException("No transaction to commit");

        Transaction.Commit();

        return Task.CompletedTask;
    }

    public Task RollbackAsync()
    {
        Transaction?.Rollback();

        ResetTransaction();

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Transaction?.Dispose();
        _connection?.Dispose();
    }

    private void ResetTransaction()
    {
        Transaction?.Dispose();
        Transaction = null;
    }
}