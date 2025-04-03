using ProjectSPA_ATS.AST;
using ProjectSPA_ATS.LexicalAnalysis;
using ProjectSPA_ATS.PKB;

namespace ProjectSPA_ATS.Parser
{
    public class ParserService
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

        // Konstruktor przyjmujący listę tokenów
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

            // 2. Parsujemy listę tokenów, otrzymujemy obiekt AST
            initTokens(tokens);
            ProcedureNode root = ParseProcedure();
            _PBKService.AddProcedure(root);
            return true;
        }

        public ProcedureNode ParseProcedure()
        {

            Eat(TokenType.Procedure);                     // spodziewamy się 'procedure'
            Token nameToken = Eat(TokenType.Identifier);  // nazwa procedury
            string procName = nameToken.Value;
            Eat(TokenType.LBrace);                       // '{'
            List<StatementNode> stmtList = ParseStmtList();
            Eat(TokenType.RBrace);                       // '}'

            return new ProcedureNode(procName, stmtList);
        }

        private List<StatementNode> ParseStmtList()
        {
            var statements = new List<StatementNode>();
            // Parsujemy wielokrotnie instrukcje dopóki nie natrafimy na '}' lub EOF.
            // (Uwaga: nie wywołujemy Eat tutaj na '}' – sprawdzamy tylko, czy nie należy przerwać.)
            while (CurrentToken.Type != TokenType.RBrace && CurrentToken.Type != TokenType.EndOfFile)
            {
                statements.Add(ParseStmt());
            }
            return statements;
        }

        public StatementNode ParseStmt()
        {
            if (CurrentToken.Type == TokenType.Identifier)
            {
                return ParseAssign();
            }
            if (CurrentToken.Type == TokenType.While)
            {
                return ParseWhile();
            }
            throw new Exception($"Nieznana instrukcja zaczynająca się od {CurrentToken.Type}");
        }

        private AssignNode ParseAssign()
        {
            // Po wywołaniu ParseStmt zakładamy, że bieżący token to Identifier
            Token varToken = Eat(TokenType.Identifier);
            string varName = varToken.Value;
            Eat(TokenType.Assign);            // '='
            ExpressionNode exprNode = ParseExpr();
            Eat(TokenType.Semicolon);         // ';'
            return new AssignNode(varName, exprNode);
        }

        private WhileNode ParseWhile()
        {
            Eat(TokenType.While);             // 'while'
            Token condToken = Eat(TokenType.Identifier);  // warunek - zmienna
            string condVar = condToken.Value;
            Eat(TokenType.LBrace);            // '{'
            List<StatementNode> body = ParseStmtList();
            Eat(TokenType.RBrace);            // '}'
            return new WhileNode(condVar, body);
        }

        private ExpressionNode ParseExpr()
        {
            // expr -> factor { '+' factor }
            ExpressionNode left = ParseFactor();
            while (CurrentToken.Type == TokenType.Plus)
            {
                Eat(TokenType.Plus);
                ExpressionNode right = ParseFactor();
                // Tworzymy węzeł binarnego dodawania, łącząc left i right
                left = new BinaryOpNode(left, right, "+");
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
            throw new Exception($"Nieoczekiwany token w wyrażeniu: {CurrentToken.Type}");
        }
    }
}