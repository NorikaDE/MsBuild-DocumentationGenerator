using System.Collections.Generic;
using System.IO;
using Norika.MsBuild.DocumentationGenerator.Business.IO.Interfaces;

namespace Norika.MsBuild.DocumentationGenerator.Business.IO
{
    public class DefaultFileImplementation : IFile
    {
        public DefaultFileImplementation(string fullPath)
        {
            FileInfo fileInfo = new FileInfo(fullPath);
            FullPath = fileInfo.FullName;
            FileName = fileInfo.Name;
            Extension = fileInfo.Extension;
        }

        public string FullPath { get; }
        public string FileName { get; }
        public string Extension { get; }

        public string ReadAllText()
        {
            DefaultFileSystemReaderImplementation readerImplementation = new DefaultFileSystemReaderImplementation();
            return readerImplementation.ReadAllText(FullPath);
        }

        public IList<string> ReadAllLines()
        {
            DefaultFileSystemReaderImplementation readerImplementation = new DefaultFileSystemReaderImplementation();
            return readerImplementation.ReadAllLines(FullPath);
        }
    }
}