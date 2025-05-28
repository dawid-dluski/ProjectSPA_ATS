using ProjectSPA_ATS.Structures;
using ProjectSPA_ATS.Structures.AST;

namespace ProjectSPA_ATS.PKB
{
    public class DesignExtractor
    {
        private readonly IPBKService _pkb;
        private int _stmtCounter;          // numeracja stmt#
        private string _currentProc;       // nazwa procedury – do relacji Calls

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
                int id = _stmtCounter++;          // nowy stmt#
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
            _pkb.AddCall(_currentProc, node.Callee);          // Calls rel
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
    }
}
