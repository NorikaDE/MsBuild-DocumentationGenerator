using System;
using System.Collections.Concurrent;

namespace Norika.MsBuild.DocumentationGenerator.Business.Logging
{
    public static class Log
    {
        private static readonly ConcurrentBag<IApplicationLogger> RegisteredLoggers =
            new ConcurrentBag<IApplicationLogger>()
            {
                new DefaultConsoleApplicationLoggerImplementation()
            };

        public static void WriteLine(string message)
        {
            foreach (IApplicationLogger logger in RegisteredLoggers)
            {
                logger.WriteLine(message);
            }
        }

        public static void WriteLine(string message, params object[] args)
        {
            foreach (IApplicationLogger logger in RegisteredLoggers)
            {
                logger.WriteLine(message, args);
            }
        }

        public static void WriteError(Exception ex)
        {
            foreach (IApplicationLogger logger in RegisteredLoggers)
            {
                logger.WriteError(ex);
            }
        }

        public static void RegisterLogger(IApplicationLogger logger)
        {
            RegisteredLoggers.Add(logger);
        }
    }

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

    public interface IApplicationLogger
    {
        void WriteLine(string message, params object[] args);

        void WriteLine(string message);

        void WriteError(Exception ex);
    }
}