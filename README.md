# Kodify

Kodify is a .NET library that provides AI-powered code generation, problem generation, and code explanation capabilities.

## Key Features

- **Code Solution Generation**: Generate multiple unique code solutions for programming problems in various languages
- **Problem Generation**: Create custom programming problems based on keywords and difficulty levels
- **Code Explanation**: Get detailed step-by-step explanations of code solutions

### Supported Languages

![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white) ![C++](https://img.shields.io/badge/c++-%2300599C.svg?style=for-the-badge&logo=c%2B%2B&logoColor=white) ![Python](https://img.shields.io/badge/python-3670A0?style=for-the-badge&logo=python&logoColor=ffdd54) ![Java](https://img.shields.io/badge/java-%23ED8B00.svg?style=for-the-badge&logo=openjdk&logoColor=white) ![JavaScript](https://img.shields.io/badge/javascript-%23323330.svg?style=for-the-badge&logo=javascript&logoColor=%23F7DF1E) ![TypeScript](https://img.shields.io/badge/typescript-%23007ACC.svg?style=for-the-badge&logo=typescript&logoColor=white)
![SQL](https://img.shields.io/badge/sql-%2307405e.svg?style=for-the-badge&logo=postgresql&logoColor=white) ![PHP](https://img.shields.io/badge/php-%23777BB4.svg?style=for-the-badge&logo=php&logoColor=white) ![Swift](https://img.shields.io/badge/swift-F54A2A?style=for-the-badge&logo=swift&logoColor=white) ![Kotlin](https://img.shields.io/badge/kotlin-%237F52FF.svg?style=for-the-badge&logo=kotlin&logoColor=white) ![Dart](https://img.shields.io/badge/dart-%230175C2.svg?style=for-the-badge&logo=dart&logoColor=white) ![Go](https://img.shields.io/badge/go-%2300ADD8.svg?style=for-the-badge&logo=go&logoColor=white)
![Ruby](https://img.shields.io/badge/ruby-%23CC342D.svg?style=for-the-badge&logo=ruby&logoColor=white) ![Scala](https://img.shields.io/badge/scala-%23DC322F.svg?style=for-the-badge&logo=scala&logoColor=white) ![Rust](https://img.shields.io/badge/rust-%23000000.svg?style=for-the-badge&logo=rust&logoColor=white) ![Racket](https://img.shields.io/badge/racket-%23A0522D.svg?style=for-the-badge&logo=racket&logoColor=white) ![Erlang](https://img.shields.io/badge/erlang-%23A90533.svg?style=for-the-badge&logo=erlang&logoColor=white) ![Elixir](https://img.shields.io/badge/elixir-%234B275F.svg?style=for-the-badge&logo=elixir&logoColor=white)
## Installation

You can install the package via NuGet:


```shell
dotnet add package Kodify
```
## Configuration

To use Kodify, you need to configure the OpenAI settings:


```csharp
var config = new OpenAIConfig
{
ApiKey = "your-api-key",
Model = "gpt-4" // or your preferred model
};
var openAIService = new OpenAIService(config);
```

## Usage

### Generating Code Solutions
```csharp
var solutions = openAIService.GenerateSolutionsAsync(
    description: "Write a function that finds the sum of two numbers",
    input: "2, 3",
    output: "5",
    language: "C#",
    culture: "en",
    solutionCount: 3
);

await foreach (var solution in solutions)
{
    Console.WriteLine($"Code: {solution.Code}");
    Console.WriteLine($"Explanation: {solution.Explanation}");
    Console.WriteLine($"Accuracy: {solution.Accuracy}");
}
```

### Generating Programming Problems

```csharp
var problems = openAIService.GenerateProblemsAsync(
    keywords: "arrays sorting",
    difficulty: "medium",
    culture: "en",
    examplesCount: 2,
    problemCount: 3
);

await foreach (var problem in problems)
{
    Console.WriteLine($"Description: {problem.Description}");
    Console.WriteLine($"Keywords: {problem.Keywords}");
    Console.WriteLine($"Difficulty: {problem.Difficulty}");
}
```

## Features (Detailed)

### Code Solution Generation
- Generates multiple unique solutions for a given problem
- Provides explanations for each solution
- Includes accuracy predictions
- Supports multiple programming languages
- Avoids generating duplicate solutions

### Problem Generation
- Creates custom programming problems based on keywords
- Supports different difficulty levels
- Provides example inputs and outputs
- Supports multiple languages for problem descriptions
- Ensures generated problems are unique

## Dependencies

- .NET 8.0
- OpenAI (v2.1.0)

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Version

 0.0.4: Current release (latest stable)
## Project Links & Dependancy References

- [GitHub Repository](https://github.com/mhrstv/kodify)
