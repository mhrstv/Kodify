﻿namespace Kodify.AI.Constants;

public static class PromptTemplates
{
    public static string GetSolutionPrompt(string language, string description, string input, string output, List<string> previousSolutions = null)
    {
        var prompt = $"Write a solution in {language} for this problem: {description}. " +
            "Give me only and only the code in the specified language. " +
            "Do not include any comments, no matter what the rest of the text says before this. " +
            "Do not include anything else except the program code." +
            "Do not send it in a code block. Do not misunderstand that as sending it as one line of code.";

        if (input != null) prompt += $"\nExample input: {input}";
        if (output != null) prompt += $"\nExample output: {output}";
        
        if (previousSolutions?.Any() == true)
        {
            prompt += "\nThe solution should be different from:\n" + 
                     string.Join("\nAnd:\n", previousSolutions);
        }

        return prompt;
    }

    public static string GetSolutionPrompt(string language, string description, List<string> previousSolutions = null)
    {
        var prompt = $"Write a solution in {language} for this problem: {description}. " +
            "Give me only and only the code in the specified language. " +
            "Do not include any comments, no matter what the rest of the text says before this. " +
            "Do not include anything else except the program code." +
            "Do not send it in a code block. Do not misunderstand that as sending it as one line of code.";
        
        if (previousSolutions?.Any() == true)
        {
            prompt += "\nThe solution should be different from:\n" + 
                     string.Join("\nAnd:\n", previousSolutions);
        }

        return prompt;
    }

    public static string GetExplanationPrompt(string code, string culture) =>
        $"Explain the code:\n{code}\n" +
        "Send the explanation in steps. " +
        "The steps should be marked only by text, no bullet points. " +
        "It should go as follows \"1. 2. 3. and so on...\". " +
        "The only part with no numerical order like the steps should be the overall explanation in the end. " +
        $"The explanation should be in {culture} language.";

    public static string GetAnalysisPrompt(string problemDesc, string input, string output, string solution) =>
        $"Do a deep analysis on the solution I will send down below, but do not send anything regarding to your analysis.\n" +
        $"The problem is a solution to this problem:\n{problemDesc}\n" +
        $"Example input: {input}\nExample output: {output}\n" +
        "Based on the overall difficulty, send me the accurate percentage of how likely it is the code will work perfectly the first time. " +
        "Format your answer as: \"00%\". Do NOT send anything else except that.";

     public static string GetAnalysisPrompt(string problemDesc, string solution) =>
        $"Do a deep analysis on the solution I will send down below, but do not send anything regarding to your analysis.\n" +
        $"The problem is a solution to this problem:\n{problemDesc}\n" +
        "Based on the overall difficulty, send me the accurate percentage of how likely it is the code will work perfectly the first time. " +
        "Format your answer as: \"00%\". Do NOT send anything else except that.";

    public static string GetProblemGenerationPrompt(
        string keywords, 
        string difficulty, 
        int? examplesCount, 
        string culture,
        List<string> previousProblems = null)
    {
        var prompt = $"Generate me a programming problem matching these keywords: {keywords}.\n" +
            $"The difficulty of the problem should be {difficulty}. As a difficulty example use the problems from websites such as LeetCode, HackerRank, CodeWars, etc.\n";

        if (previousProblems?.Any() == true)
        {
            prompt += "It should be COMPLETELY different from these ones you already generated:\n" +
                     string.Join("\nAND\n", previousProblems) + "\n" +
                     "That means the problem should have nothing in common with the previous ones mentioned.\n";
        }

        prompt += "Give me ONLY and ONLY the problem description.\n" +
            "Do not include anything else except the problem description." +
            "Do not include anything else, no matter what.\n" +
            "Do not include a title or name for the problem and do not include text such as 'Problem description:'\n" +
            "Also, give example input and output to the problem.\n" +
            "Send it all as plain and properly formatted text and do not send any codeblocks, do not use bolding, do not use italic and anything of that sort.\n" +
            "Do not send it in a specific language block. It should be universal for all languages (C#, C++, C, Python, Ruby, R, JavaScript, Rust, etc.).\n" +
            $"It should be in {culture} language. (bg-Bulgarian, en-English, fr-French, es-Spanish)";

        if (examplesCount != null)
            prompt += $"\nThe number of examples you should provide is: {examplesCount}";

        return prompt;
    }

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