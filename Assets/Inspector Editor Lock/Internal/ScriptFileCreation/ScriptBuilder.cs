using System.Collections.Generic;
using System.Text;

namespace ScriptFileCreation
{
    
    public class ScriptBuilder
    {
        private StringBuilder m_Inheritance = new StringBuilder(string.Empty);
        private StringBuilder m_Using = new StringBuilder(string.Empty);
        
        private CodeBlock m_NameSpace;
        private CodeBlock m_Class;
        private CodeBlock m_Code;

        public CodeBlock Code => m_Code;

        public ScriptBuilder(string fileName)
        {
            m_Class = new CodeBlock(new CodeLine($"public class {StringHelpers.WithoutEnding(fileName)}"));
            m_NameSpace = CodeBlockDefaults.CodeBlockEmpty;
            m_Code = CodeBlockDefaults.CodeBlockEmpty;
        }

        public ScriptBuilder WithInheritance(string inheritance)
        {
            if(m_Inheritance.ToString() == string.Empty)
            {
                m_Inheritance.Append($": {inheritance}");
                return this;
            }
            
            m_Inheritance.AppendLine($"{inheritance}, ");
            return this;
        }

        public ScriptBuilder WithInheritance(string[] inheritance) 
        {
            foreach (string argument in inheritance)
            {
                WithInheritance(argument);
            }

            return this;
        }
        
        /// <summary>
        /// Add a line of code to this instance of the ScriptBuilder.
        /// </summary>
        /// <param name="content">The string code to add.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public ScriptBuilder AddCodeLine(string content)
        {
            content = content.EndsWith(";") ? content.Remove(content.Length -1) 
                                            : content;
            
            if (m_Code is CodeBlockEmpty)
            {
                m_Code = new CodeBlock(new CodeLine(content, ";"));
                return this;
            }

            m_Code.AddContent(new CodeLine(content, ";"));
            return this;
        }

        /// <summary>
        /// Add an attribute field to the code of this ScriptBuilder.
        /// </summary>
        /// <param name="attribute">The string attribute to add.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public ScriptBuilder AddCodeLineAttributeField(string attribute)
        {
            attribute = attribute.Replace("[", string.Empty)
                                 .Replace("]", string.Empty);

            if (m_Code is CodeBlockEmpty)
            {
                m_Code = new CodeBlock(new CodeLine($"[{attribute}]"));
                return this;
            }

            m_Code.AddContent(new CodeLine($"[{attribute}]"));
            return this;
        }

        public ScriptBuilder AddClassAttributeField(string attribute)
        {
            attribute = attribute.Replace("[", string.Empty)
                .Replace("]", string.Empty);

            m_Class.AddAttribute(new CodeLine($"[{attribute}]"));
            return this;
        }

        /// <summary>
        /// Declare a using directive to add at the beginning of this script.
        /// </summary>
        /// <param name="nameSpace">The namespace to add.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public ScriptBuilder WithUsing(string nameSpace)
        {
            m_Using.AppendLine(StringHelpers.CreateUsingString(nameSpace));
            return this;
        }

        /// <summary>
        /// Declare several using directives to add at the beginning of this script.
        /// </summary>
        /// <param name="nameSpaces">The namespaces to add.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public ScriptBuilder WithUsings(string[] nameSpaces)
        {
            foreach (string nameSpace in nameSpaces)
            {
                WithUsing(nameSpace);
            }

            return this;
        }

        /// <summary>
        /// Declare a namespace scope for this script. Overwrites any previously declared scope.
        /// </summary>
        /// <param name="nameSpace">The namespace scope of this script.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public ScriptBuilder WithNamespace(string nameSpace)
        {
            m_NameSpace = CodeBlock.New($"namespace {nameSpace}");
            m_NameSpace.AddBody(m_Class);
            return this;
        }

        /// <summary>
        /// Assemble this ScriptBuilder into a string to be used as the content of a C# file.
        /// </summary>
        /// <returns>The assembled script as a string.</returns>
        public string Build() => AssembleScript();

        private string AssembleScript()
        {
            m_Class.AddBody(m_Code);
            
            var stringBuilder = new StringBuilder();
            if (m_Inheritance.Length > 0)
            {
                m_Class.AppendToContent(0, m_Inheritance.ToString());
            }

            stringBuilder.AppendLine(m_Using.ToString())
                         .AppendLine(m_NameSpace.ToString());
            
            return stringBuilder.ToString();
        }

        public override string ToString() => AssembleScript();
    }
}