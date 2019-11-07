using System.Collections.Generic;
using System.IO;
using System.Text;
using Norika.MsBuild.DocumentationGenerator.Business.IO.Interfaces;

namespace Norika.MsBuild.DocumentationGenerator.Business.IO
{
    public class DefaultFileSystemReaderImplementation : IFileSystemReader
    {
        public string ReadAllText(string path)
        {
            return ReadAllText(path, Encoding.Default);
        }

        public string ReadAllText(string path, Encoding encoding)
        {
            if (!(File.Exists(path))) throw new FileNotFoundException("The given file could not be found!", path);
            return File.ReadAllText(path, encoding);
        }

        public IList<string> ReadAllLines(string path)
        {
            return ReadAllLines(path, Encoding.Default);
        }

        public IList<string> ReadAllLines(string path, Encoding encoding)
        {
            if (!(File.Exists(path))) throw new FileNotFoundException("The given file could not be found!", path);
            return File.ReadAllLines(path, encoding);
        }
    }
}