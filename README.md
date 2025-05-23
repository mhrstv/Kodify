# Kodify

Kodify is an **AI-powered .NET library** that automates your project's documentation, code analysis, changelog generation, and visualization. It streamlines development workflows by enhancing standard project metadata with AI-curated explanations, intelligent code analysis using Roslyn.

![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge&logo=.net&logoColor=white)
![GitHub Actions](https://img.shields.io/badge/GitHub-Actions-active?style=for-the-badge&logo=github)
![License](https://img.shields.io/badge/license-MIT-blue?style=for-the-badge)

---

## Features

- **Automated Documentation Generation**  
  Generate comprehensive, user-focused README files using structured project data and AI-enhanced language. The documentation is built from source analysis and enhanced via the OpenAI API.

- **Changelog Automation**  
  Automatically generate curated changelogs by scanning Git commit history. Commit messages are processed to replace issue/PR references (like `#123`) with clickable GitHub links for improved traceability.

- **Static Code Analysis**  
  Leverage Roslyn to analyze your C# code, detect API controllers, extract key project structure details, and automatically detect license files.

- **Seamless Git Integration**  
  Automatically detect your project's Git repository; normalize remote URLs and, when applicable, integrate with GitHub to enrich commit messages with clickable links.

- **Customizable Content Generation**  
  Use the included ContentBuilder and ReadmeGenerator to either automatically generate structured documentation or compose manual content enhanced by AI.

- **OpenAI-Powered Enhancement**  
  Improve documentation language, structure, and consistency through customizable prompt templates and an integrated OpenAI service.

---

## Installation

Install Kodify via the .NET CLI:

```shell
dotnet add package Kodify
```

---

## Quick Start

### 1. Create an instance of an AI service

Use the `OpenAIService` to create an instance of the AI service which serves as your docs enhancer:

```csharp
var aiConfig = new OpenAIConfig 
{ 
    ApiKey = "",
    Model = "gpt-4o-mini" 
};
var aiService = new OpenAIService(openAiConfig);
```

### 2. Generate your README file

Generate a README by combining analytical data with AI enhancement:

```csharp
var markdownGenerator = new MarkdownGenerator(aiService);

// Generate a README file for your project
await markdownGenerator.GenerateReadMe("MyProject",
    "A concise summary that showcases the project's purpose, features, and value.",
    "Step-by-step usage instructions go here..." // Shortened, of course
);
```

### 3. Create a Changelog

Automatically generate a changelog that aggregates commit history:

```csharp
// This invokes the ChangelogGenerator which creates a CHANGELOG.md in your project root
markdownGenerator.GenerateChangelog();
markdownGenerator.GenerateChangelog(aiService); // This overload uses the AI service to clean up messy commits, PRs and comments.
```
---

## Detailed Usage

### Documentation & Content Generation

- **Structured Content Building:**  
  The `ContentBuilder` compiles key details (e.g., API detection, licenses, and key features) into markdown, which is later enhanced by the AI.

- **Manual Content Customization:**  
  Not a fan of full automation? Use `BuildManualContent` from the ContentBuilder to create a more traditional README layout, then enhance with the AI service.

- **AI Enhancement:**  
  The `ReadmeGenerator` and `MarkdownGenerator` blend the raw content with prompt templates (defined in `PromptTemplates.cs`), resulting in user-friendly, production-ready documentation.

### Changelog Generation

- **Commit Curation:**  
  The `ChangelogGenerator` groups changes by Git tags (or treats commit history as unreleased changes) and annotates commit messages. Issue references like `#123` are automatically converted to clickable links when a GitHub repository is detected.
  
### Code Analysis

- **CodeAnalyzer:**  
  Parses C# source files to generate a `ProjectInfo` object. It provides insights such as detected API controllers, directory structures, and license files.

### Git Integration

- **GitRepositoryService & GitHubApiService:**  
  These services detect Git repositories, normalize URLs (removing any trailing `.git`), and optionally integrate with GitHub for enriching repository context and commit annotations.

---

## Configuration & Customization

- **OpenAI API:**  
  Set up your `OpenAIConfig` by ensuring the `OPENAI_API_KEY` environment variable is properly configured.

- **Prompt & Template Customization:**  
  Edit `PromptTemplates.cs` to tailor the AI prompts to your desired tone and structure.

- **Advanced Settings:**  
  Extend classes like `ContentBuilder` and `ReadmeGenerator` to further customize the generated documentation and changelog to suit your project's style.

---

## Project Resources

- **Source Code & Repository:**  
  [GitHub Repository](https://github.com/mhrstv/Kodify)

- **License:**  
  Kodify is released under the MIT License. See [LICENSE](LICENSE) for more information.

---

## Dependencies
- **LibGit2Sharp**
- **Microsoft.CodeAnalysis (Roslyn)**
- **OpenAI**
- **Newtonsoft.Json**
