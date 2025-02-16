namespace Kodify.AI.Constants;
using System.Collections.Generic;

public static class PromptTemplates
{
    public static string GetDocumentationPrompt(
    string projectName,
    string projectSummary,
    string usageInstructions,
    string structuredContent,
    bool hasApi, string licenseType)
    {
        return $@"
        Create a clean, user-focused README for: {projectName}
        Summary: {projectSummary}
        Usage: {usageInstructions}

        RULES:
        1. {(hasApi ? "Include API section" : "Absolutely NO technical metadata")}
        2. Never mention files or code structure
        3. Focus on end-user features
        4. Use simple, engaging language

        LICENSE INSTRUCTIONS:
        {GetLicenseInstructions(licenseType)}

        SAMPLE STRUCTURE:
        # [Project Name]
        [Brief overview]
        
        ## Features
        - [User-facing features]
        
        ## Getting Started
        [Installation/Usage]
        
        {(hasApi ? "## API Reference" : "")}

        ## Contributing
        [Basic guidelines]
    ";
    }

    public static string GetDocumentationEnhancementPrompt(string template, List<string> sections)
    {
        return $@"
        ENHANCE THIS DOCUMENTATION USING THE FOLLOWING TEMPLATE:
        {template}

        EXISTING CONTENT SECTIONS:
        {string.Join("\n", sections)}

        INSTRUCTIONS:
        1. Merge template structure with existing content
        2. Maintain all technical details from existing content
        3. Improve language and professionalism
        4. Add missing standard sections from template
        5. Ensure markdown formatting is valid
        6. Add appropriate emojis where suitable
        7. Keep technical accuracy as highest priority

        OUTPUT REQUIREMENTS:
        - Valid markdown format
        - No code blocks unless for actual code examples
        - Section headers must match template hierarchy
        - Final output must be production-ready
    ";
    }

    public static string GetChangelogEnhancementPrompt(string rawChangelog)
    {
        return $@"
        Please clean up the following changelog text by improving clarity and readability of commit messages and PR annotations.
        Keep the overall structure (section headers, grouping, formatting, etc.) intact and modify only parts that are unclear or messy.

        RAW CHANGELOG:
        {rawChangelog}
    ";
    }

    private static string GetLicenseInstructions(string licenseType)
    {
        return licenseType switch
        {
            "MIT" => "Include MIT license section with badge and link",
            "Apache-2.0" => "Include Apache 2.0 license section",
            "None" => "Add prominent 'No License' warning section",
            _ => "Create custom license section"
        };
    }
} 