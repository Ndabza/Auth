namespace Auth.Application.Common.Interfaces;

public interface IDbInitializer
{
    Task InitializeAsync();
}