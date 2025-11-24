namespace Evently.Modules.Users.PublicApi;

public sealed record UserResponse(Guid Id, string Email, string FullName, string LastName);
