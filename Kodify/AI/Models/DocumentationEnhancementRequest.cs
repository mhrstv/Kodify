namespace Kodify.AI.Models
{
    public class DocumentationEnhancementRequest
    {
        public string Template { get; set; }
        public List<string> ReadmeSections { get; set; }
        public string ProjectName { get; set; }
        public string ProjectSummary { get; set; }
    }
}

namespace Kodify.AI.Models
{
    public class DocumentationEnhancementResponse
    {
        public string EnhancedContent { get; set; }
        public bool Success { get; set; }
        public string Error { get; set; }
    }
}
