using System;
using System.Collections.Generic;
using ProjectSPA_ATS.AST;

using ProjectSPA_ATS.Parser;
using ProjectSPA_ATS.PKB;

namespace SimpleSPA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /// Services
            PKBService _PKBService = PKBService.Instance;
            ParserService _ParserService = ParserService.GetInstance(_PKBService);

            // Loding simple code from source file
            string simpleSourceCode = String.Empty;
            string fileName = args.Length > 0 ? args[0] : "SimpleExample.txt";
            if (File.Exists(fileName))
            {
                try
                {
                    simpleSourceCode = File.ReadAllText(fileName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Load file error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"File does not exist.");
                return;
            }

            /// Parser
            _ParserService.ParseProgram(simpleSourceCode);
            Console.WriteLine("\n== AST ==");
            var root = _PKBService.GetProcedureList()[0];
            PrintAst(root);

            /// PQL
            /// 
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
