using ProjectSPA_ATS.LexicalAnalysis;
using ProjectSPA_ATS.PKB;
using ProjectSPA_ATS.Structures.AST;
using System.Xml.Linq;

namespace ProjectSPA_ATS.Parser
{
    sealed public class ParserService
    {
        private static ParserService? _instance;
        private readonly IPBKService _PBKService;

        private ParserService(IPBKService PKBService)
        {
            _PBKService = PKBService;
        }

        public static ParserService GetInstance(IPBKService pbkService)
        {
            return _instance ??= new ParserService(pbkService);
        }

        private List<Token> tokens;
        private int position = 0;
        private Token CurrentToken => tokens[position];

        
        public void initTokens(List<Token> tokens) 
        {
            this.tokens = tokens;
        }

        // Pobiera bieżący token i przesuwa wskaźnik na następny
        private Token Eat(TokenType expectedType)
        {
            Token token = CurrentToken;
            if (token.Type != expectedType)
            {
                throw new Exception($"Błąd składni: oczekiwano {expectedType}, a znaleziono {token.Type} (\"{token.Value}\")");
            }
            position++;
            return token;
        }
        public bool ParseProgram(string sourceCode)
        {
            var lexer = new Lexer(sourceCode);
            List<Token> tokens = lexer.GetTokens(sourceCode);
            Console.WriteLine("== TOKENS ==");
            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }

            //Parsujemy listę tokenów, otrzymujemy obiekt AST
            initTokens(tokens);


            while (CurrentToken.Type != TokenType.EndOfFile)
            {
                var procNode = ParseProcedure();
                _PBKService.AddProcedure(procNode);

                // extractor po każdej procedurze
                new DesignExtractor(_PBKService).Extract(procNode);
            }
            return true;
        }

        public ProcedureNode ParseProcedure()
        {

            Eat(TokenType.Procedure);                     // spodziewamy się 'procedure'
            Token nameToken = Eat(TokenType.Identifier);  // nazwa procedury
            string procName = nameToken.Value;
            Eat(TokenType.LBrace);                       
            List<StatementNode> stmtList = ParseStmtList();
            Eat(TokenType.RBrace);                       
           
            return new ProcedureNode(procName, stmtList);
        }

        private List<StatementNode> ParseStmtList()
        {
            var statements = new List<StatementNode>();
           
                while (CurrentToken.Type != TokenType.RBrace && CurrentToken.Type != TokenType.EndOfFile)
            {
                statements.Add(ParseStmt()); 
            }
            return statements;
        }

        public StatementNode ParseStmt()
        {
            return CurrentToken.Type switch
            {
                TokenType.Identifier => ParseAssign(),
                TokenType.While => ParseWhile(),
                TokenType.If => ParseIf(),
                TokenType.Call => ParseCall(),
                _ => throw new($"Nieznany token {CurrentToken}")
            };
        }

        private AssignNode ParseAssign()
        {
            
            Token varToken = Eat(TokenType.Identifier);
            string varName = varToken.Value;
            Eat(TokenType.Assign);            
            ExpressionNode exprNode = ParseExpr();
            Eat(TokenType.Semicolon);        
            return new AssignNode(varName, exprNode);
        }

        private WhileNode ParseWhile()
        {
            Eat(TokenType.While);             
            Token condToken = Eat(TokenType.Identifier);  
            string condVar = condToken.Value;
            Eat(TokenType.LBrace);            
            List<StatementNode> body = ParseStmtList();
            Eat(TokenType.RBrace);            
            return new WhileNode(condVar, body);
        }
        private IfNode ParseIf()
       {
           Eat(TokenType.If);
           var cond = Eat(TokenType.Identifier).Value;
           Eat(TokenType.Then);
           Eat(TokenType.LBrace);
           var thenB = ParseStmtList();
           Eat(TokenType.RBrace);
           Eat(TokenType.Else);
           Eat(TokenType.LBrace);
           var elseB = ParseStmtList();
           Eat(TokenType.RBrace);
           return new IfNode(cond, thenB, elseB);
       }

       private CallNode ParseCall()
       {
           Eat(TokenType.Call);
           string callee = Eat(TokenType.Identifier).Value;
           Eat(TokenType.Semicolon);
           return new CallNode(callee);
       }


        private ExpressionNode ParseExpr()
        {
           
            ExpressionNode left = ParseTerm();
            while (CurrentToken.Type == TokenType.Plus || CurrentToken.Type == TokenType.Minus)
            {
                var op = CurrentToken.Type; 
                Eat(op);
                var right = ParseTerm();
                left = new BinaryOpNode(left, right, op == TokenType.Plus ? "+" : "-");
            }
            return left;
        }

        private ExpressionNode ParseTerm()   // mnożenie
        {
            var left = ParseFactor();
            while (CurrentToken.Type == TokenType.Star)
            {
                Eat(TokenType.Star);
                var right = ParseFactor();
                left = new BinaryOpNode(left, right, "*");
            }
            return left;
        }
        private ExpressionNode ParseFactor()
        {
            if (CurrentToken.Type == TokenType.Number)
            {
                Token numToken = Eat(TokenType.Number);
                int value = int.Parse(numToken.Value);
                return new ConstNode(value);
            }
            if (CurrentToken.Type == TokenType.Identifier)
            {
                Token varToken = Eat(TokenType.Identifier);
                string name = varToken.Value;
                return new VarRefNode(name);
            }

            if(CurrentToken.Type == TokenType.LParen)
            {
                Eat(TokenType.LParen);
                ExpressionNode inner = ParseExpr();
                Eat(TokenType.RParen);
                return inner;
            }
            throw new Exception($"Nieoczekiwany token w wyrażeniu: {CurrentToken.Type}");
        }
    }
}
