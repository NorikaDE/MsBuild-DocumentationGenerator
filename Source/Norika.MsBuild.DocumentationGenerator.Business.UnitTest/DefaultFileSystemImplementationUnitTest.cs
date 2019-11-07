using Microsoft.VisualStudio.TestTools.UnitTesting;
using Norika.MsBuild.DocumentationGenerator.Business.IO;

namespace Norika.MsBuild.DocumentationGenerator.Business.UnitTest
{
    [TestClass]
    public class DefaultFileSystemImplementationUnitTest
    {
        [TestMethod]
        public void GetFileName_FromFileNameBuildFromNameAndExtension_ShouldReturnFileNameCorrectlyWithExtension()
        {
            DefaultFileSystemImplementation fileSystemSut = new DefaultFileSystemImplementation();

            string returnValue = fileSystemSut.GetFileName("TestFile.txt");

            Assert.AreEqual("TestFile.txt", returnValue);
        }

        [TestMethod]
        public void
            GetFileName_FromFileNameBuildFromDirectoryAndNameAndExtension_ShouldReturnFileNameCorrectlyWithExtension()
        {
            DefaultFileSystemImplementation fileSystemSut = new DefaultFileSystemImplementation();

            string returnValue = fileSystemSut.GetFileName("Directory/TestFile.txt");

            Assert.AreEqual("TestFile.txt", returnValue);
        }
    }
}