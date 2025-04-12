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
        string GetVarName(AssignNode node);
        string GetVarName(Modify modify);
        string GetVarName(Use use);

        // Modify API
        void AddModify(Modify modify);
        List<Modify> GetModifyList();
        Modify GetModifyByIndex(int index);
        int GetModifyListSize();
        string GetModified(int stmtIndex);
        List<int> GetModifies(string varName);

        // Use API
        void AddUses(Use use);
        List<Use> GetUseList();
        Use GetUseByIndex(int index);
        int GetUseListSize();
        string GetUsed(int stmtIndex);
        List<int> GetUses(string varName);

        // Follow API
        void AddFollow(Follow follow);
        List<Follow> GetFollowList();
        Follow GetFollowByIndex(int index);
        int GetFollowListSize();
        List<int> GetFollowedStarBy(int stmtIndex);
        List<int> GetFollowedBy(int stmtIndex);
        List<int> GetFollowsStar(int stmtIndex);
        List<int> GetFollows(int stmtIndex);

        // Parent API
        void AddParent(Parent parent);
        List<Parent> GetParentList();
        Parent GetParentByIndex(int index);
        int GetParentListSize();
        List<int> GetParentedStarBy(int stmtIndex);
        List<int> GetParentedBy(int stmtIndex);
    }
}