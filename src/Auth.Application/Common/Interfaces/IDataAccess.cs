namespace Auth.Application.Common.Interfaces;

public interface IDataAccess
{
    IDbConnection Connection { get; }
    IDbTransaction? Transaction { get; set; }
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
    void Dispose();
}