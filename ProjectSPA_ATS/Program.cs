using System;
using System.Collections.Generic;
using ProjectSPA_ATS.PQL;
using ProjectSPA_ATS.Parser;
using ProjectSPA_ATS.PKB;
using ProjectSPA_ATS.Structures.AST;
using ProjectSPA_ATS.Helpers;

namespace SimpleSPA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /// Services
            PKBService _pbkService = PKBService.Instance;
            ParserService _ParserService = ParserService.GetInstance(_pbkService);
            PQLService _PQLService = PQLService.GetInstance(_pbkService);

            /// Loding simple code from source file
            string simpleSourceCode = FileHelper.LoadFileContent(args);

            /// Parser
            _ParserService.ParseProgram(simpleSourceCode);
            Console.WriteLine("\n== AST ==");
            var root = _pbkService.GetProcedureList()[0];
            PrintAst(root);

            /// PQL
            _PQLService.StartListening();
        }

        /// <summary>
        /// Drukuje AST rekurencyjnie
        /// </summary>
        static void PrintAst(AstNode node, int indent = 0)
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
