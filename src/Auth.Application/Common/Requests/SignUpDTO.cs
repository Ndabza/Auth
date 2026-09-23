namespace Auth.Application.Common.Requests;

public sealed record SignUpDto(
    string Email,
    string FirstName,
    string LastName,
    string Bio,
    Stream AvatarStream,
    string ContentType,
    string Extension,
    string Password);