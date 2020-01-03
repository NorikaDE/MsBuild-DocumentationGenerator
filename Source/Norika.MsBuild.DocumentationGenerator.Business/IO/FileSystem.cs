using Norika.MsBuild.DocumentationGenerator.Business.IO.Interfaces;

namespace Norika.MsBuild.DocumentationGenerator.Business.IO
{
    public static class FileSystem
    {
        private static IFileSystem _fileSystemImplementation;
        public static IFileSystem Accessor => _fileSystemImplementation ?? 
                                       (_fileSystemImplementation = new DefaultFileSystemImplementation());

        public static IFile GetFileInformation(string path)
        {
            return Accessor.GetFile(path);
        }

        public static void SetAccessor(IFileSystem fileSystem)
        {
            _fileSystemImplementation = fileSystem;
        }
    }
}