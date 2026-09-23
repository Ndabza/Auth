namespace Auth.Infrastructure.Repository;

public sealed class RefreshTokenRepository(IDataAccess dataAccess) : IRefreshTokenRepository
{
    public async Task InsertIntoRefreshTokenAsync(Guid userId, string refreshToken, DateTime expires,
        IDbTransaction? transaction = null)
    {
        const string sql =
            "insert into refresh_token (user_id, token, token_expires) values (@user_id, @token, @token_expires);";

        var parameter = new
        {
            user_id = userId,
            token = refreshToken,
            token_expires = expires
        };

        await dataAccess.Connection.ExecuteAsync(sql, parameter, transaction);
    }

    public async Task<RefreshToken?> GetByRefreshTokenAsync(string refreshToken, IDbTransaction? transaction = null)
    {
        const string sql = "select * from refresh_token where token = @token;";
        return await dataAccess.Connection.QuerySingleAsync<RefreshToken>(sql, new { token = refreshToken },
            transaction);
    }

    public async Task RevokeAllActiveTokensAsync(Guid userId, IDbTransaction? transaction = null)
    {
        const string sql = @"update refresh_token
                            set revoked = @revoked
                            where user_id = @user_id
                                and revoke is null
                                and token_expires > @now;";
        var parameter = new
        {
            user_id = userId,
            revoked = DateTime.Now,
            now = DateTime.UtcNow
        };

        await dataAccess.Connection.ExecuteAsync(sql, parameter, transaction);
    }

    public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken, IDbTransaction? transaction = null)
    {
        const string sql =
            "update refresh_token set revoked = @revoked, replaced_by = @replaced_by where user_id = @user_id;";

        var parameter = new
        {
            user_id = refreshToken.UserId,
            revoked = refreshToken.Revoked,
            replaced_by = refreshToken.ReplacedBy
        };

        await dataAccess.Connection.ExecuteAsync(sql, parameter, transaction);
    }
}