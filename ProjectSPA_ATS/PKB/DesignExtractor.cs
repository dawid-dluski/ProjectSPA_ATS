using ProjectSPA_ATS.Helpers;
using ProjectSPA_ATS.Structures;
using ProjectSPA_ATS.Structures.AST;

namespace ProjectSPA_ATS.PKB
{
    public class DesignExtractor
    {
        private readonly IPBKService _pkb;
        private int _stmtCounter;          // numeracja stmt#
        private string _currentProc;       // nazwa procedury – do relacji Calls
        private static int _globalStmtCounter = 1;

        public DesignExtractor(IPBKService pkb) => _pkb = pkb;

        public void Extract(ProcedureNode proc)
        {
            _currentProc = proc.Name;
            _stmtCounter = 1;

            ProcessStmtList(proc.Statements, parentId: null);
        }

        private void ProcessStmtList(List<StatementNode> list, int? parentId)
        {
            int? prevId = null;   // do relacji Follow

            foreach (var stmt in list)
            {
                int id = _globalStmtCounter++;        
                stmt.StatementId = id;
                if (prevId.HasValue)
                    _pkb.AddFollow(new Follow(prevId.Value, id));

                if (parentId.HasValue)
                    _pkb.AddParent(new Parent(parentId.Value, id));

                switch (stmt)
                {
                    case AssignNode a:
                        HandleAssign(a, id);
                        break;

                    case WhileNode w:
                        HandleWhile(w, id);
                        break;

                    case IfNode ifn:
                        HandleIf(ifn, id);
                        break;

                    case CallNode call:
                        HandleCall(call, id);
                        break;
                }

                prevId = id;      // obecny staje się poprzednim
            }
        }

        /* --------------------  ASSIGN  ---------------------------- */
        private void HandleAssign(AssignNode node, int id)
        {
            _pkb.AddModify(new Modify(id, node.VarName));

            foreach (var v in CollectUsedVars(node.Expression))
                _pkb.AddUses(new Use(id, v));
        }

        /* --------------------  WHILE  ----------------------------- */
        private void HandleWhile(WhileNode node, int id)
        {
            _pkb.AddUses(new Use(id, node.ConditionVar));
            ProcessStmtList(node.Body, parentId: id);
        }

        /* --------------------  IF / ELSE  ------------------------- */
        private void HandleIf(IfNode node, int id)
        {
            _pkb.AddUses(new Use(id, node.ConditionVar));
            ProcessStmtList(node.ThenBody, id);
            ProcessStmtList(node.ElseBody, id);
        }

        /* --------------------  CALL  ------------------------------ */
        private void HandleCall(CallNode node, int id)
        {
            _pkb.AddCall(_currentProc, node.Callee);

            var visited = new HashSet<string>();
            AddModifiesFromCallees(id, node.Callee, visited);
        }



        private void AddModifiesFromCallees(int callStmtId, string callee, HashSet<string> visited)
        {
            if (!visited.Add(callee)) return; // zapobiegamy cyklom

            var proc = _pkb.GetProcedureByName(callee);
            if (proc == null) return;

            var assigns = Flatten(proc.Statements).OfType<AssignNode>();
            foreach (var a in assigns)
                _pkb.AddModify(new Modify(callStmtId, a.VarName));

            var nestedCalls = Flatten(proc.Statements).OfType<CallNode>();
            foreach (var nested in nestedCalls)
                AddModifiesFromCallees(callStmtId, nested.Callee, visited);
        }

        private IEnumerable<string> CollectUsedVars(ExpressionNode expr)
        {
            switch (expr)
            {
                case VarRefNode v: return new[] { v.VarName };
                case ConstNode _: return Enumerable.Empty<string>();
                case BinaryOpNode b:
                    return CollectUsedVars(b.Left).Concat(CollectUsedVars(b.Right));
                default:
                    return Enumerable.Empty<string>();
            }
        }


        // do pobrania zagniezdzonych assign w wywolanej procedurze
        private IEnumerable<StatementNode> Flatten(IEnumerable<StatementNode> stmts)
        {
            foreach (var s in stmts)
            {
                yield return s;

                switch (s)
                {
                    case WhileNode w:
                        foreach (var inner in Flatten(w.Body))
                            yield return inner;
                        break;

                    case IfNode i:
                        foreach (var inner in Flatten(i.ThenBody))
                            yield return inner;
                        foreach (var inner in Flatten(i.ElseBody))
                            yield return inner;
                        break;
                }
            }
        }


    }
}
