namespace Auth.Application.Common.Requests;

public sealed record SignUpDto(
    string Email,
    string FirstName,
    string LastName,
    string Bio,
    string AvatarUrl,
    string Password);