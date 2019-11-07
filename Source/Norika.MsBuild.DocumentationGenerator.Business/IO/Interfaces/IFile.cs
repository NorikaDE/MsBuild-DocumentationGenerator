using System.Collections.Generic;

namespace Norika.MsBuild.DocumentationGenerator.Business.IO.Interfaces
{
    public interface IFile
    {
        string FullPath { get; }
        string FileName { get; }
        string Extension { get; }

        string ReadAllText();

        IList<string> ReadAllLines();
    }
}