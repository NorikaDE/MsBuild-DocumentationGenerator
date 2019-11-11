using System;

namespace Norika.MsBuild.DocumentationGenerator.Business.Logging
{
    internal class DefaultConsoleApplicationLoggerImplementation : IApplicationLogger
    {
        public void WriteLine(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }

        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public void WriteError(Exception ex)
        {
            Console.Write(ex);
        }
    }
}