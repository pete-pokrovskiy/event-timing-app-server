namespace EventTiming.Logic.Services.Auth
{
    public interface IAuthenticationService
    {
        bool IsAuthenticated { get; }
        string UserLogin { get; }
    }
}
