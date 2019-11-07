using System;

namespace Norika.MsBuild.DocumentationGenerator.Business
{
    /// <summary>
    /// Exception for DocumentationGenerator errors
    /// </summary>
    public class DocumentationGeneratorException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Describing error message</param>
        public DocumentationGeneratorException(string message) : base(message)
        {
        }
    }
}