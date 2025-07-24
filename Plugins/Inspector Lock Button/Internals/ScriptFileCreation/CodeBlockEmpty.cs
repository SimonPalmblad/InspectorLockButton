using System.Collections.Generic;

namespace ScriptFileCreation
{

    public static class CodeBlockDefaults
    {
        public static readonly CodeBlockEmpty CodeBlockEmpty = new CodeBlockEmpty();
    }

    /// <summary>
    /// An unset CodeBlock class without any content or hierarchy.
    /// </summary>
    public class CodeBlockEmpty : CodeBlock, ICodeHierarchy
    {
        public CodeBlockEmpty() : base(new List<CodeLine>())
        {
            m_Parent = this;
            m_CodeBody = this;
        }

        public override ICodeHierarchy Parent => this;
        public override ICodeHierarchy Root => this;
        public override ICodeHierarchy CodeBody => this;

        public override void AddContent(CodeLine content)
        {
        }

        public override void AppendToContent(int contentIndex, string appendText)
        {
        }

        public override void AddBody(CodeBlock scriptBody)
        {
        }

        public override string ToString() => string.Empty;
    }
}