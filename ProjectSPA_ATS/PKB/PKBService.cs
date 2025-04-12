using ProjectSPA_ATS.Exceptions;
using ProjectSPA_ATS.Structures;
using ProjectSPA_ATS.Structures.AST;

namespace ProjectSPA_ATS.PKB
{
    sealed public class PKBService : IPBKService
    {
        private static readonly PKBService _instance = new PKBService();
        private PKBService() 
        {
            ModifyList = new List<Modify>();
            UseList = new List<Use>();
            FollowList = new List<Follow>();
            ParentList = new List<Parent>();
        }

        public static PKBService Instance => _instance;
        
        private List<ProcedureNode> ProcedureList = new List<ProcedureNode>();
        private List<AssignNode> VariableList;
        private List<Modify> ModifyList;
        private List<Use> UseList;
        private List<Follow> FollowList;
        private List<Parent> ParentList;

        // Procedure API
        public void AddProcedure(ProcedureNode proc) {
            if (ProcedureList.Any(x => x.Name == proc.Name))
            {
                throw new ProcedureNameConflictException();
            }

            ProcedureList.Add(proc);
        }
        public List<ProcedureNode> GetProcedureList()
        {
            return ProcedureList;
        }
        public ProcedureNode GetProcedureByName(string n) 
        {
            return ProcedureList.FirstOrDefault(x => x.Name == n);
        }
        
        // Variable API
        public void AddVariable(AssignNode v)
        {
            if (VariableList.Any(x => x.VarName == v.VarName))
            {
                throw new VariableNameConflictException();
            }
            VariableList.Add(v);
        }
        public List<AssignNode> GetVariableList()
        {
            return VariableList;
        }
        public AssignNode GetVariableByName(string n)
        {
            return VariableList.FirstOrDefault(x => x.VarName == n);
        }
        public AssignNode GetVariableByIndex(int i)
        {
            if (i < 0 || i >= VariableList.Count)
            {
                throw new IndexOutOfRangeException("Index out of range");
            }
            return VariableList[i];
        }
        public int getVariableListSize()
        {
            return VariableList.Count;
        }
        public string GetVarName(AssignNode node)
        {
            return node.VarName;
        }
        public string GetVarName(Modify modify)
        {
            return modify.Variable;
        }
        public string GetVarName(Use use)
        {
            return use.VariableName;
        }

        // Modify API
        public void AddModify(Modify m)
        {
            /*if (ModifyList.Any(x => x.ModifyVar == m.ModifyVar))
            {
                throw new ModifyNameConflictException();
            }*/
            ModifyList.Add(m);
        }
        public List<Modify> GetModifyList()
        {
            return ModifyList;
        }
        public Modify GetModifyByIndex(int i)
        {
            if (i < 0 || i >= ModifyList.Count)
            {
                throw new IndexOutOfRangeException("Index out of range");
            }
            return ModifyList[i];
        }
        public int GetModifyListSize()
        {
            return ModifyList.Count;
        }
        public string GetModified(int stmtIndex)
        {
            return ModifyList
                .Where(m => m.Statement == stmtIndex)
                .Select(m => m.Variable)
                .FirstOrDefault();
        }
        public List<int> GetModifies(string varName)
        {
            return ModifyList
                .Where(m => m.Variable == varName)
                .Select(m => m.Statement)
                .ToList();
        }

        // Follow API
        public void AddFollow(Follow f)
        {
            FollowList.Add(f);
        }
        public List<Follow> GetFollowList()
        {
            return FollowList;
        }
        public Follow GetFollowByIndex(int i)
        {
            if (i < 0 || i >= FollowList.Count)
            {
                throw new IndexOutOfRangeException("Index out of range");
            }
            return FollowList[i];
        }
        public int GetFollowListSize()
        {
            return FollowList.Count;
        }
        public List<int> GetFollowedStarBy(int stmtIndex)
        {
            var result = new List<int>();
            var visited = new HashSet<int>();
            var current = stmtIndex;

            while (true)
            {
                var follow = FollowList.FirstOrDefault(f => f.PrecedingStmtId == current);
                if (follow == null || visited.Contains(follow.FollowingStmtId))
                    break;
                result.Add(follow.FollowingStmtId);
                visited.Add(follow.FollowingStmtId);
                current = follow.FollowingStmtId;
            }

            return result;
        }
        public List<int> GetFollowedBy(int stmtIndex)
        {
            return FollowList
                .Where(f => f.PrecedingStmtId == stmtIndex)
                .Select(f => f.FollowingStmtId)
                .ToList();
        }
        public List<int> GetFollowsStar(int stmtIndex)
        {
            var result = new List<int>();
            var visited = new HashSet<int>();
            var current = stmtIndex;

            while (true)
            {
                var follow = FollowList.FirstOrDefault(f => f.FollowingStmtId == current);
                if (follow == null || visited.Contains(follow.PrecedingStmtId))
                    break;
                result.Add(follow.PrecedingStmtId);
                visited.Add(follow.PrecedingStmtId);
                current = follow.PrecedingStmtId;
            }

            return result;
        }
        public List<int> GetFollows(int stmtIndex)
        {
            return FollowList
                .Where(f => f.FollowingStmtId == stmtIndex)
                .Select(f => f.PrecedingStmtId)
                .ToList();
        }

        // Use API
        public void AddUses(Use u)
        {
            UseList.Add(u);
        }
        public List<Use> GetUseList()
        {
            return UseList;
        }
        public Use GetUseByIndex(int i)
        {
            if (i < 0 || i >= UseList.Count)
            {
                throw new IndexOutOfRangeException("Index out of range");
            }
            return UseList[i];
        }
        public int GetUseListSize()
        {
            return UseList.Count;
        }
        public string GetUsed(int stmtIndex)
        {
            return UseList
                .Where(u => u.StatementId == stmtIndex)
                .Select(u => u.VariableName)
                .FirstOrDefault();
        }
        public List<int> GetUses(string varName)
        {
            return UseList
                .Where(u => u.VariableName == varName)
                .Select(u => u.StatementId)
                .ToList();
        }

        // Parent API
        public void AddParent(Parent p)
        {
            ParentList.Add(p);
        }
        public List<Parent> GetParentList()
        {
            return ParentList;
        }
        public Parent GetParentByIndex(int i)
        {
            if (i < 0 || i >= ParentList.Count)
            {
                throw new IndexOutOfRangeException("Index out of range");
            }
            return ParentList[i];
        }
        public int GetParentListSize()
        {
            return ParentList.Count;
        }
        public List<int> GetParentedStarBy(int stmtIndex)
        {
            var result = new List<int>();
            var queue = new Queue<int>();
            var children = ParentList
                .Where(p => p.ParentStmtId == stmtIndex)
                .Select(p => p.ChildStmtId)
                .ToList();

            foreach (var child in children)
                queue.Enqueue(child);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                result.Add(current);

                var nested = ParentList
                    .Where(p => p.ParentStmtId == current)
                    .Select(p => p.ChildStmtId);

                foreach (var c in nested)
                    queue.Enqueue(c);
            }

            return result;
        }
        public List<int> GetParentedBy(int stmtIndex)
        {
            return ParentList
                .Where(p => p.ParentStmtId == stmtIndex)
                .Select(p => p.ChildStmtId)
                .ToList();
        }

        public List<Follow> GetFollowAll() => FollowList;
        public List<Parent> GetParentAll() => ParentList;
        public List<Use> GetUsesAll() => UseList;
        public List<Modify> GetModifyAll() => ModifyList;
    }
}