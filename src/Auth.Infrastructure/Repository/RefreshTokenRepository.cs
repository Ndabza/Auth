namespace Auth.Infrastructure.Repository;

public sealed class RefreshTokenRepository(IDataAccess dataAccess) : IRefreshTokenRepository
{
    public async Task InsertIntoRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        const string sql =
            """
            insert into refresh_token (user_id, token, token_expires) 
            values (@user_id, @token, @token_expires);
            """;

        var parameter = new
        {
            user_id = refreshToken.UserId,
            token = refreshToken.Token,
            token_expires = refreshToken.TokenExpires
        };

        await dataAccess.Connection.ExecuteAsync(new CommandDefinition(sql, parameter,
            transaction: dataAccess.Transaction, cancellationToken: cancellationToken));
    }

    public async Task<RefreshToken?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        const string sql = "select * from refresh_token where token = @token;";
        return await dataAccess.Connection.QuerySingleAsync<RefreshToken>(new CommandDefinition(sql,
            new { token = refreshToken },
            transaction: dataAccess.Transaction, cancellationToken: cancellationToken));
    }

    public async Task RevokeAllActiveTokensAsync(Guid userId, CancellationToken cancellationToken)
    {
        const string sql = """
                           update refresh_token
                           set revoked_at = @revoked
                           where user_id = @user_id and revoked_at is null and token_expires > @now;
                           """;

        var parameter = new
        {
            user_id = userId,
            revoked = DateTime.Now,
            now = DateTime.UtcNow
        };

        await dataAccess.Connection.ExecuteAsync(new CommandDefinition(sql, parameter,
            transaction: dataAccess.Transaction, cancellationToken: cancellationToken));
    }

    public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        const string sql = """
                           update refresh_token set revoked_at = @revoked_at, 
                                                    replaced_by_token = @replaced_by_token 
                           where user_id = @user_id and token = @token and revoked_at is null;
                           """;

        var parameter = new
        {
            user_id = refreshToken.UserId,
            token = refreshToken.Token,
            revoked_at = refreshToken.RevokedAt,
            replaced_by_token = refreshToken.ReplacedByToken
        };

        await dataAccess.Connection.ExecuteAsync(new CommandDefinition(sql, parameter,
            transaction: dataAccess.Transaction, cancellationToken: cancellationToken));
    }

    public async Task<int> DeleteExpiredTokens(CancellationToken cancellationToken)
    {
        const string sql = "delete from refresh_token where token_expires < now();";
        return await dataAccess.Connection.ExecuteAsync(new CommandDefinition(sql, cancellationToken: cancellationToken));
    }
}