using ProjectSPA_ATS.Structures;
using ProjectSPA_ATS.Structures.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSPA_ATS.PKB
{
    public class DesignExtractor
    {
        private readonly IPBKService _pbk;
        private int stmtCounter = 1;

        public DesignExtractor(IPBKService pbkService)
        {
            _pbk = pbkService;
        }
        /// <summary>
        /// Główna metoda do wyciągania relacji z AST. 
        /// Na start parsujemy listę instrukcji w ProcedureNode.
        /// </summary>
        /// <param name="procedure">Obiekt AST reprezentujący procedurę</param>
        public void Extract(ProcedureNode procedure)
        {
            // Przetwarzamy listę instrukcji w tej procedurze
            ProcessStatementList(procedure.Statements, parentStmtId: null);

            // Po przetworzeniu wszystkich instrukcji – wypisz relacje w konsoli
            PrintAllRelations();
        }

        /// <summary>
        /// Rekurencyjnie przetwarza listę instrukcji (stmtLst)
        /// Dodaje relacje Follow, Parent, Uses, Modify.
        /// </summary>
        private void ProcessStatementList(List<StatementNode> stmts, int? parentStmtId)
        {
            for (int i = 0; i < stmts.Count; i++)
            {
                // Pobieramy aktualną instrukcję
                StatementNode stmt = stmts[i];
                // Przydzielamy jej unikalny numer (stmt#)
                int currentStmtId = stmtCounter++;

                // 1. Dodaj relację Follow
                if (i > 0) 
                {
                    _pbk.AddFollow(new Follow(currentStmtId - 1, currentStmtId));
                }

                // 2. Dodaj relację Parent
                if (parentStmtId.HasValue)
                {
                    _pbk.AddParent(new Parent (parentStmtId.Value, currentStmtId));
                }

                // 3. Dodaj relacje Uses/Modify w zależności od typu instrukcji
                if (stmt is AssignNode assign)
                {
                    // Modify
                    _pbk.AddModify(new Modify (currentStmtId, assign.VarName));

                    // Uses
                    List<string> usedVars = CollectUsedVariables(assign.Expression);
                    foreach (var uv in usedVars)
                    {
                        _pbk.AddUses(new Use (currentStmtId, uv));
                    }
                }
                else if (stmt is WhileNode whileNode)
                {
                    _pbk.AddUses(new Use(currentStmtId, whileNode.ConditionVar));

                    ProcessStatementList(whileNode.Body, currentStmtId);
                }
            }
        }

        /// <summary>
        /// Pomocnicza metoda zbiera wszystkie zmienne użyte w wyrażeniu (ExpressionNode)
        /// np. w BinaryOpNode, VarRefNode, ConstNode itd.
        /// </summary>
        private List<string> CollectUsedVariables(ExpressionNode expr)
        {
            var result = new List<string>();
            switch (expr)
            {
                case VarRefNode varRef:
                    result.Add(varRef.VarName);
                    break;

                case BinaryOpNode binOp:
                    result.AddRange(CollectUsedVariables(binOp.Left));
                    result.AddRange(CollectUsedVariables(binOp.Right));
                    break;

            }
            return result;
        }

        /// <summary>
        /// Ta metoda wypisuje wszystkie relacje zgromadzone w PKB
        /// (Follow, Parent, Uses, Modify).
        /// </summary>
        private void PrintAllRelations()
        {
            var followList = (_pbk as PKBService)?.GetFollowAll();
            var parentList = (_pbk as PKBService)?.GetParentAll();
            var usesList = (_pbk as PKBService)?.GetUsesAll();
            var modifyList = (_pbk as PKBService)?.GetModifyAll();

            Console.WriteLine("\n=== PKB RELATIONS ===");

            if (followList is not null)
            {
                Console.WriteLine("\nFOLLOWS:");
                foreach (var f in followList)
                {
                    Console.WriteLine($"  Follows({f.PrecedingStmtId}, {f.FollowingStmtId})");
                }
            }

            if (parentList is not null)
            {
                Console.WriteLine("\nPARENT:");
                foreach (var p in parentList)
                {
                    Console.WriteLine($"  Parent({p.ParentStmtId}, {p.ChildStmtId})");
                }
            }

            if (usesList is not null)
            {
                Console.WriteLine("\nUSES:");
                foreach (var u in usesList)
                {
                    Console.WriteLine($"  Uses(stmt#{u.StatementId}, \"{u.VariableName}\")");
                }
            }

            if (modifyList is not null)
            {
                Console.WriteLine("\nMODIFIES:");
                foreach (var m in modifyList)
                {
                    Console.WriteLine($"  Modifies(stmt#{m.Statement}, \"{m.Variable}\")");
                }
            }
            Console.WriteLine("\n=== END PKB RELATIONS ===");
        }
    }
}