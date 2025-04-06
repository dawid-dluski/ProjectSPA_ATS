namespace ProjectSPA_ATS.Exceptions
{
    internal class ModifyNameConflictException : Exception
    {
        public ModifyNameConflictException()
        : base("A variable with the same name already exists.")
        {
        }

        public ModifyNameConflictException(string message)
        : base(message)
        {
        }
    }
}