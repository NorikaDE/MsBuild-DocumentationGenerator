using System;
using System.Globalization;
using System.Text;
using Norika.Documentation.Core;
using Norika.Documentation.Core.Types;
using Norika.MsBuild.Core.Data.Help;
using Norika.MsBuild.Model.Interfaces;

namespace Norika.MsBuild.DocumentationGenerator.Business
{
    /// <summary>
    /// Generator for format and printable MsBuild targets implementation
    /// </summary>
    /// <typeparam name="T">Output type for the generated documentation. Must implement <see cref="IPrintableDocument">IPrintableDocument</see>.</typeparam>
    /// ToDo: Check if the class have to be generic
    public class TargetOverviewSiteGenerator<T> where T : IPrintableDocument
    {
        /// <summary>
        /// Builder for the target output document
        /// </summary>
        private readonly PrintableDocument<T> _printableDocument;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="printableDocument">Builder for the target output document</param>
        public TargetOverviewSiteGenerator(PrintableDocument<T> printableDocument)
        {
            _printableDocument = printableDocument;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TargetOverviewSiteGenerator()
        {
            _printableDocument = new PrintableDocument<T>();
        }


        /// <summary>
        /// Creates the documentation body content for the given MsBuild target
        /// </summary>
        /// <param name="target">The target for which the documentation should be generated.</param>
        /// <returns>Document representing the generated documentation</returns>
        public IPrintableDocument CreateOverview(IMsBuildTarget target)
        {
            IPrintableDocument targetOverviewDocument = _printableDocument.Create(target.Name);

            AppendDescriptionSection(targetOverviewDocument, target);
            AppendParameterSection(targetOverviewDocument, target);
            AppendOutputSection(targetOverviewDocument, target);
            AppendDependencySection(targetOverviewDocument, target);
            AppendErrorHandlingSection(targetOverviewDocument, target);
            AppendExampleSection(targetOverviewDocument, target);

            return targetOverviewDocument;
        }


        /// <summary>
        /// Append a section to the documentation containing the targets description by using
        /// the xml based help. Therefore the help section "Description" is used.  
        /// </summary>
        /// <param name="targetOverviewDocument">Document the section should be appended to.</param>
        /// <param name="target">The target for which the section should be created.</param>
        private void AppendDescriptionSection(IPrintableDocument targetOverviewDocument, IMsBuildTarget target)
        {
            if (!target.Help.ContainsSection(MsBuildHelpSections.Description, StringComparison.OrdinalIgnoreCase)
            ) return;

            string descriptionContent = target.Help.GetSectionContent(
                MsBuildHelpSections.Description, StringComparison.OrdinalIgnoreCase
            );

            IPrintableDocumentChapter descriptionChapter = targetOverviewDocument.AddNewChapter("Description");

            IPrintableDocumentChapterStringContent descriptionChapterContent =
                descriptionChapter.AddNewContent<IPrintableDocumentChapterStringContent>();

            descriptionChapterContent.Content = string.Join(Environment.NewLine, descriptionContent);
        }


        /// <summary>
        /// Append a section to the documentation containing the targets parameters by using the
        /// xml based help. Therefore the help section "Parameter" is used.
        /// </summary>
        /// <param name="targetOverviewDocument">Document the section should be appended to.</param>
        /// <param name="target">The target for which the section should be created.</param>
        private void AppendParameterSection(IPrintableDocument targetOverviewDocument, IMsBuildTarget target)
        {
            if (!target.Help.ContainsSection(MsBuildHelpSections.Parameter,
                StringComparison.OrdinalIgnoreCase)) return;

            IPrintableDocumentChapter parameterChapter = targetOverviewDocument.AddNewChapter("Parameters");

            foreach (IMsBuildElementHelpParagraph parameterHelpParagraph in
                target.Help.LookUp(MsBuildHelpSections.Parameter, StringComparison.OrdinalIgnoreCase))
            {
                IPrintableDocumentParagraph parameterParagraph =
                    parameterChapter.AddNewParagraph(
                        string.Format(CultureInfo.InvariantCulture, "Parameter {0}",
                            parameterHelpParagraph.Additional)
                    );

                IPrintableDocumentChapterStringContent parameterSectionBody =
                    parameterParagraph.AddNewContent<IPrintableDocumentChapterStringContent>();

                parameterSectionBody.Content = parameterHelpParagraph.Content;
            }
        }


        /// <summary>
        /// Append a section to the documentation containing the targets outputs by using the
        /// xml based help. Therefore the help section "Outputs" is used.
        /// </summary>
        /// <param name="targetOverviewDocument">Document the section should be appended to.</param>
        /// <param name="target">The target for which the section should be created.</param>
        private void AppendOutputSection(IPrintableDocument targetOverviewDocument, IMsBuildTarget target)
        {
            if (!target.Help.ContainsSection(MsBuildHelpSections.Outputs,
                StringComparison.OrdinalIgnoreCase)) return;

            foreach (IMsBuildElementHelpParagraph outputsHelpParagraph in
                target.Help.LookUp(MsBuildHelpSections.Outputs, StringComparison.OrdinalIgnoreCase))
            {
                IPrintableDocumentChapter parameterChapter =
                    targetOverviewDocument.AddNewChapter(
                        string.Format(CultureInfo.InvariantCulture, "Outputs {0}",
                            outputsHelpParagraph.Additional)
                    );

                IPrintableDocumentChapterStringContent outputsSectionBody =
                    parameterChapter.AddNewContent<IPrintableDocumentChapterStringContent>();

                outputsSectionBody.Content = outputsHelpParagraph.Content;
            }
        }


        /// <summary>
        /// Append a section to the documentation containing the targets dependencies by using the
        /// MsBuild target attributes. Therefore all dependencies like "AfterTargets", "BeforeTargets"
        /// and "DependsOnTargets" are appended as a paragraph table. 
        /// </summary>
        /// <param name="targetOverviewDocument">Document the section should be appended to.</param>
        /// <param name="target">The target for which the section should be created.</param>
        private void AppendDependencySection(IPrintableDocument targetOverviewDocument,
            IMsBuildTarget target)
        {
            if (target.HasTargetDependencies == false) return;

            IPrintableDocumentChapter dependencyChapter =
                targetOverviewDocument.AddNewChapter("Target Dependencies");

            IPrintableParagraphTable dependencyTable =
                dependencyChapter.AddNewContent<IPrintableParagraphTable>()
                    .WithHeaders("Target", "Dependency Type", "Dependency Description");

            foreach (var dependentOnTarget in target.DependsOnTargets)
            {
                dependencyTable.WithRow(dependentOnTarget, "DependsOnTargets",
                    string.Format(CultureInfo.InvariantCulture,
                        "Calls the target {0} before execution of {1}.", dependentOnTarget, target.Name));
            }

            foreach (var afterTarget in target.AfterTargets)
            {
                dependencyTable.WithRow(afterTarget, "AfterTargets",
                    string.Format(CultureInfo.InvariantCulture,
                        "Runs the target {0} after the execution of {1} has finished.", target.Name, afterTarget));
            }

            foreach (var beforeTargets in target.BeforeTargets)
            {
                dependencyTable.WithRow(beforeTargets, "BeforeTargets",
                    string.Format(CultureInfo.InvariantCulture,
                        "Runs the target {0} before the execution of {1} starts.", target.Name, beforeTargets));
            }
        }

        /// <summary>
        /// Append a section to the documentation containing the targets examples by using the
        /// xml based help. Therefore all examples are appended a code block.
        /// </summary>
        /// <param name="targetOverviewDocument">Document the section should be appended to.</param>
        /// <param name="target">The target for which the section should be created.</param>
        private void AppendExampleSection(IPrintableDocument targetOverviewDocument,
            IMsBuildTarget target)
        {
            if (target.Help.ContainsSection(MsBuildHelpSections.Example, StringComparison.OrdinalIgnoreCase) == false)
                return;

            foreach (IMsBuildElementHelpParagraph exampleHelpParagraph in target.Help.LookUp(
                MsBuildHelpSections.Example, StringComparison.OrdinalIgnoreCase))
            {
                IPrintableDocumentChapter exampleChapter =
                    targetOverviewDocument.AddNewChapter(MsBuildHelpSections.Example);

                IPrintableDocumentCodeBlock exampleCodeBlock =
                    exampleChapter.AddNewContent<IPrintableDocumentCodeBlock>();

                var codeBlock = MsBuildElementHelpCodeBlockUtility.Parse(exampleHelpParagraph.Content);

                exampleCodeBlock.AppendContentLine(codeBlock.Content);
                exampleCodeBlock.SetLanguage(codeBlock.Language.ToString());
            }
        }


        /// <summary>
        /// Append a section to the documentation containing the targets error handling by using the
        /// MsBuild Task "OnError". The task is read from the target content if exists.  
        /// </summary>
        /// <param name="targetOverviewDocument">Document the section should be appended to.</param>
        /// <param name="target">The target for which the section should be created.</param>
        private void AppendErrorHandlingSection(IPrintableDocument targetOverviewDocument,
            IMsBuildTarget target)
        {
            if (target.OnErrorTargets.Count == 0) return;

            IPrintableDocumentChapter errorHandlingChapter =
                targetOverviewDocument.AddNewChapter("Error Handling");

            IPrintableDocumentChapterStringContent chapterContent =
                errorHandlingChapter.AddNewContent<IPrintableDocumentChapterStringContent>();

            StringBuilder builder = new StringBuilder();

            builder.AppendLine(
                string.Format(CultureInfo.InvariantCulture,
                    "This chapter contains an description of the error handling of this target. All listed targets are executed after an error occured in the target {0}.",
                    target.Name)
            );
            builder.AppendLine();
            foreach (var onErrorTarget in target.OnErrorTargets)
            {
                builder.AppendLine(string.Format(CultureInfo.InvariantCulture, "  - {0}", onErrorTarget));
            }

            chapterContent.Content = builder.ToString();
        }
    }
}