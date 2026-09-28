namespace Auth.Infrastructure.Persistence;

public sealed class UnitOfWork(IDataAccess dataAccess): IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dataAccess.BeginTransactionAsync();
        
        try
        {
            await dataAccess.CommitAsync();
        }
        catch (Exception)
        {
            await dataAccess.RollbackAsync();
            throw;
        }
    }
}