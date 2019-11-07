using System.Collections.Generic;
using System.Text;

namespace Norika.MsBuild.DocumentationGenerator.Business.IO.Interfaces
{
    public interface IFileSystemReader
    {
        string ReadAllText(string path);

        string ReadAllText(string path, Encoding encoding);

        IList<string> ReadAllLines(string path);
        IList<string> ReadAllLines(string path, Encoding encoding);
    }
}