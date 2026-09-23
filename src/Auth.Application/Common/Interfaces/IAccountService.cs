namespace Auth.Application.Common.Interfaces;

public interface IAccountService
{
    Task SignUpAsync(SignUpDto signUpDto);
    Task<TokenResponse> SignInAsync(SignInRequest signInRequest);
    Task<TokenResponse> RefreshToken(string refreshToken);
    Task SignOutAsync();
}