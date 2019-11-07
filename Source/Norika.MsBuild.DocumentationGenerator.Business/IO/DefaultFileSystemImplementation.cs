using System.IO;
using Norika.MsBuild.DocumentationGenerator.Business.IO.Interfaces;

namespace Norika.MsBuild.DocumentationGenerator.Business.IO
{
    public class DefaultFileSystemImplementation : IFileSystem
    {
        public string GetFileName(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            return fileInfo.Name;
        }
    }
}