using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using LibGit2Sharp;
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

        public Kodify.AutoDoc.Models.ProjectInfo Analyze()
        {
            var rootPath = DetectProjectRoot();
            return Analyze(rootPath);
        }

        public Kodify.AutoDoc.Models.ProjectInfo Analyze(string path)
        {
            var projectInfo = new Kodify.AutoDoc.Models.ProjectInfo
            {
                ProjectPath = path,
                HasWebApi = false,
                License = DetectLicense(path),
                Structure = AnalyzeProjectStructure(path)
            };

            // Add Git repository detection
            var (hasGit, repoUrl) = CheckForGitRepository(path);
            projectInfo.HasGitRepository = hasGit;
            projectInfo.RepositoryUrl = repoUrl;

            var allFiles = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories)
        .Where(f => !IsAutogeneratedFile(f))
        .ToList();

            foreach (var file in allFiles)
            {
                var codeFile = AnalyzeCSharpFile(file);
                projectInfo.SourceFiles.Add(codeFile);

                // Strict API controller check
                foreach (var cls in codeFile.Classes)
                {
                    if (IsApiController(cls))
                    {
                        projectInfo.HasWebApi = true;
                        Console.WriteLine($"[DEBUG] API Controller Found: {cls.Name}");
                    }
                }
            }

            projectInfo.License = DetectLicense(path);
            return projectInfo;
        }
        private bool IsAutogeneratedFile(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            return fileName.StartsWith(".") ||
                   fileName.Contains("AssemblyInfo") ||
                   fileName.Contains("GlobalUsings");
        }
        private bool IsApiController(ClassInfo cls)
        {
            // Match only classes ending with "Controller"
            if (!cls.Name.EndsWith("Controller")) return false;

            // Check for MVC base types
            var validBaseTypes = new[] { "ControllerBase", "ApiController" };
            var hasValidBaseType = cls.BaseTypes
                .Any(bt => validBaseTypes.Any(vbt => bt.EndsWith(vbt)));

            // Check for [ApiController] attribute
            var hasApiAttribute = cls.Attributes
                .Any(a => a.EndsWith("ApiController"));

            return hasValidBaseType || hasApiAttribute;
        }
        private bool DetectApiControllers(List<CodeFile> sourceFiles)
        {
            return sourceFiles.Any(f =>
                f.Classes.Any(c => c.IsApiController)
            );
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
                    BaseTypes = classDecl.BaseList?.Types
                        .Select(t => t.ToString())
                        .ToList() ?? new List<string>(),
                    Attributes = classDecl.AttributeLists
                        .SelectMany(al => al.Attributes)
                        .Select(a => a.Name.ToString())
                        .ToList(),
                    Summary = GetSummary(classDecl)
                };

                // Add method analysis
                classInfo.Methods = classDecl.DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .Select(m => new MethodInfo
                    {
                        Name = m.Identifier.Text,
                        Attributes = m.AttributeLists
                            .SelectMany(al => al.Attributes)
                            .Select(a => a.Name.ToString())
                            .ToList(),
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

        private (bool hasGit, string repoUrl) CheckForGitRepository(string projectPath)
        {
            try
            {
                var repoPath = Repository.Discover(projectPath);
                using var repo = new Repository(repoPath);
                return (true, repo.Network.Remotes.FirstOrDefault()?.Url ?? "No remote repository configured");
            }
            catch
            {
                return (false, "No Git repository found");
            }
        }

        private string DetectProjectRoot()
        {
            var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
            DirectoryInfo trueRoot = null;

            // First check for Git repository root
            try
            {
                var repoPath = Repository.Discover(directory.FullName);
                if (!string.IsNullOrEmpty(repoPath))
                {
                    using var repo = new Repository(repoPath);
                    trueRoot = new DirectoryInfo(repo.Info.WorkingDirectory);
                }
            }
            catch { }

            // If Git root not found, look for solution/project files
            if (trueRoot == null)
            {
                var currentDir = directory;
                while (currentDir != null)
                {
                    if (currentDir.GetFiles().Any(f => f.Extension == ".sln" || f.Extension == ".csproj"))
                    {
                        trueRoot = currentDir;
                        break;
                    }
                    currentDir = currentDir.Parent;
                }
            }

            return trueRoot?.FullName ?? 
                throw new DirectoryNotFoundException("Project root not found");
        }
    }
}