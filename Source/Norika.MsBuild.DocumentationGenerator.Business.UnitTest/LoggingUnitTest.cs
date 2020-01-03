using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Norika.MsBuild.DocumentationGenerator.Business.Logging;

namespace Norika.MsBuild.DocumentationGenerator.Business.UnitTest
{
    [TestClass]
    public class LoggingUnitTest
    {
        [TestMethod]
        public void WriteDebug_Test_ShouldPrintThisMessagesName()
        {
            DefaultConsoleApplicationLoggerImplementation logger = new DefaultConsoleApplicationLoggerImplementation();

            logger.WriteDebug("This is a test method");
        }
        
        [TestMethod]
        public void GetCallerName_FromTestMethod_ShouldReturnThisMethodName()
        {
            DefaultConsoleApplicationLoggerImplementation logger = new DefaultConsoleApplicationLoggerImplementation();

            MethodBase returnValue = logger.GetCallerName();
            
            Assert.AreEqual("GetCallerName_FromTestMethod_ShouldReturnThisMethodName", returnValue.Name);
        }
        
        [TestMethod]
        public void GetCallerName_FromNestedWrappedMethod_ShouldReturnThisMethodName()
        {
            DefaultConsoleApplicationLoggerImplementation logger = new DefaultConsoleApplicationLoggerImplementation();

            MethodBase returnValue = GetCallerNameWrapper(2, logger);
            
            Assert.AreEqual("GetCallerName_FromNestedWrappedMethod_ShouldReturnThisMethodName", 
                returnValue.Name);
        }
        
        [TestMethod]
        public void GetCallerDepth_FromNestedWrappedMethod_ShouldReturnIntValueRepresentingThisMethodsCallStack()
        {
            DefaultConsoleApplicationLoggerImplementation logger = new DefaultConsoleApplicationLoggerImplementation();

            StackTrace stackTrace = new StackTrace();

            int expectedValue = stackTrace.GetFrames().Length;
            int returnValue = GetCallerDepthWrapper(2, logger);
            
            Assert.AreEqual(expectedValue, 
                returnValue);
        }
        
        [TestMethod]
        public void GetCallerDepth_FromTestMethod_ShouldReturnIntValueRepresentingThisMethodsCallStack()
        {
            DefaultConsoleApplicationLoggerImplementation logger = new DefaultConsoleApplicationLoggerImplementation();

            StackTrace stackTrace = new StackTrace();

            int expectedValue = stackTrace.GetFrames().Length;
            int returnValue = logger.GetCallerDepth();
            
            Assert.AreEqual(expectedValue, 
                returnValue);
        }

        [TestMethod]
        public void BuildDateTimeString_FromDateTimeNow_ShouldMatchGivenPattern()
        {
            DefaultConsoleApplicationLoggerImplementation logger = new DefaultConsoleApplicationLoggerImplementation();

            Regex pattern = new Regex("\\d{2}-\\d{2}-\\d{4} \\d{2}:\\d{2}:\\d{2}");

            string builtDateTime = logger.BuildDateTimeString();
            Console.WriteLine(builtDateTime);
            
            Assert.IsTrue(pattern.IsMatch(builtDateTime));
        }
        
        private MethodBase GetCallerNameWrapper(int stackNumber, DefaultConsoleApplicationLoggerImplementation logger)
        {
            return logger.GetCallerName(stackNumber);
        }
        
        private int GetCallerDepthWrapper(int stackNumber, DefaultConsoleApplicationLoggerImplementation logger)
        {
            return logger.GetCallerDepth(stackNumber);
        }
    }
}