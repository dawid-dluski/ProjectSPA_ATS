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
        private List<Call> CallList = new();
        // Procedure API
        public void AddProcedure(ProcedureNode proc)
        {
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

        public bool IsParent(int p, int c) => ParentList.Any(x => x.ParentStmtId == p && x.ChildStmtId == c);
        public bool IsParentStar(int anc, int desc)
        {
            int? cur = desc;
            while (cur is not null)
            {
                cur = ParentList.FirstOrDefault(x => x.ChildStmtId == cur)?.ParentStmtId;
                if (cur == anc) return true;
            }
            return false;
        }
        public IEnumerable<int> GetChildren(int parent) =>
            ParentList.Where(p => p.ParentStmtId == parent).Select(p => p.ChildStmtId);
        public IEnumerable<int> GetDescendants(int parent)
        {
            var q = new Queue<int>(GetChildren(parent));
            while (q.Count > 0)
            {
                var c = q.Dequeue();
                yield return c;
                foreach (var ch in GetChildren(c)) q.Enqueue(ch);
            }
        }
        public int? GetParent(int child) =>
            ParentList.FirstOrDefault(p => p.ChildStmtId == child)?.ParentStmtId;
        public IEnumerable<int> GetAncestors(int stmt)
        {
            var cur = GetParent(stmt);
            while (cur is not null)
            {
                yield return cur.Value;
                cur = GetParent(cur.Value);
            }
        }







        public void AddCall(string caller, string callee) => CallList.Add(new Call(caller, callee));
        public List<Follow> GetFollowAll() => FollowList;
        public List<Parent> GetParentAll() => ParentList;
        public List<Use> GetUsesAll() => UseList;
        public List<Modify> GetModifyAll() => ModifyList;

        public IReadOnlyList<Call> GetCallsAll() => CallList;

        /* ==== Modifies / Uses – procedury ==== */

        public bool IsProcModifies(string proc, string var) => GetProcModifies(proc).Contains(var);
        public bool IsProcUses(string proc, string var) => GetProcUses(proc).Contains(var);

        public IEnumerable<string> GetProcModifies(string proc) =>
            CollectModUses(proc, true);

        public IEnumerable<string> GetProcUses(string proc) =>
            CollectModUses(proc, false);

        // Calls API (Zweryfikowane)
        public bool IsCalls(string caller, string callee) =>
            CallList.Any(c => c.CallerProc == caller && c.CalleeProc == callee);
        public IEnumerable<string> GetCallees(string caller, bool transitive = false)
        {
            if (!transitive) return CallList.Where(c => c.CallerProc == caller).Select(c => c.CalleeProc);
            return DfsForward(caller, new()).Where(p => p != caller);
        }
        public IEnumerable<string> GetCallers(string callee, bool transitive = false)
        {
            if (!transitive) return CallList.Where(c => c.CalleeProc == callee).Select(c => c.CallerProc);
            return DfsBackward(callee, new()).Where(p => p != callee);
        }
        public bool IsCallsStar(string caller, string callee) =>
            GetCallees(caller, true).Contains(callee);

        // Helpers
        private IEnumerable<string> CollectModUses(string proc, bool wantMod)
        {
            var visited = new HashSet<string>();
            var vars = new HashSet<string>();

            void DFS(string p)
            {
                if (!visited.Add(p)) return;

                // bezpośrednie
                var stmtIds = ProcedureList.First(pr => pr.Name == p)
                                            .Statements.Select(s => s.StatementId);

                if (wantMod)
                    vars.UnionWith(ModifyList.Where(m => stmtIds.Contains(m.Statement))
                                             .Select(m => m.Variable));
                else
                    vars.UnionWith(UseList.Where(u => stmtIds.Contains(u.StatementId))
                                          .Select(u => u.VariableName));

                // kolejne procedury
                foreach (var callee in GetCallees(p))
                    DFS(callee);
            }
            DFS(proc);
            return vars;
        }
        private IEnumerable<string> DfsForward(string start, HashSet<string> visited)
        {
            if (!visited.Add(start)) yield break;
            foreach (var callee in CallList.Where(c => c.CallerProc == start).Select(c => c.CalleeProc))
            {
                yield return callee;
                foreach (var deep in DfsForward(callee, visited)) yield return deep;
            }
        }
        private IEnumerable<string> DfsBackward(string start, HashSet<string> visited)
        {
            if (!visited.Add(start)) yield break;
            foreach (var caller in CallList.Where(c => c.CalleeProc == start).Select(c => c.CallerProc))
            {
                yield return caller;
                foreach (var deep in DfsBackward(caller, visited)) yield return deep;
            }
        }
    }
}