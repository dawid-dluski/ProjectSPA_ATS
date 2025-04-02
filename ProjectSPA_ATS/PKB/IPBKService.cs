using ProjectSPA_ATS.AST;

namespace ProjectSPA_ATS.PKB
{
    public interface IPBKService
    {
        // Procedure API
        void AddProcedure(ProcedureNode procedure);

        List<ProcedureNode> GetProcedureList();
        ProcedureNode GetProcedureByName(string name);
    }
}
