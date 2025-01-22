using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.IO;
using AutoDoc.Models;

namespace AutoDoc.Services
{
    public class CodeAnalyzer
    {
        public List<DocumentationModel> Analyze(string path)
        {
            var documentation = new List<DocumentationModel>();
            var files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var code = File.ReadAllText(file);
                var tree = CSharpSyntaxTree.ParseText(code);
                var root = tree.GetRoot();

                var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();
                foreach (var method in methods)
                {
                    var model = new DocumentationModel
                    {
                        Name = method.Identifier.Text,
                        Summary = method.GetLeadingTrivia().ToString().Trim(),
                        Parameters = method.ParameterList.Parameters.ToString(),
                        ReturnType = method.ReturnType.ToString()
                    };
                    documentation.Add(model);
                }
            }

            return documentation;
        }
    }
}