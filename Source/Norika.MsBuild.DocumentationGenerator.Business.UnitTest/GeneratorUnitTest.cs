using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Norika.Documentation.Core;
using Norika.Documentation.Core.Types;
using Norika.Documentation.Markdown.Container;
using Norika.Documentation.Markdown.Container.Interfaces;
using Norika.MsBuild.DocumentationGenerator.Business.IO.Interfaces;

namespace Norika.MsBuild.DocumentationGenerator.Business.UnitTest
{
    // Todo: Fix resource manager
    [TestClass]
    public class GeneratorUnitTest
    {
        [TestMethod]
        public void
            CreateBody_WithInputFileContainingExactlyOneProperty_ShouldCreateNewChapterForPropertiesInOutputDocument()
        {
            string testProjectContent = "<Project><PropertyGroup><Property1>#1</Property1></PropertyGroup></Project>";

            Mock<IFile> fileMock = new Mock<IFile>();
            fileMock.Setup(fm => fm.Extension).Returns("targets");
            fileMock.Setup(fm => fm.FileName).Returns("Test.targets");
            fileMock.Setup(fm => fm.FullPath).Returns("test/Test.targets");
            fileMock.Setup(fm => fm.ReadAllText()).Returns(testProjectContent);

            Mock<IPrintableDocumentChapter> chapterMock = new Mock<IPrintableDocumentChapter>();
            Mock<IPrintableDocumentChapterStringContent> chapterContentMock =
                new Mock<IPrintableDocumentChapterStringContent>();
            Mock<IPrintableParagraphTable> tableMock = new Mock<IPrintableParagraphTable>();
            Mock<IPrintableParagraphTableDataRow> tableRowMock = new Mock<IPrintableParagraphTableDataRow>();


            chapterMock.Setup(c => c.AddNewContent<IPrintableDocumentChapterStringContent>())
                .Returns(chapterContentMock.Object);
            chapterMock.Setup(c => c.AddNewContent<IPrintableParagraphTable>()).Returns(tableMock.Object);

            Mock<IMarkdownDocument> markdownDocumentMock = new Mock<IMarkdownDocument>();
            markdownDocumentMock.Setup(md => md.AddNewChapter(It.IsAny<string>())).Returns(chapterMock.Object);

            Mock<PrintableDocument<IMarkdownDocument>> printableMarkdownDocument =
                new Mock<PrintableDocument<IMarkdownDocument>>();
            printableMarkdownDocument.Setup(pmd => pmd.Create(It.IsAny<string>())).Returns(markdownDocumentMock.Object);


            ProjectOverviewGenerator<IMarkdownDocument> documentationGenerator =
                new ProjectOverviewGenerator<IMarkdownDocument>(
                    fileMock.Object, printableMarkdownDocument.Object);

            documentationGenerator.CreateBody();

            markdownDocumentMock.Verify(md => md.AddNewChapter(It.Is<string>(s => s.Equals("Properties"))));
        }

        [TestMethod]
        public void
            CreateBody_WithInputFileContainingExactlyOneProperty_ShouldCreateEntryInPropertyChapterForDefinedProperty()
        {
            string testProjectContent = "<Project><PropertyGroup><Property1>#1</Property1></PropertyGroup></Project>";

            Mock<IFile> fileMock = new Mock<IFile>();
            fileMock.Setup(fm => fm.Extension).Returns("targets");
            fileMock.Setup(fm => fm.FileName).Returns("Test.targets");
            fileMock.Setup(fm => fm.FullPath).Returns("test/Test.targets");
            fileMock.Setup(fm => fm.ReadAllText()).Returns(testProjectContent);

            Mock<IPrintableDocumentChapter> chapterMock = new Mock<IPrintableDocumentChapter>();
            Mock<IPrintableDocumentChapterStringContent> chapterContentMock =
                new Mock<IPrintableDocumentChapterStringContent>();
            Mock<IPrintableParagraphTable> tableMock = new Mock<IPrintableParagraphTable>();

            chapterMock.Setup(c => c.AddNewContent<IPrintableDocumentChapterStringContent>())
                .Returns(chapterContentMock.Object);
            chapterMock.Setup(c => c.AddNewContent<IPrintableParagraphTable>()).Returns(tableMock.Object);

            Mock<IMarkdownDocument> markdownDocumentMock = new Mock<IMarkdownDocument>();
            markdownDocumentMock.Setup(md => md.AddNewChapter(It.IsAny<string>())).Returns(chapterMock.Object);

            Mock<PrintableDocument<IMarkdownDocument>> printableMarkdownDocument =
                new Mock<PrintableDocument<IMarkdownDocument>>();
            printableMarkdownDocument.Setup(pmd => pmd.Create(It.IsAny<string>())).Returns(markdownDocumentMock.Object);


            ProjectOverviewGenerator<IMarkdownDocument> documentationGenerator =
                new ProjectOverviewGenerator<IMarkdownDocument>(
                    fileMock.Object, printableMarkdownDocument.Object);

            documentationGenerator.CreateBody();

            tableMock.Verify(md => md.WithRow(It.Is<string[]>(s => s[0].Equals("Property1"))));
        }

        [TestMethod]
        public void
            CreateBody_WithInputFileContainingExactlyOnePropertyWithoutTargets_ShouldNotCreateTargetsSectionInOutputDocument()
        {
            string testProjectContent = "<Project><PropertyGroup><Property1>#1</Property1></PropertyGroup></Project>";

            Mock<IFile> fileMock = new Mock<IFile>();
            fileMock.Setup(fm => fm.Extension).Returns("targets");
            fileMock.Setup(fm => fm.FileName).Returns("Test.targets");
            fileMock.Setup(fm => fm.FullPath).Returns("test/Test.targets");
            fileMock.Setup(fm => fm.ReadAllText()).Returns(testProjectContent);

            Mock<IPrintableDocumentChapter> chapterMock = new Mock<IPrintableDocumentChapter>();
            Mock<IPrintableDocumentChapterStringContent> chapterContentMock =
                new Mock<IPrintableDocumentChapterStringContent>();
            Mock<IPrintableParagraphTable> tableMock = new Mock<IPrintableParagraphTable>();

            chapterMock.Setup(c => c.AddNewContent<IPrintableDocumentChapterStringContent>())
                .Returns(chapterContentMock.Object);
            chapterMock.Setup(c => c.AddNewContent<IPrintableParagraphTable>()).Returns(tableMock.Object);

            Mock<IMarkdownDocument> markdownDocumentMock = new Mock<IMarkdownDocument>();
            markdownDocumentMock.Setup(md => md.AddNewChapter(It.IsAny<string>())).Returns(chapterMock.Object);

            Mock<PrintableDocument<IMarkdownDocument>> printableMarkdownDocument =
                new Mock<PrintableDocument<IMarkdownDocument>>();
            printableMarkdownDocument.Setup(pmd => pmd.Create(It.IsAny<string>())).Returns(markdownDocumentMock.Object);


            ProjectOverviewGenerator<IMarkdownDocument> documentationGenerator =
                new ProjectOverviewGenerator<IMarkdownDocument>(
                    fileMock.Object, printableMarkdownDocument.Object);

            documentationGenerator.CreateBody();

            markdownDocumentMock.Verify(md => md.AddNewChapter(It.Is<string>(s => s.Equals("Targets"))), Times.Never);
        }

        [TestMethod]
        [Ignore]
        public void Test1()
        {
            string resourceContent = ResourceManager.GetResourceString(
                "Norika.MsBuild.DocumentationGenerator.Business.UnitTest.TestData.PropertyOnlyProjectFile.xml");

            Mock<IFile> fileMock = new Mock<IFile>();
            fileMock.Setup(fm => fm.Extension).Returns("target");
            fileMock.Setup(fm => fm.FileName).Returns("Test.target");
            fileMock.Setup(fm => fm.FullPath).Returns("test/Test.target");
            fileMock.Setup(fm => fm.ReadAllText()).Returns(resourceContent);

            ProjectOverviewGenerator<IMarkdownDocument> documentationGenerator =
                new ProjectOverviewGenerator<IMarkdownDocument>(fileMock.Object);
            documentationGenerator.CreateBody();

            string returnValue = documentationGenerator.Print();
        }

        [TestMethod]
        [Ignore]
        public void Test2()
        {
            string resourceContent = ResourceManager.GetResourceString(
                "Norika.MsBuild.DocumentationGenerator.Business.UnitTest.TestData.TargetOnlyProjectFile.xml");

            Mock<IFile> fileMock = new Mock<IFile>();
            fileMock.Setup(fm => fm.Extension).Returns("target");
            fileMock.Setup(fm => fm.FileName).Returns("Test.target");
            fileMock.Setup(fm => fm.FullPath).Returns("test/Test.target");
            fileMock.Setup(fm => fm.ReadAllText()).Returns(resourceContent);

            ProjectOverviewGenerator<IMarkdownDocument> documentationGenerator =
                new ProjectOverviewGenerator<IMarkdownDocument>(fileMock.Object);
            documentationGenerator.CreateBody();

            string returnValue = documentationGenerator.Print();
        }

        [TestMethod]
        [Ignore]
        public void Test3()
        {
            string resourceContent = ResourceManager.GetResourceString(
                "Norika.MsBuild.DocumentationGenerator.Business.UnitTest.TestData.TargetAndPropertyProjectFile.xml");

            Mock<IFile> fileMock = new Mock<IFile>();
            fileMock.Setup(fm => fm.Extension).Returns("target");
            fileMock.Setup(fm => fm.FileName).Returns("Test.target");
            fileMock.Setup(fm => fm.FullPath).Returns("test/Test.target");
            fileMock.Setup(fm => fm.ReadAllText()).Returns(resourceContent);

            ProjectOverviewGenerator<IMarkdownDocument> documentationGenerator =
                new ProjectOverviewGenerator<IMarkdownDocument>(fileMock.Object);
            documentationGenerator.CreateBody();

            string returnValue = documentationGenerator.Print();
        }
    }

    public static class ResourceManager
    {
        public static string GetResourceString(string resourceName)
        {
            var assembly = typeof(GeneratorUnitTest).GetTypeInfo().Assembly;

            Stream resource = assembly.GetManifestResourceStream(
                resourceName);

            StreamReader reader = new StreamReader(resource);
            string resourceContent = reader.ReadToEnd();

            return resourceContent;
        }
    }
}