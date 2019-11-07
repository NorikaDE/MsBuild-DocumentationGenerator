using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Norika.Documentation.Core;
using Norika.Documentation.Core.Types;
using Norika.Documentation.Markdown.Container.Interfaces;
using Norika.MsBuild.Model.Interfaces;

namespace Norika.MsBuild.DocumentationGenerator.Business.UnitTest
{
    [TestClass]
    public class TargetOverviewSiteGeneratorUnitTest
    {
        [TestMethod]
        public void
            CreateOverview_WithOneProperlyNamedTarget_ShouldCallCreationOfNewDocumentWithTitleMatchingTheTargetName()
        {
            #region 1) Arrange

            Mock<IMsBuildElementHelp> elementHelpMock = new Mock<IMsBuildElementHelp>();

            Mock<IMsBuildTarget> targetMock = new Mock<IMsBuildTarget>();
            targetMock.Setup(t => t.Name).Returns("TargetA");
            targetMock.Setup(t => t.Help).Returns(elementHelpMock.Object);

            Mock<IMarkdownDocument> markdownDocumentMock = new Mock<IMarkdownDocument>();
            markdownDocumentMock.Setup(md => md.DefaultFileExtension).Returns("md");

            Mock<PrintableDocument<IMarkdownDocument>> printableDocumentMock =
                new Mock<PrintableDocument<IMarkdownDocument>>();

            printableDocumentMock.Setup(pd => pd.Create(It.IsAny<string>()))
                .Returns(markdownDocumentMock.Object);

            TargetOverviewSiteGenerator<IMarkdownDocument> markdownTargetDocumentGenerator =
                new TargetOverviewSiteGenerator<IMarkdownDocument>(printableDocumentMock.Object);

            #endregion

            // 2) Act
            IPrintableDocument document = markdownTargetDocumentGenerator.CreateOverview(targetMock.Object);

            // 3) Assert
            printableDocumentMock.Verify(pd => pd.Create(
                It.Is<string>(s => s.Equals("TargetA"))), Times.Exactly(1));
        }

        [TestMethod]
        public void
            CreateOverview_WithTargetContainingDescriptionAsHelpContent_ShouldCallCreationOfNewDocumentChapterNamedDescription()
        {
            #region 1) Arrange

            IMsBuildElementHelp help = MsBuildTestDataBuilder.Create<IMsBuildTarget>()
                .SetName("TargetA")
                .WithHelp()
                .AddHelpParagraph("DESCRIPTION", "This is a target description", "$(ParamPropertyOne)")
                .Build();

            Mock<IMsBuildTarget> targetMock = new Mock<IMsBuildTarget>();
            targetMock.Setup(t => t.Name).Returns("TargetA");
            targetMock.Setup(t => t.Help).Returns(help);

            Mock<IPrintableDocumentChapterStringContent> chapterContentMock =
                new Mock<IPrintableDocumentChapterStringContent>();

            Mock<IPrintableDocumentChapter> chapterMock = new Mock<IPrintableDocumentChapter>();
            chapterMock.Setup(c => c.AddNewContent<IPrintableDocumentChapterStringContent>())
                .Returns(chapterContentMock.Object);

            Mock<IMarkdownDocument> markdownDocumentMock = new Mock<IMarkdownDocument>();
            markdownDocumentMock.Setup(md => md.DefaultFileExtension).Returns("md");
            markdownDocumentMock.Setup(md => md.AddNewChapter(It.IsAny<string>())).Returns(chapterMock.Object);

            Mock<PrintableDocument<IMarkdownDocument>> printableDocumentMock =
                new Mock<PrintableDocument<IMarkdownDocument>>();

            printableDocumentMock.Setup(pd => pd.Create(It.IsAny<string>()))
                .Returns(markdownDocumentMock.Object);

            TargetOverviewSiteGenerator<IMarkdownDocument> markdownTargetDocumentGenerator =
                new TargetOverviewSiteGenerator<IMarkdownDocument>(printableDocumentMock.Object);

            #endregion

            // 2) Act
            IPrintableDocument document = markdownTargetDocumentGenerator.CreateOverview(targetMock.Object);

            // 3) Assert
            markdownDocumentMock.Verify(md => md.AddNewChapter(
                It.Is<string>(s => s.Equals("Description", StringComparison.OrdinalIgnoreCase))), Times.Exactly(1));
        }

        [TestMethod]
        public void CreateOverview_WithTargetContainingDescriptionAsHelpContent_ShouldSetCorrectDescriptionContent()
        {
            #region 1) Arrange

            string targetDescriptionText = "This is the target description";

            IMsBuildElementHelp help = MsBuildTestDataBuilder.Create<IMsBuildTarget>()
                .SetName("TargetA")
                .WithHelp()
                .AddHelpParagraph("DESCRIPTION", targetDescriptionText, "$(ParamPropertyOne)")
                .Build();

            Mock<IMsBuildTarget> targetMock = new Mock<IMsBuildTarget>();
            targetMock.Setup(t => t.Name).Returns("TargetA");
            targetMock.Setup(t => t.Help).Returns(help);

            Mock<IPrintableDocumentChapterStringContent> chapterContentMock =
                new Mock<IPrintableDocumentChapterStringContent>();
            chapterContentMock.SetupSet(cc => cc.Content = It.IsAny<string>()).Verifiable();

            Mock<IPrintableDocumentChapter> chapterMock = new Mock<IPrintableDocumentChapter>();
            chapterMock.Setup(c => c.AddNewContent<IPrintableDocumentChapterStringContent>())
                .Returns(chapterContentMock.Object);

            Mock<IMarkdownDocument> markdownDocumentMock = new Mock<IMarkdownDocument>();
            markdownDocumentMock.Setup(md => md.DefaultFileExtension).Returns("md");
            markdownDocumentMock.Setup(md => md.AddNewChapter(It.IsAny<string>())).Returns(chapterMock.Object);

            Mock<PrintableDocument<IMarkdownDocument>> printableDocumentMock =
                new Mock<PrintableDocument<IMarkdownDocument>>();

            printableDocumentMock.Setup(pd => pd.Create(It.IsAny<string>()))
                .Returns(markdownDocumentMock.Object);

            TargetOverviewSiteGenerator<IMarkdownDocument> markdownTargetDocumentGenerator =
                new TargetOverviewSiteGenerator<IMarkdownDocument>(printableDocumentMock.Object);

            #endregion

            // 2) Act
            IPrintableDocument document = markdownTargetDocumentGenerator.CreateOverview(targetMock.Object);

            // 3) Assert
            chapterContentMock.VerifySet(cc => cc.Content = It.Is<string>(
                s => s.Trim().Equals(targetDescriptionText)));
        }

        [TestMethod]
        public void
            CreateOverview_WithTargetContainingTwoParameterDescriptionsAsHelpContent_ShouldAppendTwoParameterSectionsWithParameterName()
        {
            #region 1) Arrange

            string targetDescriptionText = "This is the target parameter";

            IMsBuildElementHelp help = MsBuildTestDataBuilder.Create<IMsBuildTarget>()
                .SetName("TargetA")
                .WithHelp()
                .AddHelpParagraph("PARAMETER", targetDescriptionText, "$(ParamPropertyOne)")
                .AddHelpParagraph("PARAMETER", targetDescriptionText, "$(ParamPropertyTwo)")
                .Build();


            Mock<IMsBuildTarget> targetMock = new Mock<IMsBuildTarget>();
            targetMock.Setup(t => t.Name).Returns("TargetA");
            targetMock.Setup(t => t.Help).Returns(help);

            Mock<IPrintableDocumentChapterStringContent> chapterContentMock =
                new Mock<IPrintableDocumentChapterStringContent>();
            chapterContentMock.SetupSet(cc => cc.Content = It.IsAny<string>()).Verifiable();

            Mock<IPrintableDocumentParagraph> paragraphMock = new Mock<IPrintableDocumentParagraph>();
            Mock<IPrintableDocumentChapter> chapterMock = new Mock<IPrintableDocumentChapter>();
            chapterMock.Setup(c => c.AddNewParagraph(It.IsAny<string>())).Returns(paragraphMock.Object);
            paragraphMock.Setup(p => p.AddNewContent<IPrintableDocumentChapterStringContent>())
                .Returns(chapterContentMock.Object);

            Mock<IMarkdownDocument> markdownDocumentMock = new Mock<IMarkdownDocument>();
            markdownDocumentMock.Setup(md => md.DefaultFileExtension).Returns("md");
            markdownDocumentMock.Setup(md => md.AddNewChapter(It.IsAny<string>())).Returns(chapterMock.Object);

            Mock<PrintableDocument<IMarkdownDocument>> printableDocumentMock =
                new Mock<PrintableDocument<IMarkdownDocument>>();

            printableDocumentMock.Setup(pd => pd.Create(It.IsAny<string>()))
                .Returns(markdownDocumentMock.Object);

            TargetOverviewSiteGenerator<IMarkdownDocument> markdownTargetDocumentGenerator =
                new TargetOverviewSiteGenerator<IMarkdownDocument>(printableDocumentMock.Object);

            #endregion

            // 2) Act
            IPrintableDocument document = markdownTargetDocumentGenerator.CreateOverview(targetMock.Object);

            // 3) Assert
            chapterMock.Verify(
                cm => cm.AddNewParagraph(It.Is<string>(s => s.Equals("Parameter $(ParamPropertyOne)"))),
                Times.Exactly(1));
            chapterMock.Verify(
                cm => cm.AddNewParagraph(It.Is<string>(s => s.Equals("Parameter $(ParamPropertyTwo)"))),
                Times.Exactly(1));
        }

        [TestMethod]
        public void
            CreateOverview_WithTargetContainingTwoParameterDescriptionsAsHelpContent_ShouldAppendTwoParameterSectionsWithBody()
        {
            #region 1) Arrange

            string targetDescriptionText = "This is the target parameter";

            IMsBuildElementHelp help = MsBuildTestDataBuilder.Create<IMsBuildTarget>()
                .SetName("TargetA")
                .WithHelp()
                .AddHelpParagraph("PARAMETER", targetDescriptionText, "$(ParamPropertyOne)")
                .AddHelpParagraph("PARAMETER", targetDescriptionText, "$(ParamPropertyTwo)")
                .Build();

            Mock<IMsBuildTarget> targetMock = new Mock<IMsBuildTarget>();
            targetMock.Setup(t => t.Name).Returns("TargetA");
            targetMock.Setup(t => t.Help).Returns(help);

            Mock<IPrintableDocumentChapterStringContent> chapterContentMock =
                new Mock<IPrintableDocumentChapterStringContent>();
            chapterContentMock.SetupSet(cc => cc.Content = It.IsAny<string>()).Verifiable();

            Mock<IPrintableDocumentParagraph> paragraphMock = new Mock<IPrintableDocumentParagraph>();
            Mock<IPrintableDocumentChapter> chapterMock = new Mock<IPrintableDocumentChapter>();
            chapterMock.Setup(c => c.AddNewParagraph(It.IsAny<string>())).Returns(paragraphMock.Object);
            paragraphMock.Setup(p => p.AddNewContent<IPrintableDocumentChapterStringContent>())
                .Returns(chapterContentMock.Object);

            Mock<IMarkdownDocument> markdownDocumentMock = new Mock<IMarkdownDocument>();
            markdownDocumentMock.Setup(md => md.DefaultFileExtension).Returns("md");
            markdownDocumentMock.Setup(md => md.AddNewChapter(It.IsAny<string>())).Returns(chapterMock.Object);

            Mock<PrintableDocument<IMarkdownDocument>> printableDocumentMock =
                new Mock<PrintableDocument<IMarkdownDocument>>();

            printableDocumentMock.Setup(pd => pd.Create(It.IsAny<string>()))
                .Returns(markdownDocumentMock.Object);

            TargetOverviewSiteGenerator<IMarkdownDocument> markdownTargetDocumentGenerator =
                new TargetOverviewSiteGenerator<IMarkdownDocument>(printableDocumentMock.Object);

            #endregion

            // 2) Act
            IPrintableDocument document = markdownTargetDocumentGenerator.CreateOverview(targetMock.Object);

            // 3) Assert
            paragraphMock.Verify(p => p.AddNewContent<IPrintableDocumentChapterStringContent>(), Times.Exactly(2));
        }


        [TestMethod]
        public void
            CreateOverview_WithTargetContainingTwoParameterDescriptionsWithSameContentAsHelpContent_ShouldAppendTwoParameterSectionsWithDescriptionInBody()
        {
            #region 1) Arrange

            string targetDescriptionText = "This is the target parameter";

            IMsBuildElementHelp help = MsBuildTestDataBuilder.Create<IMsBuildTarget>()
                .SetName("TargetA")
                .WithHelp()
                .AddHelpParagraph("PARAMETER", targetDescriptionText, "$(ParamPropertyOne)")
                .AddHelpParagraph("PARAMETER", targetDescriptionText, "$(ParamPropertyTwo)")
                .Build();

            Mock<IMsBuildTarget> targetMock = new Mock<IMsBuildTarget>();
            targetMock.Setup(t => t.Name).Returns("TargetA");
            targetMock.Setup(t => t.Help).Returns(help);

            Mock<IPrintableDocumentChapterStringContent> chapterContentMock =
                new Mock<IPrintableDocumentChapterStringContent>();
            chapterContentMock.SetupSet(cc => cc.Content = It.IsAny<string>()).Verifiable();

            Mock<IPrintableDocumentParagraph> paragraphMock = new Mock<IPrintableDocumentParagraph>();
            Mock<IPrintableDocumentChapter> chapterMock = new Mock<IPrintableDocumentChapter>();
            chapterMock.Setup(c => c.AddNewParagraph(It.IsAny<string>())).Returns(paragraphMock.Object);
            paragraphMock.Setup(p => p.AddNewContent<IPrintableDocumentChapterStringContent>())
                .Returns(chapterContentMock.Object);

            Mock<IMarkdownDocument> markdownDocumentMock = new Mock<IMarkdownDocument>();
            markdownDocumentMock.Setup(md => md.DefaultFileExtension).Returns("md");
            markdownDocumentMock.Setup(md => md.AddNewChapter(It.IsAny<string>())).Returns(chapterMock.Object);

            Mock<PrintableDocument<IMarkdownDocument>> printableDocumentMock =
                new Mock<PrintableDocument<IMarkdownDocument>>();

            printableDocumentMock.Setup(pd => pd.Create(It.IsAny<string>()))
                .Returns(markdownDocumentMock.Object);

            TargetOverviewSiteGenerator<IMarkdownDocument> markdownTargetDocumentGenerator =
                new TargetOverviewSiteGenerator<IMarkdownDocument>(printableDocumentMock.Object);

            #endregion

            // 2) Act
            IPrintableDocument document = markdownTargetDocumentGenerator.CreateOverview(targetMock.Object);

            // 3) Assert
            chapterContentMock.VerifySet(cc => cc.Content = It.Is<string>(s => s.Equals(targetDescriptionText)),
                Times.Exactly(2));
        }

        [TestMethod]
        public void
            CreateOverview_WithTargetContainingTwoParameterDescriptionsWithDifferingContentAsHelpContent_ShouldAppendTwoParameterSectionsWithCorrectDescriptionInEachBody()
        {
            #region 1) Arrange

            string targetParameterOneDescriptionText = "This is the target parameter for parameter one.";
            string targetParameterTwoDescriptionText = "This is the target parameter for parameter two.";

            IMsBuildElementHelp help = MsBuildTestDataBuilder.Create<IMsBuildTarget>()
                .SetName("TargetA")
                .WithHelp()
                .AddHelpParagraph("PARAMETER", targetParameterOneDescriptionText, "$(ParamPropertyOne)")
                .AddHelpParagraph("PARAMETER", targetParameterTwoDescriptionText, "$(ParamPropertyTwo)")
                .Build();


            Mock<IMsBuildTarget> targetMock = new Mock<IMsBuildTarget>();
            targetMock.Setup(t => t.Name).Returns("TargetA");
            targetMock.Setup(t => t.Help).Returns(help);

            Mock<IPrintableDocumentChapterStringContent> chapterContentMock =
                new Mock<IPrintableDocumentChapterStringContent>();
            chapterContentMock.SetupSet(cc => cc.Content = It.IsAny<string>()).Verifiable();

            Mock<IPrintableDocumentParagraph> paragraphMock = new Mock<IPrintableDocumentParagraph>();
            Mock<IPrintableDocumentChapter> chapterMock = new Mock<IPrintableDocumentChapter>();
            chapterMock.Setup(c => c.AddNewParagraph(It.IsAny<string>())).Returns(paragraphMock.Object);
            paragraphMock.Setup(p => p.AddNewContent<IPrintableDocumentChapterStringContent>())
                .Returns(chapterContentMock.Object);

            Mock<IMarkdownDocument> markdownDocumentMock = new Mock<IMarkdownDocument>();
            markdownDocumentMock.Setup(md => md.DefaultFileExtension).Returns("md");
            markdownDocumentMock.Setup(md => md.AddNewChapter(It.IsAny<string>())).Returns(chapterMock.Object);

            Mock<PrintableDocument<IMarkdownDocument>> printableDocumentMock =
                new Mock<PrintableDocument<IMarkdownDocument>>();

            printableDocumentMock.Setup(pd => pd.Create(It.IsAny<string>()))
                .Returns(markdownDocumentMock.Object);

            TargetOverviewSiteGenerator<IMarkdownDocument> markdownTargetDocumentGenerator =
                new TargetOverviewSiteGenerator<IMarkdownDocument>(printableDocumentMock.Object);

            #endregion

            // 2) Act
            IPrintableDocument document = markdownTargetDocumentGenerator.CreateOverview(targetMock.Object);

            // 3) Assert
            chapterContentMock.VerifySet(
                cc => cc.Content = It.Is<string>(s => s.Equals(targetParameterOneDescriptionText)), Times.Exactly(1));
            chapterContentMock.VerifySet(
                cc => cc.Content = It.Is<string>(s => s.Equals(targetParameterTwoDescriptionText)), Times.Exactly(1));
        }
    }


    public class MsBuildTestDataBuilder
    {
        public static IMsBuildElementBuilder<T> Create<T>() where T : class, IMsBuildElement
        {
            MsBuildElementBuilderImplementation<T> msBuildElementBuilderImplementation =
                new MsBuildElementBuilderImplementation<T>();

            return msBuildElementBuilderImplementation;
        }
    }

    public class MsBuildElementBuilderImplementation<T> : IMsBuildElementBuilder<T> where T : class, IMsBuildElement
    {
        private Mock<T> _msBuildElementMock = new Mock<T>();

        public IMsBuildElementHelpBuilder WithHelp()
        {
            MsBuildElementHelpBuilderImplementation helpBuilder = new MsBuildElementHelpBuilderImplementation();
            return helpBuilder;
        }

        public IMsBuildElementBuilder<T> SetName(string name)
        {
            if (typeof(T) == typeof(IMsBuildTarget))
            {
                Mock<IMsBuildTarget> msBuildTargetMock = _msBuildElementMock as Mock<IMsBuildTarget>;
                msBuildTargetMock?.Setup(t => t.Name).Returns(name);
            }

            return this;
        }

        public T Build()
        {
            return _msBuildElementMock.Object;
        }

        public Mock<T> BuildMock()
        {
            return _msBuildElementMock;
        }
    }

    public interface IMsBuildElementBuilder<T> where T : class, IMsBuildElement
    {
        IMsBuildElementHelpBuilder WithHelp();

        IMsBuildElementBuilder<T> SetName(string name);

        T Build();
    }

    public interface IMsBuildElementHelpBuilder
    {
        IMsBuildElementHelpBuilder AddHelpParagraph(string title, string content);

        IMsBuildElementHelpBuilder AddHelpParagraph(string title, string content, string additional);

        IMsBuildElementHelp Build();
    }

    public class MsBuildElementHelpBuilderImplementation : IMsBuildElementHelpBuilder
    {
        private readonly Mock<IMsBuildElementHelp> _msBuildElementHelpMock = new Mock<IMsBuildElementHelp>();

        private readonly IList<Mock<IMsBuildElementHelpParagraph>> _msBuildHelpParagraphMocks =
            new List<Mock<IMsBuildElementHelpParagraph>>();

        public MsBuildElementHelpBuilderImplementation()
        {
            _msBuildElementHelpMock.Setup(h => h.Count).Returns(_msBuildHelpParagraphMocks.Count);

            _msBuildElementHelpMock.Setup(h => h.IsReadOnly).Returns(_msBuildHelpParagraphMocks.IsReadOnly);

            _msBuildElementHelpMock.Setup(h => h.Remove(It.IsAny<string>(), It.IsAny<bool>()))
                .Callback((string s, bool b) =>
                {
                    _msBuildHelpParagraphMocks.Remove(
                        _msBuildHelpParagraphMocks.First(hp => hp.Object.Name.Equals(s)));
                });

            _msBuildElementHelpMock.Setup(h => h.ContainsSection(It.IsAny<string>(), It.IsAny<StringComparison>()))
                .Returns((string s, StringComparison sc) =>
                    _msBuildHelpParagraphMocks.Any(hp => hp.Object.Name.Equals(s, sc)));

            _msBuildElementHelpMock.Setup(h => h.LookUp(It.IsAny<string>(), It.IsAny<StringComparison>()))
                .Returns(
                    (string s, StringComparison sc) =>
                        _msBuildHelpParagraphMocks.Where(hp => hp.Object.Name.Equals(s, sc)).Select(hp => hp.Object)
                            .ToList());

            _msBuildElementHelpMock.Setup(h => h.LookUp(It.IsAny<string>()))
                .Returns(((string s) => _msBuildHelpParagraphMocks.Where(hp => hp.Object.Name.Equals(s))
                    .Select(hp => hp.Object).ToList()));

            _msBuildElementHelpMock.Setup(h => h.GetSectionContent(It.IsAny<string>(), It.IsAny<StringComparison>()))
                .Returns((string s, StringComparison sc) => _msBuildHelpParagraphMocks
                    .FirstOrDefault(hp => hp.Object.Name.Equals(s, sc))
                    ?.Object.Content);
        }

        public IMsBuildElementHelpBuilder AddHelpParagraph(string title, string content)
        {
            Mock<IMsBuildElementHelpParagraph> helpParagraphMock = new Mock<IMsBuildElementHelpParagraph>();
            helpParagraphMock.Setup(hp => hp.Content).Returns(content);
            helpParagraphMock.Setup(hp => hp.Name).Returns(title);
            _msBuildHelpParagraphMocks.Add(helpParagraphMock);
            return this;
        }

        public IMsBuildElementHelpBuilder AddHelpParagraph(string title, string content, string additional)
        {
            Mock<IMsBuildElementHelpParagraph> helpParagraphMock = new Mock<IMsBuildElementHelpParagraph>();
            helpParagraphMock.Setup(hp => hp.Content).Returns(content);
            helpParagraphMock.Setup(hp => hp.Name).Returns(title);
            helpParagraphMock.Setup(hp => hp.Additional).Returns(additional);
            _msBuildHelpParagraphMocks.Add(helpParagraphMock);
            return this;
        }

        public IMsBuildElementHelp Build()
        {
            return _msBuildElementHelpMock.Object;
        }

        public Mock<IMsBuildElementHelp> BuildMock()
        {
            return _msBuildElementHelpMock;
        }
    }
}