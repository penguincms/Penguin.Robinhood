using Penguin.Json.Abstractions.Interfaces;

namespace Penguin.Robinhood.Models
{
    public class LoggedObject : IJsonPopulatedObject
    {
        string IJsonPopulatedObject.RawJson { get; set; }
    }
}