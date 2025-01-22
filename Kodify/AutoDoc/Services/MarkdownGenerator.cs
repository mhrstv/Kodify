using System;
using AutoDoc.Models;
using Markdig;
using System.Collections.Generic;
using System.IO;

namespace AutoDoc.Services
{
    public class MarkdownGenerator
    {
        public void GenerateMarkdown(List<DocumentationModel> documentation, string outputPath)
        {
            try
            {
                // Ensure the output directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

                using (var writer = new StreamWriter(outputPath))
                {
                    writer.WriteLine("# Documentation");
                    foreach (var doc in documentation)
                    {
                        writer.WriteLine($"## {doc.Name}");
                        writer.WriteLine($"**Summary:** {doc.Summary}");
                        writer.WriteLine($"**Parameters:** {doc.Parameters}");
                        writer.WriteLine($"**Return Type:** {doc.ReturnType}");
                        writer.WriteLine();
                    }
                }
            }
            catch (DirectoryNotFoundException dirEx)
            {
                // Handle the case where the directory is not found
                Console.WriteLine($"Error: The specified directory was not found. {dirEx.Message}");
            }
            catch (IOException ioEx)
            {
                // Handle I/O errors (e.g., file access issues)
                Console.WriteLine($"Error: An I/O error occurred while writing to the file. {ioEx.Message}");
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Handle unauthorized access errors
                Console.WriteLine($"Error: You do not have permission to write to this file. {authEx.Message}");
            }
            catch (Exception ex)
            {
                // Handle any other unexpected exceptions
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}