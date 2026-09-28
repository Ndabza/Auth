using MediatR;

namespace Auth.Application.Features.Auth.SignUp;

public sealed class SignUpCommandHandler(
    IUnitOfWork unitOfWork,
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IUserProfileRepository userProfileRepository,
    TimeProvider timeProvider) : IRequestHandler<SignUpCommand>
{
    public async Task Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        var signUpDto = request.SignUpDto;

        var userExists = await userRepository.GetUserByEmailAsync(signUpDto.Email, cancellationToken);

        if (userExists != null)
            throw new ConflictException($"User with email: '{signUpDto.Email}' already exists.'");

        var hash = passwordHasher.HashPassword(signUpDto.Password);
        var user = AuthUser.Create(signUpDto.Email, hash, timeProvider);

        var userId = await userRepository.CreateUserAsync(user, cancellationToken);
        var userProfile = UserProfile.Create(userId, signUpDto.FirstName, signUpDto.LastName, signUpDto.Bio, signUpDto.AvatarUrl, timeProvider);
        
        await userProfileRepository.CreateProfileAsync(userProfile, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}