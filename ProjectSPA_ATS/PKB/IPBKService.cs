using ProjectSPA_ATS.Structures.AST;

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
