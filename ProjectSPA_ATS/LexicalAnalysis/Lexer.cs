using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSPA_ATS.LexicalAnalysis
{
    public class Lexer
    {
        private string source;
        private int position = 0;

        public Lexer(string source)
        {
            this.source = source;
        }

        public List<Token> GetTokens(string source)
        {
            var tokens = new List<Token>();
            int i = 0;
            while (i < source.Length)
            {
                char c = source[i];
                if (char.IsWhiteSpace(c))
                {
                    i++;
                    continue;  // pomijamy białe znaki
                }
                if (char.IsLetter(c))
                {
                    // budujemy identyfikator lub słowo kluczowe
                    int start = i;
                    while (i < source.Length && (char.IsLetterOrDigit(source[i])))
                    {
                        i++;
                    }
                    string text = source.Substring(start, i - start);
                    if (text == "procedure")
                    {
                        tokens.Add(new Token(TokenType.Procedure));
                    }
                    else if (text == "while")
                    {
                        tokens.Add(new Token(TokenType.While));
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.Identifier, text));
                    }
                    continue;
                }
                if (char.IsDigit(c))
                {
                    // budujemy liczbę
                    int start = i;
                    while (i < source.Length && char.IsDigit(source[i]))
                    {
                        i++;
                    }
                    string number = source.Substring(start, i - start);
                    tokens.Add(new Token(TokenType.Number, number));
                    continue;
                }
                
                switch (c)
                {
                    case '{':
                        tokens.Add(new Token(TokenType.LBrace));
                        i++;
                        break;
                    case '}':
                        tokens.Add(new Token(TokenType.RBrace));
                        i++;
                        break;
                    case '+':
                        tokens.Add(new Token(TokenType.Plus));
                        i++;
                        break;
                    case '=':
                        tokens.Add(new Token(TokenType.Assign));
                        i++;
                        break;
                    case ';':
                        tokens.Add(new Token(TokenType.Semicolon));
                        i++;
                        break;
                    default:
                        throw new Exception($"Nieoczekiwany znak: {c}");
                }
            }
            tokens.Add(new Token(TokenType.EndOfFile));
            return tokens;
        }

    }

}
