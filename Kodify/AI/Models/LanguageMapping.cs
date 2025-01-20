namespace Kodify.AI.Models;

public static class LanguageMapping
{
    public static readonly Dictionary<string, string> SupportedLanguages = new()
    {
        { "C#", "csharp" },
        { "C++", "cpp" },
        { "Python", "python" },
        { "Java", "java" },
        { "JavaScript", "javascript" },
        { "TypeScript", "typescript" },
        { "SQL", "sql" },
        { "PHP", "php" },
        { "Swift", "swift" },
        { "Kotlin", "kotlin" },
        { "Dart", "dart" },
        { "Go", "go" },
        { "Ruby", "ruby" },
        { "Scala", "scala" },
        { "Rust", "rust" },
        { "Racket", "racket" },
        { "Erlang", "erlang" },
        { "Elixir", "elixir" }
    };

    public static string GetLanguageIdentifier(string language)
    {
        return SupportedLanguages.TryGetValue(language, out var identifier) 
            ? identifier 
            : throw new ArgumentException($"Unsupported language: {language}");
    }
} 