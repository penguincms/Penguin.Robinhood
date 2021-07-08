namespace Penguin.Robinhood.Interfaces
{
    public interface IAuthenticationSettings
    {
        string ClientId { get; set; }
        string DeviceToken { get; set; }
        string Password { get; set; }
        string Username { get; set; }
    }
}