using ProjectSPA_ATS.Parser;
using ProjectSPA_ATS.PKB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectSPA_ATS.LexicalAnalysis;
using ProjectSPA_ATS.PQL.Models;

namespace ProjectSPA_ATS.PQL
{
 sealed public class PQLService
    {
        private static PQLService? _instance;
        private readonly IPBKService _pkbService;
        private readonly QueryEvaluator _queryEvaluator;
        
        private PQLService(IPBKService pkbService)
        {
            _pkbService = pkbService;
            _queryEvaluator = new QueryEvaluator(pkbService);
        }
        
        public static PQLService GetInstance(IPBKService pkbService)
        {
            return _instance ??= new PQLService(pkbService);
        }
        
        public void StartListening()
        {
            Console.WriteLine("Ready");

            while (true)
            {
                string declarationLine = Console.ReadLine() ?? string.Empty;
                string selectLine = Console.ReadLine() ?? string.Empty;

                var parser = new PQLParser();
                var declarations = new List<string> { declarationLine };
                var query = parser.Parse(declarations, selectLine);

                string result = _queryEvaluator.EvaluateQuery(query);

                Console.WriteLine(result);
            }
        }
    }
}
