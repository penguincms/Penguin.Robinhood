using Penguin.Robinhood.Interfaces;
using System;

namespace Penguin.Robinhood.Models
{
    public class SymbolResponse : IHasId
    {
        public Guid Id { get; set; }
        public string Symbol { get; set; }
    }
}