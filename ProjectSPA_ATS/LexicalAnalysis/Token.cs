using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSPA_ATS.LexicalAnalysis
{
    public class Token
    {
        public TokenType Type { get; }
        public string Value { get; }

        public Token(TokenType type, string value = "")
        {
            Type = type;
            Value = value;
        }

        public override string ToString() => Value != "" ? $"{Type}({Value})" : Type.ToString();
    }
}
