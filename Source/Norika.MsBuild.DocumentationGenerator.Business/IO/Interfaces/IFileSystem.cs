namespace Norika.MsBuild.DocumentationGenerator.Business.IO.Interfaces
{
    public interface IFileSystem
    {
        string GetFileName(string path);

        void DeleteDirectory(string path);

        void CreateDirectory(string path);

        bool Exists(string path);
        IFile GetFile(string path);
    }
}