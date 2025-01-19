namespace Kodify.AI.Configuration;

public class OpenAIConfig
{
    public string ApiKey { get; set; }
    public string Model { get; set; } = "gpt-4o-mini";
    
    // You might want to load this from configuration/settings later
    public static OpenAIConfig Default => new()
    {
        ApiKey = "api-key",
        Model = "gpt-4o-mini"
    };
} 