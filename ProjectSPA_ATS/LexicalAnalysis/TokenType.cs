using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSPA_ATS.LexicalAnalysis
{
    public enum TokenType
    {
        // Słowa kluczowe:
        Procedure,  
        While, 
        If, 
       Then, 
       Else,
       Call,
                    // Symbole:
        LBrace,     
        RBrace,     
        Assign,     
        Semicolon, 
        Plus,       
        Minus,
        Star,
        // Identyfikatory i wartości:
        Identifier, // nazwa zmiennej lub procedury
        Number,     // stała liczbowa
                    // Specjalny:
        EndOfFile,  // koniec wejścia

        // PQL
        Select,
        Such,
        That,
        With,
        Boolean,
        Comma,
        LParen,
        RParen,
        LAngle,
        RAngle,
        DesignEntity,     // stmt, assign, while, variable, constant, prog_line
        StringLiteral,    // "x"
        Underscore,       // _
        Dot,              // .
        Equals,           // =
        RelRef,           // Follows, Parent, Modifies, Uses
        RelRefT,          // Follows*, Parent*, etc.
        AttributeName     // stmt#, varName, etc.



    }
}
