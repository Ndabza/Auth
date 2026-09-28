using MediatR;

namespace Auth.Application.Features.Auth.SignUp;

public sealed record SignUpCommand(SignUpDto SignUpDto) : IRequest;