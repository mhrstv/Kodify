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

            return projectInfo;
        }

        private CodeFile AnalyzeCSharpFile(string filePath)
        {
            var code = File.ReadAllText(filePath);
            var tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetRoot();

            var codeFile = new CodeFile { FilePath = filePath };

            // Analyze classes
            var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
            foreach (var classDecl in classDeclarations)
            {
                var classInfo = new ClassInfo
                {
                    Name = classDecl.Identifier.Text,
                    Summary = GetSummary(classDecl)
                };

                // Methods
                classInfo.Methods = classDecl.DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .Select(m => new MethodInfo
                    {
                        Name = m.Identifier.Text,
                        ReturnType = m.ReturnType.ToString(),
                        Parameters = m.ParameterList.Parameters.Select(p => new ParameterInfo
                        {
                            Name = p.Identifier.Text,
                            Type = p.Type?.ToString() ?? "void"
                        }).ToList(),
                        Summary = GetSummary(m)
                    }).ToList();

                // Properties
                classInfo.Properties = classDecl.DescendantNodes()
                    .OfType<PropertyDeclarationSyntax>()
                    .Select(p => new PropertyInfo
                    {
                        Name = p.Identifier.Text,
                        Type = p.Type.ToString(),
                        Summary = GetSummary(p)
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
    }
}