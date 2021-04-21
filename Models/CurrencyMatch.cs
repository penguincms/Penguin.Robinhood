using System;

namespace Penguin.Robinhood.Models
{
    [Flags]
    public enum CurrencyMatch
    {
        Code = 1,
        Name = 2,
        CodeOrName = 3
    }
}