namespace Auth.Infrastructure.Repository;

public sealed class UserProfileRepository(IDataAccess dataAccess) : IUserProfileRepository
{
    public async Task CreateProfileAsync(UserProfile userProfile, CancellationToken cancellationToken)
    {
        const string sql = """
                           insert into user_profile (user_id, first_name, last_name, bio, avatar_url) 
                           values (@user_id, @first_name, @last_name, @bio, @avatar_url);
                           """;
        var parameter = new
        {
            user_id = userProfile.UserId,
            first_name = userProfile.FirstName,
            last_name = userProfile.LastName,
            bio = userProfile.Bio,
            avatar_url = userProfile.AvatarUrl
        };

        await dataAccess.Connection.ExecuteAsync(new CommandDefinition(sql, parameter, transaction: dataAccess.Transaction, cancellationToken: cancellationToken));
    }

    public async Task<UserProfile> GetProfileByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        const string sql = "select * from user_profile where user_id = @user_id;";
        return await dataAccess.Connection.QuerySingleAsync<UserProfile>(new CommandDefinition(sql, new { user_id = userId }, transaction:dataAccess.Transaction, cancellationToken:cancellationToken));
    }
}