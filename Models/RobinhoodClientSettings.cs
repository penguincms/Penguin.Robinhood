using Penguin.Robinhood.Interfaces;

namespace Penguin.Robinhood.Models
{
    public class RobinhoodClientSettings : IAuthenticationSettings
    {
        public string ClientId { get; set; }
        public string DeviceToken { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}