namespace ProjectSPA_ATS.Exceptions
{
    internal class VariableNameConflictException : Exception
    {
        public VariableNameConflictException()
        : base("A variable with the same name already exists.")
        {
        }

        public VariableNameConflictException(string message)
        : base(message)
        {
        }
    }
}