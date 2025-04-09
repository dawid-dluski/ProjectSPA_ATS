using ProjectSPA_ATS.LexicalAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ProjectSPA_ATS.PQL.Models;

namespace ProjectSPA_ATS.Parser
{
    public class PQLParser
    {
        private List<Token> tokens = new();
        private int position = 0;

        private Token CurrentToken => tokens[position];

        private Token Eat(TokenType expectedType)
        {
            if (CurrentToken.Type != expectedType)
                throw new Exception($"Expected {expectedType}, found {CurrentToken.Type} (\"{CurrentToken.Value}\")");
            return tokens[position++];
        }

        private bool Match(TokenType type) => CurrentToken.Type == type;

        public PQLQuery Parse(List<string> declarationLines, string queryLine)
        {
            var declarations = new List<Declaration>();

            foreach (var line in declarationLines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                tokens = new PQLLexer(line).Tokenize();
                position = 0;
                declarations.Add(ParseDeclaration());
            }

            tokens = new PQLLexer(queryLine).Tokenize();
            position = 0;

            Eat(TokenType.Select);
            string selected = Eat(TokenType.Identifier).Value;

            RelationshipClause? relClause = null;
            WithClause? withClause = null;

            while (!Match(TokenType.EndOfFile))
            {
                if (Match(TokenType.Such))
                {
                    Eat(TokenType.Such);
                    Eat(TokenType.That);
                    relClause = ParseSuchThatClause();
                }
                else if (Match(TokenType.With))
                {
                    Eat(TokenType.With);
                    withClause = ParseWithClause();
                }
                else
                {
                    throw new Exception($"Unexpected token: {CurrentToken.Type}");
                }
            }

            return new PQLQuery(declarations, selected, relClause, withClause);
        }

        private Declaration ParseDeclaration()
        {
            string designEntity = Eat(TokenType.DesignEntity).Value;
            var synonyms = new List<string> { Eat(TokenType.Identifier).Value };

            while (Match(TokenType.Comma))
            {
                Eat(TokenType.Comma);
                synonyms.Add(Eat(TokenType.Identifier).Value);
            }

            Eat(TokenType.Semicolon);
            return new Declaration(designEntity, synonyms);
        }

        private RelationshipClause ParseSuchThatClause()
        {
            var relToken = Eat(CurrentToken.Type); // RelRef or RelRefT
            bool isTransitive = relToken.Type == TokenType.RelRefT;

            Eat(TokenType.LParen);
            string arg1 = ParseArgument();
            Eat(TokenType.Comma);
            string arg2 = ParseArgument();
            Eat(TokenType.RParen);

            return new RelationshipClause(relToken.Value, isTransitive, arg1, arg2);
        }

        private WithClause ParseWithClause()
        {
            string synonym = Eat(TokenType.Identifier).Value;
            Eat(TokenType.Dot);
            string attr = Eat(TokenType.Identifier).Value;
            Eat(TokenType.Equals);

            string value;
            string type;

            if (Match(TokenType.Number))
            {
                type = "int";
                value = Eat(TokenType.Number).Value;
            }
            else if (Match(TokenType.StringLiteral))
            {
                type = "string";
                value = "\"" + Eat(TokenType.StringLiteral).Value + "\"";
            }
            else
            {
                throw new Exception($"Invalid value in with clause: {CurrentToken.Type}");
            }

            return new WithClause(synonym, attr, type, value);
        }

        private string ParseArgument()
        {
            return CurrentToken.Type switch
            {
                TokenType.Identifier => Eat(TokenType.Identifier).Value,
                TokenType.Number => Eat(TokenType.Number).Value,
                TokenType.Underscore => Eat(TokenType.Underscore).Value,
                TokenType.StringLiteral => "\"" + Eat(TokenType.StringLiteral).Value + "\"",
                _ => throw new Exception($"Invalid argument: {CurrentToken.Type}")
            };
        }
    }
}
