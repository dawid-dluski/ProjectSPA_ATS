namespace ProjectSPA_ATS.Exceptions
{
    class ProcedureNameConflictException : Exception
    {
        public ProcedureNameConflictException()
        : base("A procedure with the same name already exists.")
        {
        }

        public ProcedureNameConflictException(string message)
        : base(message)
        {
        }
    }
}
