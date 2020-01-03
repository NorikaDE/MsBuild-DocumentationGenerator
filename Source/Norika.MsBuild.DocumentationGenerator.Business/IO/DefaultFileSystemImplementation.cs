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

        public void DeleteDirectory(string path)
        {
            Directory.Delete(path, true);
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public bool Exists(string path)
        {
            if (Directory.Exists(path)) return true;
            if (File.Exists(path)) return true;
            return false;
        }

        public IFile GetFile(string path)
        {
            return new DefaultFileImplementation(path);
        }
    }
}