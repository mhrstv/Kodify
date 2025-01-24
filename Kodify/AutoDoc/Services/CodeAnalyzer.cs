using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.IO;
using Kodify.AutoDoc.Models;

namespace Kodify.AutoDoc.Services
{
    public class CodeAnalyzer
    {
        private readonly ClassDiagramGenerator _classDiagramGenerator;

        public CodeAnalyzer()
        {
            _classDiagramGenerator = new ClassDiagramGenerator();
        }

        public void GenerateClassDiagrams(string outputPath)
        {
            _classDiagramGenerator.GenerateClassDiagrams(outputPath);
        }

        public Kodify.AutoDoc.Models.ProjectInfo Analyze(string path)
        {
            var projectInfo = new Kodify.AutoDoc.Models.ProjectInfo
            {
                ProjectPath = path,
                Structure = AnalyzeProjectStructure(path)
            };

            // Analyze all files
            var allFiles = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

            foreach (var file in allFiles)
            {
                if (file.EndsWith(".cs"))
                {
                    projectInfo.SourceFiles.Add(AnalyzeCSharpFile(file));
                }
                else
                {
                    projectInfo.OtherFiles.Add(new NonCodeFile
                    {
                        FilePath = file,
                        FileType = Path.GetExtension(file)
                    });
                }
            }

            projectInfo.HasWebApi = DetectApiControllers(projectInfo.SourceFiles);
            projectInfo.License = DetectLicense(path);

            return projectInfo;
        }
        private bool DetectApiControllers(List<CodeFile> sourceFiles)
        {
            foreach (var file in sourceFiles)
            {
                var code = File.ReadAllText(file.FilePath);
                var tree = CSharpSyntaxTree.ParseText(code);
                var root = tree.GetRoot();

                var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

                foreach (var classDecl in classDeclarations)
                {
                    // Check base types
                    var baseTypes = classDecl.BaseList?.Types
                        .Select(t => t.ToString())
                        .ToList() ?? new List<string>();

                    if (baseTypes.Any(t => t == "ControllerBase" || t == "ApiController"))
                        return true;

                    // Check for [ApiController] attribute
                    var attributes = classDecl.AttributeLists
                        .SelectMany(al => al.Attributes)
                        .Select(a => a.Name.ToString())
                        .ToList();

                    if (attributes.Any(a => a == "ApiController"))
                        return true;
                }
            }

            return false;
        }
        private CodeFile AnalyzeCSharpFile(string filePath)
        {
            var code = File.ReadAllText(filePath);
            var tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetRoot();

            var codeFile = new CodeFile { FilePath = filePath };

            var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
            foreach (var classDecl in classDeclarations)
            {
                var classInfo = new ClassInfo
                {
                    Name = classDecl.Identifier.Text,
                    BaseTypes = classDecl.BaseList?.Types.Select(t => t.ToString()).ToList() ?? new(),
                    Attributes = classDecl.AttributeLists
                        .SelectMany(al => al.Attributes)
                        .Select(a => a.Name.ToString())
                        .ToList(),
                    Summary = GetSummary(classDecl)
                };

                // Analyze methods
                classInfo.Methods = classDecl.DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .Select(m => new MethodInfo
                    {
                        Name = m.Identifier.Text,
                        Attributes = m.AttributeLists
                            .SelectMany(al => al.Attributes)
                            .Select(a => a.Name.ToString())
                            .ToList(),
                        // ... other method properties
                    }).ToList();

                codeFile.Classes.Add(classInfo);
            }

            return codeFile;
        }

        private string GetSummary(SyntaxNode node)
        {
            return node.GetLeadingTrivia()
                .Where(t => t.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) ||
                            t.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia))
                .Select(t => t.ToString())
                .FirstOrDefault()?
                .Replace("///", "")
                .Trim() ?? "No documentation available";
        }

        private ProjectStructure AnalyzeProjectStructure(string path)
        {
            var structure = new ProjectStructure();

            foreach (var dir in Directory.GetDirectories(path, "*", SearchOption.AllDirectories))
            {
                structure.Directories.Add(dir);
            }

            structure.FileTypes = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                .Select(f => Path.GetExtension(f))
                .Distinct()
                .ToList();

            return structure;
        }

        private LicenseInfo DetectLicense(string projectPath)
        {
            var licenseFiles = new[]
            {
        "LICENSE",
        "LICENSE.md",
        "LICENSE.txt",
        "COPYING",
        "COPYING.md",
        "COPYING.txt"
    };

            foreach (var file in licenseFiles)
            {
                var fullPath = Path.Combine(projectPath, file);
                if (File.Exists(fullPath))
                {
                    return new LicenseInfo
                    {
                        FilePath = fullPath,
                        Content = File.ReadAllText(fullPath),
                        Type = DetectLicenseType(fullPath)
                    };
                }
            }

            return new LicenseInfo { Type = "None" };
        }

        private string DetectLicenseType(string filePath)
        {
            var content = File.ReadAllText(filePath);

            if (content.Contains("MIT License")) return "MIT";
            if (content.Contains("Apache License")) return "Apache-2.0";
            if (content.Contains("GNU GENERAL PUBLIC LICENSE")) return "GPL-3.0";
            if (content.Contains("Mozilla Public License")) return "MPL-2.0";
            if (content.Contains("The Unlicense")) return "Unlicense";

            return "Custom";
        }
    }
}