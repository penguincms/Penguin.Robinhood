using Penguin.Robinhood.Interfaces;
using System;

namespace Penguin.Robinhood.Models
{
    public class SymbolResponse : IHasId
    {
        public string Symbol { get; set; }
        public Guid Id { get; set; }
    }
}