namespace Auth.Infrastructure.Repository;

public sealed class UserProfileRepository(IDataAccess dataAccess) : IUserProfileRepository
{
    public async Task CreateProfileAsync(Guid userId, string firstName, string lastName, string bio, string avatarUrl,
        IDbTransaction? transaction = null)
    {
        const string sql = @"insert into user_profile (user_id, first_name, last_name, bio, avatar_url) 
                                values (@user_id, @first_name, @last_name, @bio, @avatar_url);";
        var parameter = new
        {
            user_id = userId,
            first_name = firstName,
            last_name = lastName,
            bio,
            avatar_url = avatarUrl
        };

        await dataAccess.Connection.ExecuteAsync(sql, parameter, transaction);
    }

    public async Task<UserProfile> GetProfileByUserIdAsync(Guid userId, IDbTransaction? transaction = null)
    {
        const string sql = "select * from user_profile where user_id = @user_id;";
        return await dataAccess.Connection.QuerySingleAsync<UserProfile>(sql, new { user_id = userId }, transaction);
    }
}