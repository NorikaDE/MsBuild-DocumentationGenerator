using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Norika.Documentation.Core;
using Norika.Documentation.Core.FileSystem;
using Norika.Documentation.Core.FileSystem.Interfaces;
using Norika.Documentation.Core.Types;
using Norika.MsBuild.Core.Data;
using Norika.MsBuild.Core.Data.Help;
using Norika.MsBuild.DocumentationGenerator.Business.IO.Interfaces;
using Norika.MsBuild.DocumentationGenerator.Business.Logging;
using Norika.MsBuild.Model.Interfaces;

namespace Norika.MsBuild.DocumentationGenerator.Business
{
    /// <summary>
    /// Generator for format and printable msbuild project documentation
    /// </summary>
    /// <typeparam name="T">Output type for the generated documentation. Must implement <see cref="IPrintableDocument">IPrintableDocument</see>.</typeparam>
    public class ProjectOverviewGenerator<T> where T : IPrintableDocument
    {
        /// <summary>
        /// MsBuild project represented by the documentation generated by this class
        /// </summary>
        private readonly IMsBuildProject _msBuildProject;

        /// <summary>
        /// Target output document the generated documentation is written to
        /// </summary>
        private readonly IPrintableDocument _outputDocument;

        /// <summary>
        /// Builder for the target output document
        /// </summary>
        private readonly PrintableDocument<T> _printableDocumentBuilder;

        /// <summary>
        /// <inheritdoc cref="_outputDocument"/>
        /// </summary>
        internal IPrintableDocument OutputDocument => _outputDocument;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="project">MsBuild-Project file to generate the documentation for</param>
        /// <param name="file">File object representing the msbuild project file</param>
        public ProjectOverviewGenerator(IMsBuildProject project, IFile file)
        {
            _msBuildProject = project;

            _printableDocumentBuilder = new PrintableDocument<T>(new FileWriter());

            _outputDocument = CreateOutputDocument(file, _printableDocumentBuilder);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="project">MsBuild-Project file to generate the documentation for</param>
        /// <param name="file">File object representing the msbuild project file</param>
        /// <param name="printableDocument">Object representing output documentation file</param>
        public ProjectOverviewGenerator(IMsBuildProject project, IFile file, PrintableDocument<T> printableDocument)
        {
            _msBuildProject = project;

            _printableDocumentBuilder = printableDocument;

            _outputDocument = CreateOutputDocument(file, _printableDocumentBuilder);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="file">File object representing the msbuild project file</param>
        public ProjectOverviewGenerator(IFile file)
        {
            var formattableDocumentBuilder = new PrintableDocument<T>();
            _msBuildProject = LoadMsBuildProject(file);
            _outputDocument = CreateOutputDocument(file, formattableDocumentBuilder);

            _printableDocumentBuilder = formattableDocumentBuilder;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="file">File object representing the msbuild project file</param>
        /// <param name="documentBuilder">Factory for creating the output documentation file</param>
        public ProjectOverviewGenerator(IFile file, IFormattableDocumentBuilder documentBuilder)
        {
            _printableDocumentBuilder = new PrintableDocument<T>(documentBuilder);

            _msBuildProject = LoadMsBuildProject(file);
            _outputDocument = CreateOutputDocument(file, _printableDocumentBuilder);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="file">File object representing the msbuild project file</param>
        /// <param name="fileWriter">Writer used for creating the output documentation</param>
        /// <param name="documentBuilder">Factory for creating the output documentation file</param>
        public ProjectOverviewGenerator(IFile file, IFileWriter fileWriter, IFormattableDocumentBuilder documentBuilder)
        {
            _printableDocumentBuilder = new PrintableDocument<T>(documentBuilder, fileWriter);
            _msBuildProject = LoadMsBuildProject(file);
            _outputDocument = CreateOutputDocument(file, _printableDocumentBuilder);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="file">File object representing the msbuild project file</param>
        /// <param name="printableDocument">Object representing output documentation file</param>
        public ProjectOverviewGenerator(IFile file, PrintableDocument<T> printableDocument)
        {
            _printableDocumentBuilder = printableDocument;
            _msBuildProject = LoadMsBuildProject(file);
            _outputDocument = CreateOutputDocument(file, _printableDocumentBuilder);
        }

        /// <summary>
        /// Creates the documentations body content
        /// </summary>
        /// <returns>True if the documentation has been created successfully</returns>
        public bool CreateBody()
        {
            try
            {
                var properties = _msBuildProject.GetChildren<IMsBuildProperty>();
                var targets = _msBuildProject.GetChildren<IMsBuildTarget>();

                AppendGlobalPropertySection(properties);
                AppendPropertySection(properties);
                AppendTargetSection(targets);
                //CreateTargetOverviewFiles(targets);
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteError(ex);
                return false;
            }
        }

        /// <summary>
        /// <inheritdoc cref="IPrintableDocument.Print"/>
        /// </summary>
        /// <returns></returns>
        public string Print()
        {
            return _outputDocument.Print();
        }

        /// <summary>
        /// Loads the content as MSBuild project from the given file
        /// </summary>
        /// <param name="file">File from which the content should be loaded</param>
        /// <returns>MSBuild project representation</returns>
        private IMsBuildProject LoadMsBuildProject(IFile file)
        {
            return MsBuildProjectFile.LoadContent(file.ReadAllText());
        }

        /// <summary>
        /// Creates the output file from the document builder for the given generic type. 
        /// </summary>
        /// <param name="file">The file for which the output documentation file should be created</param>
        /// <param name="printableDocument">The document builder which is used for creating the output file</param>
        /// <returns>Instance of the requested generic type</returns>
        private IPrintableDocument CreateOutputDocument(IFile file, PrintableDocument<T> printableDocument)
        {
            return printableDocument.Create(file.FileName);
        }

        /// <summary>
        /// Appends a section to the documentation body containing all information about
        /// the given targets. Therefore a table is created containing a row per target. 
        /// </summary>
        /// <param name="targets">List of targets that should be represented by the section.</param>
        private void AppendTargetSection(IList<IMsBuildTarget> targets)
        {
            if (targets.Count == 0) return;

            IPrintableDocumentChapter targetsChapter = _outputDocument.AddNewChapter("Targets");

            IPrintableDocumentChapterStringContent paragraph =
                targetsChapter.AddNewContent<IPrintableDocumentChapterStringContent>();

            paragraph.Content =
                $"Contains all msbuild targets in {_outputDocument.Title}, ordered by name ascending.";

            var propertyTable = targetsChapter.AddNewContent<IPrintableParagraphTable>();
            propertyTable.WithHeaders("Target", "Description");


            foreach (var target in targets.OrderBy(t => t.Name))
            {
                propertyTable.WithRow(CreateHyperlinkForTargetOverviewFile(target).Print(),
                    string.Join(" ", GetPropertySynopsisOrDescription(target)?.Content));
            }
        }

        /// <summary>
        /// Creates a hyperlink to a dedicated overview file representing the target target located
        /// in a sub directory of the destination documentation directory.
        /// </summary>
        /// <param name="target">Target for which the hyperlink should be created</param>
        /// <returns>ParagraphHyperlink linking to the dedicated target documentation site</returns>
        private IPrintableDocumentParagraphHyperlink CreateHyperlinkForTargetOverviewFile(IMsBuildTarget target)
        {
            IPrintableDocumentParagraphHyperlink paragraphHyperLink =
                _outputDocument.CreateElement<IPrintableDocumentParagraphHyperlink>();

            paragraphHyperLink.DisplayString = target.Name;
            paragraphHyperLink.Hyperlink = Path.Combine(".", "Targets",
                string.Format(CultureInfo.InvariantCulture, "{0}.{1}", target.Name,
                    _outputDocument.DefaultFileExtension));

            paragraphHyperLink.ToolTip =
                string.Format(CultureInfo.CurrentCulture, "See more about the target {0}.", target.Name);

            return paragraphHyperLink;
        }

        /// <summary>
        /// Appends a section to the documentation body containing all information about
        /// the given properties. Only properties are considered that are not settable from
        /// outside the MSBuild target file. 
        /// </summary>
        /// <param name="properties">List of properties that should be represented by the section</param>
        private void AppendPropertySection(IList<IMsBuildProperty> properties)
        {
            if (properties == null || properties.All(p => p.HasPublicSetter)) return;

            var propertyChapter = _outputDocument.AddNewChapter("Properties");

            IPrintableDocumentChapterStringContent paragraph =
                propertyChapter.AddNewContent<IPrintableDocumentChapterStringContent>();

            paragraph.Content =
                $"Contains all not settable properties in {_outputDocument.Title}.";

            var propertyTable = propertyChapter.AddNewContent<IPrintableParagraphTable>();
            propertyTable.WithHeaders("Property", "Description", "Condition");

            foreach (var property in properties.Where(p => !p.HasPublicSetter))
                propertyTable.WithRow(property.Name,
                    string.Join(" ", GetPropertySynopsisOrDescription(property)?.Content),
                    property.Condition);
        }

        /// <summary>
        /// Appends a section to the documentation body containing all information about
        /// the given properties. Only properties are considered that are settable from
        /// outside the MSBuild target file. 
        /// </summary>
        /// <param name="properties">List of properties that should be represented by the section</param>
        private void AppendGlobalPropertySection(IList<IMsBuildProperty> properties)
        {
            if (properties == null || !properties.Any(p => p.HasPublicSetter)) return;

            var propertyChapter = _outputDocument.AddNewChapter("Overwriteable Properties");

            IPrintableDocumentChapterStringContent paragraph =
                propertyChapter.AddNewContent<IPrintableDocumentChapterStringContent>();
            paragraph.Content =
                $"Contains all properties in {_outputDocument.Title} that could be overwritten from outside the document.";

            var propertyTable = propertyChapter.AddNewContent<IPrintableParagraphTable>();

            propertyTable.WithHeaders("Property", "Description", "Condition");

            foreach (var property in properties.Where(p => p.HasPublicSetter))
                propertyTable.WithRow(property.Name,
                    string.Join(Environment.NewLine, GetPropertySynopsisOrDescription(property)?.Content),
                    property.Condition);
        }

        /// <summary>
        /// Returns synopsis of the given MsBuild element. When no synopsis is defined it returns the
        /// description, shortened to the first description paragraph.
        /// </summary>
        /// <param name="node">MsBuildNode to get the synopsis or description for.</param>
        /// <returns>HelpParagraph object representing the synopsis or description of the MsBuild Node</returns>
        /// Todo: Implementing the "shortening" thing addressed by the summary
        private static IMsBuildElementHelpParagraph GetPropertySynopsisOrDescription(IMsBuildElement node)
        {
            if (node.Help.Count == 0) return null;

            if (node.Help.ContainsSection(MsBuildHelpSections.Synopsis, StringComparison.OrdinalIgnoreCase))
            {
                return node.Help.LookUp(MsBuildHelpSections.Synopsis,
                    StringComparison.OrdinalIgnoreCase).FirstOrDefault();
            }

            if (node.Help.ContainsSection(MsBuildHelpSections.Description, StringComparison.OrdinalIgnoreCase))
            {
                return node.Help.LookUp(MsBuildHelpSections.Description,
                    StringComparison.OrdinalIgnoreCase).FirstOrDefault();
            }

            return default;
        }
    }
}