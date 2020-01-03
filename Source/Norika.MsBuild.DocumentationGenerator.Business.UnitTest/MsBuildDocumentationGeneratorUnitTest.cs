using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Norika.Documentation.Core;
using Norika.Documentation.Core.Types;
using Norika.Documentation.Markdown.Container.Interfaces;
using Norika.MsBuild.DocumentationGenerator.Business.IO;
using Norika.MsBuild.DocumentationGenerator.Business.IO.Interfaces;

namespace Norika.MsBuild.DocumentationGenerator.Business.UnitTest
{
    [TestClass]
    public class MsBuildDocumentationGeneratorUnitTest
    {
        private Mock<IFileSystem> CreateFileSystemMock(string directory, string file, bool directoryExists)
        {
            Mock<IFileSystem> fileSystemMock = new Mock<IFileSystem>();
            Mock<IFile> fileMock = new Mock<IFile>();
            
            fileMock.Setup(fm => fm.ReadAllText()).Returns("<Project></Project>");
            fileSystemMock.Setup(fsm => fsm.Exists(It.Is<string>(p => p.Equals(directory))))
                .Returns(directoryExists);
            fileSystemMock.Setup(fsm => fsm.GetFile(It.Is<string>(f => f.Equals(file))))
                .Returns(fileMock.Object);

            return fileSystemMock;
        }

        private Mock<PrintableDocument<IMarkdownDocument>> CreatePrintableDocumentMock()
        {
            Mock<PrintableDocument<IMarkdownDocument>> printableDocumentMock = new Mock<PrintableDocument<IMarkdownDocument>>();
            Mock<IMarkdownDocument> markdownDocumentMock = new Mock<IMarkdownDocument>();
            
            printableDocumentMock.Setup(pdm => pdm.Save(It.IsAny<string>(), It.IsAny<IPrintableDocument>()))
                .Returns(true);
            printableDocumentMock.Setup(pdm => pdm.Create(It.IsAny<string>())).Returns(markdownDocumentMock.Object);
            return printableDocumentMock;
        }
        
        [TestMethod]
        public void PrepareOutputDirectory_WithExistentDirectory_ShouldCallDelete()
        {
            string fileName = "test.targets";
            string testDirectoryName = "TestDirectory";

            var fileSystemMock = CreateFileSystemMock(testDirectoryName, fileName, true);
            var printableDocumentMock = CreatePrintableDocumentMock();
            
            FileSystem.SetAccessor(fileSystemMock.Object);
            
            MsBuildDocumentationGenerator<IMarkdownDocument> documentationGenerator = 
                new MsBuildDocumentationGenerator<IMarkdownDocument>(fileName, printableDocumentMock.Object);
            
            documentationGenerator.CreateDocumentation(testDirectoryName);
            
            fileSystemMock.Verify(fsm => fsm.DeleteDirectory(testDirectoryName), Times.Exactly(1));
        }
        
        [TestMethod]
        public void PrepareOutputDirectory_WithNotExistentDirectory_ShouldNotCallDelete()
        {
            string fileName = "test.targets";
            string testDirectoryName = "TestDirectory";
            
            var fileSystemMock = CreateFileSystemMock(testDirectoryName, fileName, false);
            var printableDocumentMock = CreatePrintableDocumentMock();
            
            FileSystem.SetAccessor(fileSystemMock.Object);
            
            MsBuildDocumentationGenerator<IMarkdownDocument> documentationGenerator = 
                new MsBuildDocumentationGenerator<IMarkdownDocument>(fileName, printableDocumentMock.Object);
            
            documentationGenerator.CreateDocumentation(testDirectoryName);
            
            fileSystemMock.Verify(fsm => fsm.DeleteDirectory(testDirectoryName), Times.Exactly(0));
        }
        
        [TestMethod]
        public void PrepareOutputDirectory_WithNotExistentDirectory_ShouldCallCreate()
        {
            string fileName = "test.targets";
            string testDirectoryName = "TestDirectory";
            
            var fileSystemMock = CreateFileSystemMock(testDirectoryName, fileName, false);
            var printableDocumentMock = CreatePrintableDocumentMock();
            
            FileSystem.SetAccessor(fileSystemMock.Object);
            
            MsBuildDocumentationGenerator<IMarkdownDocument> documentationGenerator = 
                new MsBuildDocumentationGenerator<IMarkdownDocument>(fileName, printableDocumentMock.Object);
            
            documentationGenerator.CreateDocumentation(testDirectoryName);
            
            fileSystemMock.Verify(fsm => fsm.CreateDirectory(testDirectoryName), Times.Exactly(1));
        }
        
        [TestMethod]
        public void PrepareOutputDirectory_WithExistentDirectory_ShouldCallCreateOfNewDirectoryAfterDelete()
        {
            string fileName = "test.targets";
            string testDirectoryName = "TestDirectory";
            
            var fileSystemMock = CreateFileSystemMock(testDirectoryName, fileName, true);
            var printableDocumentMock = CreatePrintableDocumentMock();
            
            FileSystem.SetAccessor(fileSystemMock.Object);
            
            MsBuildDocumentationGenerator<IMarkdownDocument> documentationGenerator = 
                new MsBuildDocumentationGenerator<IMarkdownDocument>(fileName, printableDocumentMock.Object);
            
            documentationGenerator.CreateDocumentation(testDirectoryName);
            
            fileSystemMock.Verify(fsm => fsm.DeleteDirectory(testDirectoryName), Times.Exactly(1));
            fileSystemMock.Verify(fsm => fsm.CreateDirectory(testDirectoryName), Times.Exactly(1));
        }
        
        [TestMethod]
        public void PrepareOutputDirectory_WithExistentDirectoryAndExceptionOnDelete_ShouldCallCreateEvenOnExceptionThrown()
        {
            string fileName = "test.targets";
            string testDirectoryName = "TestDirectory";
            
            Mock<IFileSystem> fileSystemMock = new Mock<IFileSystem>();
            Mock<IFile> fileMock = new Mock<IFile>();
            Mock<IMarkdownDocument> markdownDocumentMock = new Mock<IMarkdownDocument>();
            Mock<PrintableDocument<IMarkdownDocument>> printableDocumentMock = new Mock<PrintableDocument<IMarkdownDocument>>();
           
            printableDocumentMock.Setup(pdm => pdm.Save(It.IsAny<string>(), It.IsAny<IPrintableDocument>()))
                .Returns(true);
            printableDocumentMock.Setup(pdm => pdm.Create(It.IsAny<string>())).Returns(markdownDocumentMock.Object);
            
            fileMock.Setup(fm => fm.ReadAllText()).Returns("<Project></Project>");
            fileSystemMock.Setup(fsm => fsm.Exists(It.Is<string>(p => p.Equals(testDirectoryName))))
                .Returns(true);
            fileSystemMock.Setup(fsm => fsm.GetFile(It.Is<string>(f => f.Equals(fileName))))
                .Returns(fileMock.Object);
            fileSystemMock.Setup(fsm => fsm.DeleteDirectory(testDirectoryName)).Throws<IOException>();
            
            FileSystem.SetAccessor(fileSystemMock.Object);
            
            MsBuildDocumentationGenerator<IMarkdownDocument> documentationGenerator = 
                new MsBuildDocumentationGenerator<IMarkdownDocument>(fileName, printableDocumentMock.Object);
            
            documentationGenerator.CreateDocumentation(testDirectoryName);
            
            fileSystemMock.Verify(fsm => fsm.DeleteDirectory(testDirectoryName), Times.Exactly(1));
            fileSystemMock.Verify(fsm => fsm.CreateDirectory(testDirectoryName), Times.Exactly(1));
        }
    }
}