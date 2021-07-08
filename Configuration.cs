using Penguin.Robinhood.Interfaces;

namespace Penguin.Robinhood
{
    public class Configuration : IAuthenticationSettings
    {
        public string ClientId { get; set; }
        public bool Configured { get; set; }
        public string DeviceToken { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
    }
}