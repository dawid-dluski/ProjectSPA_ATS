using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSPA_ATS.LexicalAnalysis
{
    public class PQLLexer
    {
        private string source;
        private int position = 0;

        private static readonly HashSet<string> designEntities = new()
        {
            "stmt", "assign", "while", "variable", "constant", "prog_line"
        };

        private static readonly Dictionary<string, TokenType> keywords = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Select", TokenType.Select },
            { "such", TokenType.Such },
            { "that", TokenType.That },
            { "with", TokenType.With },
            { "BOOLEAN", TokenType.Boolean }
        };

        private static readonly HashSet<string> relRefs = new()
        {
            "Follows", "Parent", "Modifies", "Uses", "Calls"
        };

        public PQLLexer(string source)
        {
            this.source = source;
        }

        public List<Token> Tokenize()
        {
            var tokens = new List<Token>();
            position = 0;

            while (position < source.Length)
            {
                char c = source[position];

                if (char.IsWhiteSpace(c))
                {
                    position++;
                    continue;
                }

                if (char.IsLetter(c))
                {
                    tokens.Add(ReadIdentifierOrKeywordOrRelRef());
                    continue;
                }

                if (char.IsDigit(c))
                {
                    tokens.Add(ReadNumber());
                    continue;
                }

                switch (c)
                {
                    case '"':
                        tokens.Add(ReadStringLiteral());
                        break;
                    case ';':
                        tokens.Add(new Token(TokenType.Semicolon, ";")); position++; break;
                    case ',':
                        tokens.Add(new Token(TokenType.Comma, ",")); position++; break;
                    case '(':
                        tokens.Add(new Token(TokenType.LParen, "(")); position++; break;
                    case ')':
                        tokens.Add(new Token(TokenType.RParen, ")")); position++; break;
                    case '_':
                        tokens.Add(new Token(TokenType.Underscore, "_")); position++; break;
                    case '.':
                        tokens.Add(new Token(TokenType.Dot, ".")); position++; break;
                    case '=':
                        tokens.Add(new Token(TokenType.Equals, "=")); position++; break;
                    case '<':
                        tokens.Add(new Token(TokenType.LAngle, "<")); position++; break;
                    case '>':
                        tokens.Add(new Token(TokenType.RAngle, ">")); position++; break;
                    default:
                        throw new Exception($"Unexpected character: {c}");
                }
            }

            tokens.Add(new Token(TokenType.EndOfFile));
            return tokens;
        }

        private Token ReadIdentifierOrKeywordOrRelRef()
        {
            int start = position;
            while (position < source.Length && (char.IsLetterOrDigit(source[position]) || source[position] == '#'))
            {
                position++;
            }

            string word = source.Substring(start, position - start);

            if (position < source.Length && source[position] == '*')
            {
                string refName = word;
                position++; // consume '*'
                if (relRefs.Contains(refName))
                    return new Token(TokenType.RelRefT, refName + "*");
                else
                    return new Token(TokenType.Identifier, refName + "*"); // fallback
            }

            if (keywords.ContainsKey(word))
                return new Token(keywords[word], word);

            if (designEntities.Contains(word.ToLower()))
                return new Token(TokenType.DesignEntity, word);

            if (relRefs.Contains(word))
                return new Token(TokenType.RelRef, word);

            if (word.Contains("#") || word.Equals("stmt#", StringComparison.OrdinalIgnoreCase) || word.Equals("varName", StringComparison.OrdinalIgnoreCase))
                return new Token(TokenType.AttributeName, word);

            return new Token(TokenType.Identifier, word);
        }

        private Token ReadNumber()
        {
            int start = position;
            while (position < source.Length && char.IsDigit(source[position]))
            {
                position++;
            }

            string number = source.Substring(start, position - start);
            return new Token(TokenType.Number, number);
        }

        private Token ReadStringLiteral()
        {
            position++; // skip opening "
            int start = position;

            while (position < source.Length && source[position] != '"')
            {
                position++;
            }

            if (position >= source.Length)
                throw new Exception("Unterminated string literal");

            string value = source.Substring(start, position - start);
            position++; // skip closing "
            return new Token(TokenType.StringLiteral, value);
        }
    }
}
