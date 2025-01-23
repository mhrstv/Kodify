using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;
using System.Text;

namespace Kodify.AutoDoc.Services
{
    public class ClassDiagramGenerator
    {
        public void GenerateClassDiagrams(string outputPath)
        {
            var files = Directory.GetFiles(outputPath, "*.cs", SearchOption.AllDirectories);
            var classDiagramContent = new StringBuilder();

            classDiagramContent.AppendLine("@startuml");

            foreach (var file in files)
            {
                var code = File.ReadAllText(file);
                var tree = CSharpSyntaxTree.ParseText(code);
                var root = tree.GetRoot();

                var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
                foreach (var classDecl in classes)
                {
                    classDiagramContent.AppendLine($"class {classDecl.Identifier.Text} {{");
                    // Add properties and methods
                    foreach (var member in classDecl.Members)
                    {
                        if (member is MethodDeclarationSyntax method)
                        {
                            classDiagramContent.AppendLine($"    + {method.Identifier.Text}()"); // Public methods
                        }
                        else if (member is PropertyDeclarationSyntax property)
                        {
                            classDiagramContent.AppendLine($"    + {property.Identifier.Text} : {property.Type}"); // Public properties
                        }
                    }
                    classDiagramContent.AppendLine("}");
                    classDiagramContent.AppendLine();
                }
            }

            classDiagramContent.AppendLine("@enduml");

            File.WriteAllText(Path.Combine(outputPath, "ClassDiagrams.puml"), classDiagramContent.ToString());
        }
    }
}
