using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kodify.AutoDoc.Models
{
    public class ProjectInfo
    {
        public string ProjectPath { get; set; }
        public List<CodeFile> SourceFiles { get; set; } = new();
        public List<NonCodeFile> OtherFiles { get; set; } = new();
        public List<ClassInfo> Classes { get; set; } = new();
        public List<InterfaceInfo> Interfaces { get; set; } = new();
        public List<EnumInfo> Enums { get; set; } = new();
        public ProjectStructure Structure { get; set; } = new();
        public LicenseInfo License { get; set; } = new();
        public bool HasWebApi { get; set; }
    }

    public class CodeFile
    {
        public string FilePath { get; set; }
        public List<ClassInfo> Classes { get; set; } = new();
        public List<MethodInfo> Methods { get; set; } = new();
        public List<PropertyInfo> Properties { get; set; } = new();
    }

    public class NonCodeFile
    {
        public string FilePath { get; set; }
        public string FileType { get; set; }
    }

    public class ProjectStructure
    {
        public List<string> Directories { get; set; } = new();
        public List<string> FileTypes { get; set; } = new();
    }

    public class ClassInfo
    {
        public string Name { get; set; }
        public List<string> BaseTypes { get; set; } = new();
        public List<string> Attributes { get; set; } = new();
        public List<MethodInfo> Methods { get; set; } = new();
        public string Summary { get; set; }
        public bool IsApiController => BaseTypes.Contains("ControllerBase") ||
                                      BaseTypes.Contains("ApiController") ||
                                      Attributes.Contains("ApiController");
    }

    public class MethodInfo
    {
        public string Name { get; set; }
        public List<string> Attributes { get; set; } = new();
        public string ReturnType { get; set; }
        public List<ParameterInfo> Parameters { get; set; } = new();
        public string Summary { get; set; }
    }

    public class PropertyInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Summary { get; set; }
    }

    public class ParameterInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class LicenseInfo
    {
        public string Type { get; set; }
        public string FilePath { get; set; }
        public string Content { get; set; }
    }

    public class InterfaceInfo {  }
    public class EnumInfo {  }
}
