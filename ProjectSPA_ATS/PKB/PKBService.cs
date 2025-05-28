using ProjectSPA_ATS.Exceptions;
using ProjectSPA_ATS.Structures;
using ProjectSPA_ATS.Structures.AST;

namespace ProjectSPA_ATS.PKB
{
    sealed public class PKBService : IPBKService
    {
        private static readonly PKBService _instance = new PKBService();
        public static PKBService Instance => _instance;

        private List<ProcedureNode> ProcedureList;
        private List<AssignNode> VariableList;
        private List<Modify> ModifyList;
        private List<Use> UseList;
        private List<Follow> FollowList;
        private List<Parent> ParentList;
        private List<Call> CallList;

        private PKBService()
        {
            ProcedureList = new List<ProcedureNode>();
            VariableList = new List<AssignNode>();
            ModifyList = new List<Modify>();
            UseList = new List<Use>();
            FollowList = new List<Follow>();
            ParentList = new List<Parent>();
            CallList = new List<Call>();
        }
        // Design Extractor API
        public void AddProcedure(ProcedureNode proc)
        {
            if (ProcedureList.Any(x => x.Name == proc.Name))
            {
                throw new ProcedureNameConflictException();
            }

            ProcedureList.Add(proc);
        }
        public void AddVariable(AssignNode v)
        {
            if (VariableList.Any(x => x.VarName == v.VarName))
            {
                throw new VariableNameConflictException();
            }
            VariableList.Add(v);
        }
        public void AddModify(Modify m)
        {
            ModifyList.Add(m);
        }
        public void AddCall(string caller, string callee)
            => CallList.Add(new Call(caller, callee));
        public void AddUses(Use u)
        {
            UseList.Add(u);
        }
        public void AddFollow(Follow f)
        {
            FollowList.Add(f);
        }
        public void AddParent(Parent p)
        {
            ParentList.Add(p);
        }

        // Procedure API
        public List<ProcedureNode> GetProcedureList()
        {
            return ProcedureList;
        }
        public ProcedureNode GetProcedureByName(string n)
        {
            return ProcedureList.FirstOrDefault(x => x.Name == n);
        }

        // Variable API
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
        public List<string> GetModified(int stmtId)
        {
            List<string> result = new List<string>();
            foreach (var mod in ModifyList)
            {
                if (mod.Statement == stmtId)
                    result.Add(mod.Variable);
            }
            return result;
        }
        public List<int> GetModifies(string varName)
        {
            List<int> result = new List<int>();
            foreach (var mod in ModifyList)
            {
                if (mod.Variable == varName)
                    result.Add(mod.Statement);
            }
            return result;
        }
        public bool IsProcModifies(string proc, string var)
        {
            return GetProcModifies(proc).Contains(var);
        }
        public List<string> GetProcModifies(string proc)
        {
            HashSet<string> modifiedVars = new HashSet<string>();
            HashSet<string> visitedProcs = new HashSet<string>();
            Stack<string> stack = new Stack<string>();
            visitedProcs.Add(proc);
            stack.Push(proc);

            while (stack.Count > 0)
            {
                string currentProc = stack.Pop();
                ProcedureNode procNode = ProcedureList.FirstOrDefault(p => p.Name == currentProc);
                if (procNode == null) continue;

                List<int> stmtIds = new List<int>();
                GatherStatementIds(procNode.Statements, stmtIds);

                foreach (int stmtId in stmtIds)
                {
                    foreach (var mod in ModifyList)
                    {
                        if (mod.Statement == stmtId)
                            modifiedVars.Add(mod.Variable);
                    }
                }

                foreach (var call in CallList)
                {
                    if (call.CallerProc == currentProc && !visitedProcs.Contains(call.CalleeProc))
                    {
                        visitedProcs.Add(call.CalleeProc);
                        stack.Push(call.CalleeProc);
                    }
                }
            }

            return modifiedVars.ToList();
        }
        // Use API
        public List<string> GetUsed(int stmtId)
        {
            List<string> result = new List<string>();
            foreach (var use in UseList)
            {
                if (use.StatementId == stmtId)
                    result.Add(use.VariableName);
            }
            return result;
        }
        public List<int> GetUses(string varName)
        {
            List<int> result = new List<int>();
            foreach (var use in UseList)
            {
                if (use.VariableName == varName)
                    result.Add(use.StatementId);
            }
            return result;
        }
        public bool IsProcUses(string proc, string var)
        {
            return GetProcUses(proc).Contains(var);
        }
        public List<string> GetProcUses(string proc)
        {
            HashSet<string> usedVars = new HashSet<string>();
            HashSet<string> visitedProcs = new HashSet<string>();
            Stack<string> stack = new Stack<string>();
            visitedProcs.Add(proc);
            stack.Push(proc);

            while (stack.Count > 0)
            {
                string currentProc = stack.Pop();
                ProcedureNode procNode = ProcedureList.FirstOrDefault(p => p.Name == currentProc);
                if (procNode == null) continue;

                List<int> stmtIds = new List<int>();
                GatherStatementIds(procNode.Statements, stmtIds);

                foreach (int stmtId in stmtIds)
                {
                    foreach (var use in UseList)
                    {
                        if (use.StatementId == stmtId)
                            usedVars.Add(use.VariableName);
                    }
                }

                foreach (var call in CallList)
                {
                    if (call.CallerProc == currentProc && !visitedProcs.Contains(call.CalleeProc))
                    {
                        visitedProcs.Add(call.CalleeProc);
                        stack.Push(call.CalleeProc);
                    }
                }
            }

            return usedVars.ToList();
        }
        // Parent API (Zweryfikowane, 100% gwaracji nie daje)
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
        public List<int> GetParentedBy(int stmt)
        {
            var parent = ParentList.FirstOrDefault(p => p.ChildStmtId == stmt)?.ParentStmtId;
            return parent is null ? new List<int>() : new List<int> { parent.Value };
        }
        public List<int> GetParentedStarBy(int stmt)
        {
            var result = new List<int>();
            var cur = ParentList.FirstOrDefault(p => p.ChildStmtId == stmt)?.ParentStmtId;
            while (cur is not null)
            {
                result.Add(cur.Value);
                cur = ParentList.FirstOrDefault(p => p.ChildStmtId == cur)?.ParentStmtId;
            }
            return result;
        }
        public bool IsParent(int p, int c) 
            => ParentList.Any(x => x.ParentStmtId == p && x.ChildStmtId == c);
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
            var visited = new HashSet<int>();
            var q = new Queue<int>(GetChildren(parent));

            while (q.Count > 0)
            {
                var c = q.Dequeue();
                if (visited.Add(c))
                {
                    yield return c;
                    foreach (var ch in GetChildren(c))
                        q.Enqueue(ch);
                }
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
        
        // Follow API (Zweryfikowane, 100% gwaracji nie daje)
        public List<Follow> GetFollowList()
            => FollowList.Distinct().ToList();
        public Follow GetFollowByIndex(int index)
        {
            if (index < 0 || index >= FollowList.Count)
                throw new IndexOutOfRangeException($"Follow index {index} out of range.");
            return FollowList[index];
        }
        public int GetFollowListSize()
            => FollowList.Distinct().Count();
        public List<int> GetFollowedBy(int stmt) =>
            FollowList.Where(f => f.FollowingStmtId == stmt)
                      .Select(f => f.PrecedingStmtId)
                      .Distinct()
                      .ToList();
        public List<int> GetFollows(int stmt) =>
            FollowList.Where(f => f.PrecedingStmtId == stmt)
                      .Select(f => f.FollowingStmtId)
                      .Distinct()
                      .ToList();
        public List<int> GetFollowedStarBy(int stmt)
        {
            var result = new HashSet<int>();
            var stack = new Stack<int>(GetFollowedBy(stmt));

            while (stack.Count > 0)
            {
                var prev = stack.Pop();
                if (result.Add(prev))
                    foreach (var p in GetFollowedBy(prev))
                        stack.Push(p);
            }
            return result.ToList();
        }
        public List<int> GetFollowsStar(int stmt)
        {
            var result = new HashSet<int>();
            var stack = new Stack<int>(GetFollows(stmt));

            while (stack.Count > 0)
            {
                var next = stack.Pop();
                if (result.Add(next))
                    foreach (var n in GetFollows(next))
                        stack.Push(n);
            }
            return result.ToList();
        }
        public bool IsFollows(int s1, int s2) =>
           FollowList.Any(f => f.PrecedingStmtId == s1 && f.FollowingStmtId == s2);
        public int? GetImmediateFollower(int s1) =>
            FollowList.FirstOrDefault(f => f.PrecedingStmtId == s1)?.FollowingStmtId;
        public bool IsFollowsStar(int s1, int s2) => GetAllFollowers(s1).Contains(s2);
        public IEnumerable<int> GetAllFollowers(int s1)
        {
            var cur = GetImmediateFollower(s1);
            while (cur is not null)
            {
                yield return cur.Value;
                cur = GetImmediateFollower(cur.Value);
            }
        }
        public IEnumerable<int> GetAllPreceders(int s2)
        {
            var stack = new Stack<int>(FollowList.Where(f => f.FollowingStmtId == s2)
                                                 .Select(f => f.PrecedingStmtId));
            var seen = new HashSet<int>();
            while (stack.Count > 0)
            {
                var prev = stack.Pop();
                if (seen.Add(prev))
                {
                    yield return prev;
                    foreach (var p in FollowList.Where(f => f.FollowingStmtId == prev)
                                                .Select(f => f.PrecedingStmtId))
                        stack.Push(p);
                }
            }
        }
        // Calls API (Zweryfikowane, 100% gwaracji nie daje)
        public bool IsCalls(string caller, string callee) 
            => CallList.Any(c => c.CallerProc == caller && c.CalleeProc == callee);
        public bool IsCallsStar(string caller, string callee) 
            => GetCallees(caller, true).Contains(callee);
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

        // Helpers
        private void GatherStatementIds(List<StatementNode> statements, List<int> result)
        {
            foreach (StatementNode stmt in statements)
            {
                result.Add(stmt.StatementId);
                if (stmt is IfNode ifNode)
                {
                    GatherStatementIds(ifNode.ThenBody, result);
                    GatherStatementIds(ifNode.ElseBody, result);
                }
                else if (stmt is WhileNode whileNode)
                {
                    GatherStatementIds(whileNode.Body, result);
                }
            }
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