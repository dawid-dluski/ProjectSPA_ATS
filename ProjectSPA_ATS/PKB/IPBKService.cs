using ProjectSPA_ATS.Structures;
using ProjectSPA_ATS.Structures.AST;
using System.Xml.Linq;

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
        
        // Uses API
        void AddUses(Use use);
        List<Use> GetUseList();
        Use GetUseByIndex(int index);
        int GetUseListSize();

        // Follow API
        void AddFollow(Follow follow);
        List<Follow> GetFollowList();
        Follow GetFollowByIndex(int index);
        int GetFollowListSize();

        // Parent API
        void AddParent(Parent parent);
        List<Parent> GetParentList();
        Parent GetParentByIndex(int index);
        int GetParentListSize();
    }
}