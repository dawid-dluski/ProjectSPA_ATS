using System;
using System.Collections.Generic;
using ProjectSPA_ATS.AST;
using ProjectSPA_ATS.Lexer;
using ProjectSPA_ATS.Parser;

namespace SimpleSPA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string simpleSourceCode = String.Empty;
            // Loding simple code from source file
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

            // 1. Tworzymy obiekt Lexer i wyciągamy tokeny
            var lexer = new Lexer(simpleSourceCode);
            List<Token> tokens = lexer.GetTokens(simpleSourceCode);

            Console.WriteLine("== TOKENS ==");
            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }

            // 2. Parsujemy listę tokenów, otrzymujemy obiekt AST
            var parser = new Parser(tokens);
            ProcedureNode root = parser.ParseProcedure();

            // 3. Wypisujemy AST
            Console.WriteLine("\n== AST ==");
            PrintAst(root);
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
