using System.Collections.Generic;
using System.Text;

namespace ScriptFileCreation
{

    /// <summary>
    /// A class representing a code block. Can contain parent and child blocks, allowing for code indentation based on depth inside the hierarchy.
    /// </summary>
    public class CodeBlock : ICodeHierarchy
    {
        public CodeBlock(List<CodeLine> content)
        {
            m_Content = content;
            m_Attributes = new List<CodeLine>();
            m_Parent = this;
            m_CodeBody = CodeBlockDefaults.CodeBlockEmpty;
        }

        public CodeBlock(CodeLine content) : this(new List<CodeLine>() { content })
        {
        }

        protected List<CodeLine> m_Attributes;
        protected List<CodeLine> m_Content;

        protected ICodeHierarchy m_Parent;
        protected ICodeHierarchy m_CodeBody;

        private char m_BodyStart = '{';
        private char m_BodyEnd = '}';

        public int IndentationAmount = 0;

        public static CodeBlock New(string content) =>
            new CodeBlock(new CodeLine(content, ""));

        /// <summary>
        /// Returns the <see cref="ICodeHierarchy"/> parent object. If unset, return this <see cref="ICodeHierarchy"/>.
        /// </summary>
        public virtual ICodeHierarchy Parent => m_Parent;

        /// <summary>
        /// Traverses up the hierarchy to return the <see cref="ICodeHierarchy"/> Root object. Return this <see cref="ICodeHierarchy"/> if it's the Root.
        /// </summary>
        public virtual ICodeHierarchy Root =>
            Parent == this
                ? this
                : Parent.Root;

        /// <summary>
        /// Returns the <see cref="ICodeHierarchy"/> code body of this object. If unset, returns <see cref="CodeBlockEmpty"/>.
        /// </summary>
        public virtual ICodeHierarchy CodeBody => m_CodeBody;

        /// <summary>
        /// Adds a line of code to this <see cref="CodeBlock"/>'s content.
        /// </summary>
        /// <param name="content">The <see cref="CodeLine"/> code content to be added.</param>
        public virtual void AddContent(CodeLine content)
        {
            m_Content.Add(content);
        }

        public virtual void InsertContent(int index, CodeLine content)
        {
            m_Content.Insert(index, content);
        }

        public virtual void AddAttribute(CodeLine content)
        {
            m_Attributes.Add(content);
        }

        /// <summary>
        /// Appends a line of code at the end of an already existing <see cref="CodeLine"/> in this <see cref="CodeBlock"/>'s content.
        /// </summary>
        /// <param name="contentIndex">The index of the <see cref="CodeLine"/> to append to.</param>
        /// <param name="appendText">The string text to append.</param>
        public virtual void AppendToContent(int contentIndex, string appendText)
        {
            if (m_Content.Count < contentIndex)
            {
                return;
            }

            m_Content[contentIndex] = new CodeLine(m_Content[contentIndex] + appendText);
        }

        /// <summary>
        /// Adds a <see cref="CodeBlock"/> child to this <see cref="CodeBlock"/>'s hierarchy.
        /// </summary>
        /// <param name="scriptBody"></param>
        public virtual void AddBody(CodeBlock scriptBody)
        {
            this.m_CodeBody = scriptBody;
            scriptBody.IndentationAmount = this.IndentationAmount + 1;
            scriptBody.m_Parent = this;
        }

        public string EmptyCodebody()
        {
            var indentation = StringHelpers.LineIndentation(IndentationAmount);
            return indentation + "{" + "\n" + indentation + "}";
        }

        public override string ToString()
        {
            if (m_Content.Count == 0)
            {
                return string.Empty;
            }

            var indentation = StringHelpers.LineIndentation(IndentationAmount);
            var result = new StringBuilder();

            foreach (var attribute in m_Attributes)
            {
                result.AppendLine($"{indentation}{attribute.ToString()}");
            }

            foreach (var codeLine in m_Content)
            {
                result.AppendLine($"{indentation}{codeLine.ToString()}");
            }

            if (CodeBody is not CodeBlockEmpty)
            {
                result.AppendLine($"{indentation}{m_BodyStart}");
                result.AppendLine($"{m_CodeBody?.ToString()}");
                result.Append($"{indentation}{m_BodyEnd}");
            }

            return result.ToString();
        }

    }
}