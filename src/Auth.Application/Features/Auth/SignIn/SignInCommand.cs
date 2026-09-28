using MediatR;

namespace Auth.Application.Features.Auth.SignIn;

public sealed record SignInCommand(SignInRequest SignInRequest): IRequest<TokenResponse>;