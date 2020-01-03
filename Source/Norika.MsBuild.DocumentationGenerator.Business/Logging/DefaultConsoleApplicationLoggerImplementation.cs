using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Norika.MsBuild.DocumentationGenerator.Business.Logging
{
    public class DefaultConsoleApplicationLoggerImplementation : IApplicationLogger
    {
        private const string DateTimeFormat = "dd-MM-yyyy HH:mm:ss";
        private const string DefaultVerboseLogLinePattern = "{0} {1} > {2}: {3}";
        private const char DefaultDepthIdentifier = '.';
        
        public void WriteLine(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }

        public void WriteDebug(string message,  params object[] args)
        {
            string formatMessage = string.Format(message, args);
            string dateTimeString = BuildDateTimeString();
            string depthIdentifier = BuildCallerDepthIdentifier(GetCallerDepth(2));

            MethodBase caller = GetCallerName(2);
            
            Console.WriteLine(DefaultVerboseLogLinePattern, dateTimeString, depthIdentifier, caller.Name, formatMessage);
        }

        private string BuildCallerDepthIdentifier(int callerDepth)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i <= callerDepth; i++)
            {
                builder.Append(DefaultDepthIdentifier);
            }

            return builder.ToString();
        }

        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public void WriteError(Exception ex)
        {
            Console.Write(ex);
        }

        public MethodBase GetCallerName(int stackNumber = 1)
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(stackNumber);
            return stackFrame.GetMethod();
        }
        
        public int GetCallerDepth(int stackNumber = 1)
        {
            StackTrace stackTrace = new StackTrace();
            return stackTrace.GetFrames().Length - stackNumber;
        }

        public string BuildDateTimeString()
        {
            return DateTime.Now.ToString(DateTimeFormat, CultureInfo.CurrentCulture);
        }
    }
}