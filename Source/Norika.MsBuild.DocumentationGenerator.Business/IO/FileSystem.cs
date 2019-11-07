using Norika.MsBuild.DocumentationGenerator.Business.IO.Interfaces;

namespace Norika.MsBuild.DocumentationGenerator.Business.IO
{
    public class FileSystem
    {
        public static IFile GetFileInformation(string path)
        {
            return new DefaultFileImplementation(path);
        }
    }
}