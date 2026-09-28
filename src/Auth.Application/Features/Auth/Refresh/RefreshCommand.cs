using MediatR;

namespace Auth.Application.Features.Auth.Refresh;

public sealed record RefreshCommand(string RefreshToken):IRequest<TokenResponse>;