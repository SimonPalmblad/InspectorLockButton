

namespace ScriptFileCreation
{
    /// <summary>
    /// A class that stores a string representing a line of code, with an optional line ending string. Indentation of the code line can be modified by accessing the public int <see cref="IndentationAmount"/>.
    /// </summary>
    /// <param name="content">The string line of code to store.</param>
    /// <param name="lineEnding">Optional string line ending.</param>
    /// 
    public class CodeLine
    {
        private readonly string m_Content;
        public string LineEnding;
        public int IndentationAmount = 0;

        public CodeLine(string content, string lineEnding = "")
        {
            m_Content = content;
            this.LineEnding = lineEnding;
        }

        public override string ToString()
        {
            return StringHelpers.LineIndentation(IndentationAmount) + m_Content + LineEnding;
        }
    }
}