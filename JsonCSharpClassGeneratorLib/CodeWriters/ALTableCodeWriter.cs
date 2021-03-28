using System.IO;

namespace Xamasoft.JsonClassGenerator.CodeWriters
{
    public class ALTableCodeWriter : ICodeWriter
    {
        public string FileExtension
        {
            get { return ".Table.al"; }
        }

        public string DisplayName
        {
            get { return "AL Table"; }
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

            sw.WriteLine("table {0} \"{1}\"", Counter, tableName);
            sw.WriteLine("{");
            sw.WriteLine("\tCaption = '{0}';", type.AssignedName);
            sw.WriteLine("\tDataPerCompany = true;");
            sw.WriteLine("\tDataClassification = CustomerContent;");
            sw.WriteLine("\tLookupPageId = \"{0} List\";", tableName);
            sw.WriteLine("\tDrillDownPageId = \"{0} List\";", tableName);
            sw.WriteLine("\tfields");
            sw.WriteLine("\t{");

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
                sw.WriteLine("\t\t}");

            if (!config.UseNestedClasses)
                sw.WriteLine("\t}");

            sw.WriteLine("\ttrigger OnInsert();");
            sw.WriteLine("\tbegin");
            sw.WriteLine("\t\t{0}_guid := CreateGuid();", type.AssignedName);
            sw.WriteLine("\tend;");

            sw.WriteLine();


        }

        private void WriteClassMembers(IJsonClassGeneratorConfig config, TextWriter sw, JsonType type, string prefix)
        {
            var tableKey = config.MainClass + "_guid";
            sw.WriteLine("\t\tfield({0}; {1}_guid; {2})", 1, type.AssignedName, "Guid");
            sw.WriteLine("\t\t{");
            sw.WriteLine("\t\t\tDataClassification = CustomerContent;");
            sw.WriteLine("\t\t\tDescription = '{0}_guid';", type.AssignedName);
            sw.WriteLine("\t\t}");

            sw.WriteLine("\t\tfield({0}; {1}; Guid)", 2, tableKey);
            sw.WriteLine("\t\t{");
            sw.WriteLine("\t\t\tDataClassification = CustomerContent;");
            sw.WriteLine("\t\t\tDescription = '{0}';", tableKey);
            sw.WriteLine("\t\t}");

            int fieldCount = 3;
            foreach (var field in type.Fields)
            {
                if (config.UsePascalCase || config.ExamplesInDocumentation) sw.WriteLine();
                {
                    //sw.WriteLine(prefix + "public {0} {1} {{ get; set; }}", field.Type.GetTypeName(), field.MemberName);
                    sw.WriteLine("\t\tfield({0}; {1}; {2})", fieldCount++, field.MemberName, field.Type.GetTypeName());
                    sw.WriteLine("\t\t{");
                    sw.WriteLine("\t\t\tDataClassification = CustomerContent;");
                    sw.WriteLine("\t\t\tDescription = '{0}';", field.MemberName);
                    sw.WriteLine("\t\t}");

                }
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
