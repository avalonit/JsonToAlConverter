using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Xamasoft.JsonClassGenerator.CodeWriters
{
    public class ALPageCodeWriter : ICodeWriter
    {
        public string FileExtension
        {
            get { return "List.Page.al"; }
        }

        public string DisplayName
        {
            get { return "AL Page"; }
        }

        private int counter = 70260010;
        public int Counter
        {
            get { return counter++; }
        }

        public string GetTypeName(JsonType type, IJsonClassGeneratorConfig config)
        {
            var arraysAsLists = !config.ExplicitDeserialization;

            switch (type.Type)
            {
                case JsonTypeEnum.Anything: return "Text[250]";
                case JsonTypeEnum.Array: return arraysAsLists ? "IList<" + GetTypeName(type.InternalType, config) + ">" : GetTypeName(type.InternalType, config) + "[]";
                case JsonTypeEnum.Dictionary: return "Dictionary<string, " + GetTypeName(type.InternalType, config) + ">";
                case JsonTypeEnum.Boolean: return "Boolean";
                case JsonTypeEnum.Float: return "Boolean";
                case JsonTypeEnum.Integer: return "Integer";
                case JsonTypeEnum.Long: return "BigInteger";
                case JsonTypeEnum.Date: return "DateTime";
                case JsonTypeEnum.NonConstrained: return "Integer";
                case JsonTypeEnum.NullableBoolean: return "Boolean";
                case JsonTypeEnum.NullableFloat: return "Boolean";
                case JsonTypeEnum.NullableInteger: return "Integer";
                case JsonTypeEnum.NullableLong: return "BigInteger";
                case JsonTypeEnum.NullableDate: return "DateTime";
                case JsonTypeEnum.NullableSomething: return "Text[250]";
                case JsonTypeEnum.Object: return "Text[250]";
                case JsonTypeEnum.String: return "Text[250]";
                default: throw new System.NotSupportedException("Unsupported json type");
            }
        }


        private bool ShouldApplyNoRenamingAttribute(IJsonClassGeneratorConfig config)
        {
            return config.ApplyObfuscationAttributes && !config.ExplicitDeserialization && !config.UsePascalCase;
        }
        private bool ShouldApplyNoPruneAttribute(IJsonClassGeneratorConfig config)
        {
            return config.ApplyObfuscationAttributes && !config.ExplicitDeserialization && config.UseProperties;
        }

        public void WriteFileStart(IJsonClassGeneratorConfig config, TextWriter sw)
        {
            foreach (var line in JsonClassGenerator.FileHeader)
            {
                sw.WriteLine("// " + line);
            }
            sw.WriteLine();
            sw.WriteLine("//Placeholder for my AL file");
        }

        public void WriteFileEnd(IJsonClassGeneratorConfig config, TextWriter sw)
        {
            if (config.UseNestedClasses)
            {
                sw.WriteLine("    }");
            }
        }


        public void WriteNamespaceStart(IJsonClassGeneratorConfig config, TextWriter sw, bool root)
        {
            sw.WriteLine("//Placeholder for my AL file : namespace start");
        }

        public void WriteNamespaceEnd(IJsonClassGeneratorConfig config, TextWriter sw, bool root)
        {
            sw.WriteLine("}");
            sw.WriteLine("//Placeholder for my AL file : namespace end");
        }

        public void WriteClass(IJsonClassGeneratorConfig config, TextWriter sw, JsonType type)
        {
            var visibility = config.InternalVisibility ? "internal" : "public";

            var tableName = type.AssignedName;
            if (!type.AssignedName.Equals(config.SecondaryNamespace))
                tableName = config.SecondaryNamespace + tableName;
            sw.WriteLine("page {0} \"{1} List\"", Counter, tableName);
            sw.WriteLine("{");
            sw.WriteLine("\tPageType = List;");
            sw.WriteLine("\tSourceTable = \"{0}\";", tableName);
            sw.WriteLine("\tApplicationArea = All;");
            sw.WriteLine("\tUsageCategory = Lists;");
            sw.WriteLine("\tCaption = '{0} List';", tableName);
            sw.WriteLine("");
            sw.WriteLine("\tlayout");
            sw.WriteLine("\t{");
            sw.WriteLine("\t\tarea(Content)");
            sw.WriteLine("\t\t{");
            sw.WriteLine("\t\t\trepeater(Group)");
            sw.WriteLine("\t\t\t{");

            var prefix = config.UseNestedClasses && !type.IsRoot ? "            " : "        ";

            if (type.IsRoot && config.ExplicitDeserialization) WriteStringConstructorExplicitDeserialization(config, sw, type, prefix);

            if (config.ExplicitDeserialization)
            {
                if (config.UseProperties) WriteClassWithPropertiesExplicitDeserialization(sw, type, prefix);
                else WriteClassWithFieldsExplicitDeserialization(sw, type, prefix);
            }
            else
            {
                WriteClassMembers(config, sw, type, prefix);
            }

            if (config.UseNestedClasses && !type.IsRoot)
                sw.WriteLine("\t\t\t}");

            if (!config.UseNestedClasses)
                sw.WriteLine("\t\t}");

            sw.WriteLine("\t}");
            sw.WriteLine("}");
            sw.WriteLine();


        }

        private void WriteClassMembers(IJsonClassGeneratorConfig config, TextWriter sw, JsonType type, string prefix)
        {
            sw.WriteLine("\t\t\t\tfield(\"{0}_guid\"; Rec.\"{0}_guid\")", type.AssignedName);
            sw.WriteLine("\t\t\t\t{");
            sw.WriteLine("\t\t\t\t\tCaption = '{0}_guid';", type.AssignedName);
            sw.WriteLine("\t\t\t\t\tApplicationArea = All;");
            sw.WriteLine("\t\t\t\t\tToolTip = '{0}_guid';", type.AssignedName);
            sw.WriteLine("\t\t\t\t}");

            var tableKey = config.MainClass + "_guid";

            sw.WriteLine("\t\t\t\tfield(\"{0}\"; Rec.\"{1}\")", tableKey, tableKey);
            sw.WriteLine("\t\t\t\t{");
            sw.WriteLine("\t\t\t\t\tCaption = '{0}';", tableKey);
            sw.WriteLine("\t\t\t\t\tApplicationArea = All;");
            sw.WriteLine("\t\t\t\t\tToolTip = '{0}';", tableKey);
            sw.WriteLine("\t\t\t\t}");

            foreach (var field in type.Fields)
            {
                if (config.UsePascalCase || config.ExamplesInDocumentation) sw.WriteLine();
                //sw.WriteLine(prefix + "public {0} {1} {{ get; set; }}", field.Type.GetTypeName(), field.MemberName);
                sw.WriteLine("\t\t\t\tfield(\"{0}\"; Rec.\"{0}\")", field.MemberName);
                sw.WriteLine("\t\t\t\t{");
                sw.WriteLine("\t\t\t\t\tCaption = '{0}';", field.MemberName);
                sw.WriteLine("\t\t\t\t\tApplicationArea = All;");
                sw.WriteLine("\t\t\t\t\tToolTip = '{0}';", field.MemberName);
                sw.WriteLine("\t\t\t\t}");
            }
        }

        #region Code for (obsolete) explicit deserialization
        private void WriteClassWithPropertiesExplicitDeserialization(TextWriter sw, JsonType type, string prefix)
        {
        }

        private void WriteStringConstructorExplicitDeserialization(IJsonClassGeneratorConfig config, TextWriter sw, JsonType type, string prefix)
        {
        }

        private void WriteClassWithFieldsExplicitDeserialization(TextWriter sw, JsonType type, string prefix)
        {

        }
        #endregion

    }
}
