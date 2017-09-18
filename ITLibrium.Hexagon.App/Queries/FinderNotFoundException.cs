using System;

namespace ITLibrium.Hexagon.App.Queries
{
    public class FinderNotFoundException : Exception
    {
        public FinderNotFoundException(string message) : base(message) { }
    }
}