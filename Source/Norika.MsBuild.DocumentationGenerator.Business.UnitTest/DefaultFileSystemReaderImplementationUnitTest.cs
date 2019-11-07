using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Norika.MsBuild.DocumentationGenerator.Business.IO;

namespace Norika.MsBuild.DocumentationGenerator.Business.UnitTest
{
    [TestClass]
    public class DefaultFileSystemReaderImplementationUnitTest
    {
        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void ReadAllText_WithNotExistentFilePath_ShouldThrowFileNotFoundException()
        {
            DefaultFileSystemReaderImplementation readerSut = new DefaultFileSystemReaderImplementation();
            readerSut.ReadAllText("a/b/c/NotExistent/NotExistentTestFile.fab");
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void ReadAllText_WithEncodingAndWithNotExistentFilePath_ShouldThrowFileNotFoundException()
        {
            DefaultFileSystemReaderImplementation readerSut = new DefaultFileSystemReaderImplementation();
            readerSut.ReadAllText("a/b/c/NotExistent/NotExistentTestFile.fab", Encoding.UTF8);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void ReadAllLines_WithEncodingAndWithNotExistentFilePath_ShouldThrowFileNotFoundException()
        {
            DefaultFileSystemReaderImplementation readerSut = new DefaultFileSystemReaderImplementation();
            readerSut.ReadAllLines("a/b/c/NotExistent/NotExistentTestFile.fab", Encoding.UTF8);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void ReadAllLines_WithNotExistentFilePath_ShouldThrowFileNotFoundException()
        {
            DefaultFileSystemReaderImplementation readerSut = new DefaultFileSystemReaderImplementation();
            readerSut.ReadAllLines("a/b/c/NotExistent/NotExistentTestFile.fab");
        }
    }
}