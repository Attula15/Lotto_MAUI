namespace Lottery.Domain;
public interface IKeyCloakService
{
    public Task<string> Login(string username, string password);
    public Task<bool> Logout();
    public string GetSessionToken();
    public string GetRefreshToken();
}

