# Kodify

Kodify is an intelligent project automation library that streamlines documentation, release management, and development workflows.

## Features

- **Automated Documentation**: Generate comprehensive documentation from source code and commit history
- **Changelog Automation**: Create maintainer-friendly changelogs with semantic version tracking
- **CI/CD Integration**: Smart GitHub Actions workflow generation and optimization
- **Project Scaffolding**: Automated setup for new projects with best-practice templates
- **Dependency Management**: Intelligent NuGet package updates and compatibility checks
- **Code Analysis**: Automated code quality reports and technical debt tracking

### Supported Platforms

![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white) ![GitHub Actions](https://img.shields.io/badge/github%20actions-%232671E5.svg?style=for-the-badge&logo=githubactions&logoColor=white) ![NuGet](https://img.shields.io/badge/NuGet-004880?style=for-the-badge&logo=nuget&logoColor=white) ![Markdown](https://img.shields.io/badge/markdown-%23000000.svg?style=for-the-badge&logo=markdown&logoColor=white) ![YAML](https://img.shields.io/badge/YAML-%232C8EBB.svg?style=for-the-badge)


## Installation

```shell
dotnet add package Kodify
```

## Configuration - W.I.P.

Set up automation rules in your .kodifyrc config file:

```yaml
rules:
  documentation:
    auto_update: true
    formats: [md, pdf]
  changelog:
    semantic_versioning: true
  ci_cd:
    parallel_jobs: 4
    cache_ttl: 24h
```

## Usage Examples

### Automated Documentation Generation
```csharp
var docs = Kodify.GenerateDocumentation(
    projectPath: "./src",
    outputFormats: new[] { "md", "html" }, // W.I.P.
    includeDiagrams: true
);
```

### Changelog Management - W.I.P.
```csharp
var changelog = Kodify.UpdateChangelog(
    sinceVersion: "1.2.0",
    releaseNotes: "Added security scanning features",
    bumpType: VersionBump.Minor
);
```

## Core Capabilities

### Intelligent Automation
- Automatic documentation sync with source changes
- Semantic versioning enforcement
- CI/CD pipeline optimization // TBR
- Dependency vulnerability scanning // TBR
- Cross-platform project scaffolding

### Integration Support
- GitHub/GitLab/Bitbucket native integration
- Jira/Linear issue tracking sync // TBR
- Slack/Teams notification pipelines // TBR
- NuGet/NPM package registry support

## Dependencies

- .NET 8.0
- LibGit2Sharp (v0.27.0)
- YamlDotNet (v12.3.1) // TBR

## License
MIT License - See [LICENSE](LICENSE)

## Project Resources
- [GitHub Repository](https://github.com/mhrstv/kodify)