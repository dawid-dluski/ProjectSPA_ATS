using ProjectSPA_ATS.Structures;
using ProjectSPA_ATS.Structures.AST;

namespace ProjectSPA_ATS.PKB
{
    public interface IPBKService
    {
        // Procedure API
        void AddProcedure(ProcedureNode procedure);
        List<ProcedureNode> GetProcedureList();
        ProcedureNode GetProcedureByName(string name);

        // Variable API
        void AddVariable(AssignNode variable);
        List<AssignNode> GetVariableList();
        AssignNode GetVariableByName(string name);
        AssignNode GetVariableByIndex(int index);
        int getVariableListSize();

        // Modify API
        void AddModify(Modify modify);
        List<Modify> GetModifyList();
        Modify GetModifyByIndex(int index);
        int GetModifyListSize();
    }
}