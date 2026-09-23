namespace Auth.Infrastructure.Repository;

public sealed class UserRepository(IDataAccess dataAccess) : IUserRepository
{
    public async Task<Guid> CreateUserAsync(string email, string passwordHash, IDbTransaction? transaction = null)
    {
        const string sql = "insert into auth_user (email, password_hash) values (@email, @password_hash) returning id;";
        return await dataAccess.Connection.ExecuteScalarAsync<Guid>(sql, new
        {
            email,
            password_hash = passwordHash
        }, transaction);
    }

    public async Task<AuthUser?> GetUserByEmailAsync(string email, IDbTransaction? transaction = null)
    {
        const string sql = "select * from auth_user where email = @email;";
        return await dataAccess.Connection.QuerySingleOrDefaultAsync<AuthUser>(sql, new { email }, transaction);
    }

    public async Task<AuthUser?> GetUserByIdAsync(Guid userId, IDbTransaction? transaction = null)
    {
        const string sql = "select * from auth_user where id=@id;";
        return await dataAccess.Connection.QuerySingleOrDefaultAsync<AuthUser>(sql, new { id = userId }, transaction);
    }
}