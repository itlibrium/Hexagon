using System;

namespace ITLibrium.Hexagon.App.Commands
{
    public class CommandHandlerNotFoundException : Exception
    {
        public CommandHandlerNotFoundException(string message) : base(message) { }
    }
}