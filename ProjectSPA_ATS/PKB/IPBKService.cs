using ProjectSPA_ATS.Structures;
using ProjectSPA_ATS.Structures.AST;

namespace ProjectSPA_ATS.PKB
{
    public interface IPBKService
    {
        // Design Extractor API (Zweryfikowane, 100% gwaracji nie daje)
        void AddProcedure(ProcedureNode procedure);
        void AddVariable(AssignNode variable);
        void AddModify(Modify modify);
        void AddCall(string caller, string callee);
        void AddUses(Use use);
        void AddFollow(Follow follow);
        void AddParent(Parent parent);

        // Procedure API (Zweryfikowane, 100% gwaracji nie daje)
        List<ProcedureNode> GetProcedureList();
        ProcedureNode GetProcedureByName(string name);

        // Variable API
        List<AssignNode> GetVariableList();
        AssignNode GetVariableByName(string name);
        AssignNode GetVariableByIndex(int index);
        int getVariableListSize();
        string GetVarName(AssignNode node);
        string GetVarName(Modify modify);
        string GetVarName(Use use);

        // Modify API
        public List<string> GetModified(int stmtId); // Działa
        public List<int> GetModifies(string varName); // Działa
        public bool IsProcModifies(string proc, string var); // Niedziała
        public List<string> GetProcModifies(string proc); // Niedziała

        // Use API
        public List<string> GetUsed(int stmtId); // Działa
        public List<int> GetUses(string varName); // Działa
        public bool IsProcUses(string proc, string var); // Niedziała
        public List<string> GetProcUses(string proc); // Niedziała

        // Parent API (Zweryfikowane, 100% gwaracji nie daje)
        List<Parent> GetParentList();
        Parent GetParentByIndex(int index);
        int GetParentListSize();
        List<int> GetParentedStarBy(int stmtIndex);
        List<int> GetParentedBy(int stmtIndex);
        bool IsParent(int p, int c);
        bool IsParentStar(int anc, int desc);
        IEnumerable<int> GetChildren(int parent);
        IEnumerable<int> GetDescendants(int parent);
        int? GetParent(int child);
        IEnumerable<int> GetAncestors(int stmt);

        // Follow API (Zweryfikowane, 100% gwaracji nie daje)
        List<Follow> GetFollowList();
        Follow GetFollowByIndex(int index);
        int GetFollowListSize();
        List<int> GetFollowedStarBy(int stmtIndex);
        List<int> GetFollowedBy(int stmtIndex);
        List<int> GetFollowsStar(int stmtIndex);
        List<int> GetFollows(int stmtIndex);
        bool IsFollows(int s1, int s2);
        bool IsFollowsStar(int s1, int s2);
        int? GetImmediateFollower(int s1);
        IEnumerable<int> GetAllFollowers(int s1);
        IEnumerable<int> GetAllPreceders(int s2);

        // Calls API (Zweryfikowane, 100% gwaracji nie daje)
        bool IsCalls(string caller, string callee);
        bool IsCallsStar(string caller, string callee);
        IEnumerable<string> GetCallees(string caller, bool transitive = false);
        IEnumerable<string> GetCallers(string callee, bool transitive = false);
    }
}