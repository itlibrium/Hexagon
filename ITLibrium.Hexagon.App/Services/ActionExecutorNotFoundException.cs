using System;

namespace ITLibrium.Hexagon.App.Services
{
    public class ActionExecutorNotFoundException : Exception
    {
        public ActionExecutorNotFoundException(string message) : base(message) { }
    }
}