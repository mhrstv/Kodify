namespace Kodify.AI.Configuration;

public class OpenAIConfig : IAIConfig
{
    public string ApiKey { get; set; }
    public string Model { get; set; } = "gpt-4o-mini";
    
    // You need to call this with a proper OpenAI API key
    public static OpenAIConfig Default => new()
    {
        ApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "api-key",
        Model = "gpt-4o-mini"
    };
} 