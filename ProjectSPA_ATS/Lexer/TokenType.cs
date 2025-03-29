using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSPA_ATS.Lexer
{
    public enum TokenType
    {
        // Słowa kluczowe:
        Procedure,  // "procedure"
        While,      // "while"
                    // Symbole:
        LBrace,     // "{"
        RBrace,     // "}"
        Assign,     // "="
        Semicolon,  // ";"
        Plus,       // "+"
                    // Identyfikatory i wartości:
        Identifier, // nazwa zmiennej lub procedury
        Number,     // stała liczbowa
                    // Specjalny:
        EndOfFile   // koniec wejścia
    }
}
