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
}