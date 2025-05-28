using ProjectSPA_ATS.PQL;
using ProjectSPA_ATS.Parser;
using ProjectSPA_ATS.PKB;
using ProjectSPA_ATS.Helpers;
using System.Reflection.Emit;
using ProjectSPA_ATS.Structures;

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
            //_PQLService.StartListening();

            Console.WriteLine("=== IsProcModifies ===");
            Console.WriteLine($"Main modifies x1: {_pbkService.IsProcModifies("Main", "x1")}");
            Console.WriteLine($"Main modifies radius: {_pbkService.IsProcModifies("Main", "radius")}");
            Console.WriteLine($"Init modifies left: {_pbkService.IsProcModifies("Init", "left")}");
            Console.WriteLine($"Init modifies randomVar: {_pbkService.IsProcModifies("Init", "randomVar")}");

            Console.WriteLine("\n=== IsProcUses ===");
            Console.WriteLine($"Main uses incre: {_pbkService.IsProcUses("Main", "incre")}");
            Console.WriteLine($"Main uses volume: {_pbkService.IsProcUses("Main", "volume")}");
            Console.WriteLine($"Init uses right: {_pbkService.IsProcUses("Init", "right")}");
            Console.WriteLine($"Random uses incre: {_pbkService.IsProcUses("Random", "incre")}");
            Console.WriteLine($"Shear uses x2: {_pbkService.IsProcUses("Shear", "x2")}");

            Console.WriteLine("\n=== GetProcModifies ===");
            PrintModList("Main modifies", _pbkService.GetProcModifies("Main"));
            PrintModList("Init modifies", _pbkService.GetProcModifies("Init"));
            PrintModList("QQ modifies", _pbkService.GetProcModifies("QQ"));

            Console.WriteLine("\n=== GetProcUses ===");
            PrintUseList("Main uses", _pbkService.GetProcUses("Main"));
            PrintUseList("Shear uses", _pbkService.GetProcUses("Shear"));
            PrintUseList("QQ uses", _pbkService.GetProcUses("QQ"));

          void PrintModList(string label, IEnumerable<Modify> list)
            {
                Console.WriteLine($"{label}:");
                foreach (var m in list)
                {
                    Console.WriteLine($"  stmt {m.Statement} modifies {m.Variable}");
                }
            }

          void PrintUseList(string label, IEnumerable<Use> list)
            {
                Console.WriteLine($"{label}:");
                foreach (var u in list)
                {
                    Console.WriteLine($"  stmt {u.StatementId} uses {u.VariableName}");
                }
            }
        }
    }
}