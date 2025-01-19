namespace Kodify.AI.Constants;

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
} 