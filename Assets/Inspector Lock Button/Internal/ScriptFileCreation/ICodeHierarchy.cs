namespace ScriptFileCreation
{
    public interface ICodeHierarchy
    {
        public ICodeHierarchy Parent { get; }
        public ICodeHierarchy Root { get; }
        public ICodeHierarchy CodeBody { get; }
    }
}
