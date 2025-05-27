using ProjectSPA_ATS.Structures.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSPA_ATS.Helpers
{
    public class ASTHelper
    {
        public static void PrintAst(AstNode node, int indent = 0)
        {
            string pad = new string(' ', indent * 2);

            switch (node)
            {
                case ProcedureNode proc:
                    Console.WriteLine($"{pad}Procedure '{proc.Name}':");
                    foreach (var stmt in proc.Statements)
                    {
                        PrintAst(stmt, indent + 1);
                    }
                    break;

                case AssignNode assign:
                    Console.WriteLine($"{pad}Assign: {assign.VarName} = ");
                    PrintAst(assign.Expression, indent + 1);
                    break;

                case WhileNode wh:
                    Console.WriteLine($"{pad}While: {wh.ConditionVar}");
                    Console.WriteLine($"{pad}{{");
                    foreach (var stmt in wh.Body)
                    {
                        PrintAst(stmt, indent + 2);
                    }
                    Console.WriteLine($"{pad}}}");
                    break;

                case IfNode ifNode:
                    Console.WriteLine($"{pad}If: {ifNode.ConditionVar}");
                    Console.WriteLine($"{pad}Then {{");
                    foreach (var stmt in ifNode.ThenBody)
                    {
                        PrintAst(stmt, indent + 2);
                    }
                    Console.WriteLine($"{pad}}}");
                    Console.WriteLine($"{pad}else {{");
                    foreach (var stmt in ifNode.ElseBody)
                    {
                        PrintAst(stmt, indent + 2);
                    }
                    Console.WriteLine($"{pad}}}");
                    break;

                case CallNode call:
                    Console.WriteLine($"{pad}Call: {call.Callee}");
                    break;

                case BinaryOpNode binOp:
                    Console.WriteLine($"{pad}BinaryOp '{binOp.Operator}':");
                    Console.WriteLine($"{pad}  Left:");
                    PrintAst(binOp.Left, indent + 2);
                    Console.WriteLine($"{pad}  Right:");
                    PrintAst(binOp.Right, indent + 2);
                    break;

                case VarRefNode varRef:
                    Console.WriteLine($"{pad}VarRef '{varRef.VarName}'");
                    break;

                case ConstNode constNode:
                    Console.WriteLine($"{pad}Const {constNode.Value}");
                    break;

                default:
                    Console.WriteLine($"{pad}[Nieznany węzeł AST typu: {node?.GetType().Name}]");
                    break;
            }
        }
    }

}
