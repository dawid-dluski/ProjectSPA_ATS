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
            /// Config
            ConsoleEncodingHelper.SetIBM852Encoding();

            /// Services
            PKBService _pbkService = PKBService.Instance;
            ParserService _ParserService = ParserService.GetInstance(_pbkService);
            PQLService _PQLService = PQLService.GetInstance(_pbkService);

            /// Loding simple code from source file if path doesn't exist as argument
            string simpleSourceCode = FileHelper.LoadFileContent(args);

            /// Parser
            _ParserService.ParseProgram(simpleSourceCode);

            /// PQL
            _PQLService.StartListening();
        }
    }
}
