using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Norika.MsBuild.DocumentationGenerator.Business.IO;

namespace Norika.MsBuild.DocumentationGenerator.Business.IntegrationTest
{
    [TestClass]
    public class DefaultFileSystemReaderImplementationIntegrationTest
    {
        [TestMethod]
        [DeploymentItem("TestData/FileSystemData.txt")]
        public void ReadAllText_FromExistentResource_ShouldReturnCorrectContent()
        {
            DefaultFileSystemReaderImplementation readerSut = new DefaultFileSystemReaderImplementation();

            string returnValue = readerSut.ReadAllText("TestData/FileSystemData.txt");

            Assert.AreEqual("This is a test content.\nWith two lines.", returnValue);
        }

        [TestMethod]
        [DeploymentItem("TestData/FileSystemDataUnicode.txt")]
        public void ReadAllText_FromExistentUnicodeResourceWithUnicodeEncoding_ShouldReturnCorrectContent()
        {
            DefaultFileSystemReaderImplementation readerSut = new DefaultFileSystemReaderImplementation();

            string returnValue = readerSut.ReadAllText("TestData/FileSystemDataUnicode.txt", Encoding.UTF8);

            Assert.AreEqual("This is a test content.\nWith two lines and unicode char: ۩.", returnValue);
        }

        [TestMethod]
        [DeploymentItem("TestData/FileSystemData.txt")]
        public void ReadAllLines_FromExistentResource_ShouldReturnCorrectContent()
        {
            DefaultFileSystemReaderImplementation readerSut = new DefaultFileSystemReaderImplementation();

            IList<string> returnValue = readerSut.ReadAllLines("TestData/FileSystemData.txt");

            Assert.AreEqual("This is a test content.", returnValue[0]);
            Assert.AreEqual("With two lines.", returnValue[1]);
        }

        [TestMethod]
        [DeploymentItem("TestData/FileSystemDataUnicode.txt")]
        public void ReadAllLines_FromExistentUnicodeResourceWithUnicodeEncoding_ShouldReturnCorrectContent()
        {
            DefaultFileSystemReaderImplementation readerSut = new DefaultFileSystemReaderImplementation();

            IList<string> returnValue = readerSut.ReadAllLines("TestData/FileSystemDataUnicode.txt", Encoding.UTF8);

            Assert.AreEqual("This is a test content.", returnValue[0]);
            Assert.AreEqual("With two lines and unicode char: ۩.", returnValue[1]);
        }

        [TestMethod]
        [DeploymentItem("TestData/FileSystemDataUnicode.txt")]
        public void ReadAllLines_FromExistentUnicodeResourceWithAsciiEncoding_ShouldNotReturnCorrectContent()
        {
            DefaultFileSystemReaderImplementation readerSut = new DefaultFileSystemReaderImplementation();

            IList<string> returnValue = readerSut.ReadAllLines("TestData/FileSystemDataUnicode.txt", Encoding.ASCII);

            Assert.AreEqual("This is a test content.", returnValue[0]);
            Assert.AreNotEqual("With two lines and unicode char: ۩.", returnValue[1]);
        }

        [TestMethod]
        [DeploymentItem("TestData/FileSystemDataUnicode.txt")]
        public void ReadAllText_FromExistentUnicodeResourceWithAsciiEncoding_ShouldNotReturnCorrectContent()
        {
            DefaultFileSystemReaderImplementation readerSut = new DefaultFileSystemReaderImplementation();

            string returnValue = readerSut.ReadAllText("TestData/FileSystemDataUnicode.txt", Encoding.ASCII);

            Assert.AreNotEqual("This is a test content.\nWith two lines and unicode char: ۩.", returnValue);
        }
    }
}