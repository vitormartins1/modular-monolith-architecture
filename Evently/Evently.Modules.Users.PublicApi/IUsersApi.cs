namespace Evently.Modules.Users.PublicApi;

public interface IUsersApi
{
    Task<UserResponse?> GetAsync(Guid Id, CancellationToken cancellationToken = default);

}
