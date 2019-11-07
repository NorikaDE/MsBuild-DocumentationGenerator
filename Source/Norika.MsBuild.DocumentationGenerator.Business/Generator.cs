using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Norika.Documentation.Core;
using Norika.Documentation.Core.Types;
using Norika.MsBuild.Core.Data;
using Norika.MsBuild.DocumentationGenerator.Business.IO;
using Norika.MsBuild.DocumentationGenerator.Business.IO.Interfaces;
using Norika.MsBuild.Model.Interfaces;

namespace Norika.MsBuild.DocumentationGenerator.Business
{
    /// <summary>
    /// Documentation generator for MsBuild files
    /// </summary>
    /// <typeparam name="T">Output documentation type.</typeparam>
    public class Generator<T> where T : IPrintableDocument
    {
        /// <summary>
        /// Path of the file for which the documentation should be created. 
        /// </summary>
        private readonly string _filePath;

        /// <summary>
        /// Builder for the output document. 
        /// </summary>
        private readonly PrintableDocument<T> _printableDocument;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filePath">Path of the file for which the documentation should be created</param>
        public Generator(string filePath)
        {
            _filePath = filePath;
            _printableDocument = new PrintableDocument<T>();
        }

        /// <summary>
        /// Creates the documentation by appending all required sections and prints the file to
        /// the given output directory. 
        /// </summary>
        /// <param name="outputPath">Directory in which the documentation should be crated</param>
        public void CreateDocumentation(string outputPath)
        {
            PrepareOutputDirectory(outputPath);

            IFile file = FileSystem.GetFileInformation(_filePath);
            IMsBuildProject msBuildProject = MsBuildProjectFile.LoadContent(file.ReadAllText());

            CreateProjectOverview(msBuildProject, file, outputPath);
            CreateTargetDocumentation(outputPath, msBuildProject.GetChildren<IMsBuildTarget>());
        }


        /// <summary>
        /// Creates the documentation overview for the given MsBuild project and stores it
        /// into the given output directory. 
        /// </summary>
        /// <param name="msBuildProject">The msbuild project to create the documentation for.</param>
        /// <param name="file">The file system entry representing the msbuild project</param>
        /// <param name="outputPath">Directory to store the documentation in</param>
        /// <exception cref="DocumentationGeneratorException">Throws if the body of the documentation could nt be created</exception>
        private void CreateProjectOverview(IMsBuildProject msBuildProject, IFile file,
            string outputPath)
        {
            ProjectOverviewGenerator<T> projectOverviewDocumentationGenerator =
                new ProjectOverviewGenerator<T>(msBuildProject, file, _printableDocument);

            if (!projectOverviewDocumentationGenerator.CreateBody())
                throw new DocumentationGeneratorException("Body for project overview could not be created!");

            string outputFilePath = Path.Combine(
                outputPath, string.Format(
                    CultureInfo.InvariantCulture, "{0}.{1}", file.FileName,
                    projectOverviewDocumentationGenerator.OutputDocument.DefaultFileExtension
                )
            );

            if (!_printableDocument.Save(outputFilePath, projectOverviewDocumentationGenerator.OutputDocument))
                throw new IOException("The documentation could not be saved.");
        }


        /// <summary>
        /// Creates the documentation for each of the given targets. 
        /// </summary>
        /// <param name="outputPath">Directory in which the documentation is stored in</param>
        /// <param name="targets">List of targets to create the documentation for</param>
        private void CreateTargetDocumentation(string outputPath, IEnumerable<IMsBuildTarget> targets)
        {
            TargetOverviewSiteGenerator<T> targetOverviewDocumentationGenerator =
                new TargetOverviewSiteGenerator<T>(_printableDocument);

            string targetOutputPath = Path.Combine(outputPath, "Targets");

            PrepareOutputDirectory(targetOutputPath);

            Parallel.ForEach(targets, target =>
            {
                IPrintableDocument targetDocument = targetOverviewDocumentationGenerator.CreateOverview(target);

                SaveTargetDocumentation(targetOutputPath, target.Name, targetDocument);
            });
        }


        /// <summary>
        /// Saves the created target documentation to the given output directory by creating
        /// a sub directory named like the target.
        /// </summary>
        /// <param name="targetOutputPath">Directory in which the documentation should be stored in</param>
        /// <param name="targetName">The name of the target</param>
        /// <param name="targetDocument">The target documentation document</param>
        /// <exception cref="IOException">The documentation could not be saved</exception>
        private void SaveTargetDocumentation(string targetOutputPath,
            string targetName, IPrintableDocument targetDocument)
        {
            if (!_printableDocument.Save(
                Path.Combine(targetOutputPath, string.Format(CultureInfo.InvariantCulture, "{0}.{1}",
                    targetName, targetDocument.DefaultFileExtension)),
                targetDocument))
            {
                throw new IOException("File could not be saved!");
            }
        }


        /// <summary>
        /// Prepares the output directory for the documentation.
        /// </summary>
        /// <param name="outputPath">Directory to prepare</param>
        /// Todo: Use IFileSystem for encapsulating dependencies
        private static void PrepareOutputDirectory(string outputPath)
        {
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);
            else
            {
                Directory.Delete(outputPath, true);
                Directory.CreateDirectory(outputPath);
            }
        }
    }
}