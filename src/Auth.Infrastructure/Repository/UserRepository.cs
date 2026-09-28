namespace Auth.Infrastructure.Repository;

public sealed class UserRepository(IDataAccess dataAccess) : IUserRepository
{
    public async Task<Guid> CreateUserAsync(AuthUser authUser, CancellationToken cancellationToken)
    {
        const string sql = "insert into auth_user (email, password_hash) values (@email, @password_hash) returning id;";
        return await dataAccess.Connection.ExecuteScalarAsync<Guid>(
            new CommandDefinition(sql, new
            {
                email =  authUser.Email,
                password_hash = authUser.PasswordHash
            }, transaction: dataAccess.Transaction, cancellationToken: cancellationToken));
    }

    public async Task<AuthUser?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        const string sql = "select * from auth_user where email = @email;";
        return await dataAccess.Connection.QuerySingleOrDefaultAsync<AuthUser>(new CommandDefinition(sql, new { email },
            transaction: dataAccess.Transaction, cancellationToken: cancellationToken));
    }

    public async Task<AuthUser?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        const string sql = "select * from auth_user where id=@id;";
        return await dataAccess.Connection.QuerySingleOrDefaultAsync<AuthUser>(new CommandDefinition(sql,
            new { id = userId }, transaction: dataAccess.Transaction, cancellationToken: cancellationToken));
    }
}