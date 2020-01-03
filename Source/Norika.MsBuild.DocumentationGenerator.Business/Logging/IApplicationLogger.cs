using System;
using System.Runtime.CompilerServices;

namespace Norika.MsBuild.DocumentationGenerator.Business.Logging
{
    public interface IApplicationLogger
    {
        void WriteLine(string message, params object[] args);

        void WriteDebug(string message, params object[] args);

        void WriteLine(string message);

        void WriteError(Exception ex);
    }
}