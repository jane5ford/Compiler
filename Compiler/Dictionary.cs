using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler
{
    class Dictionary
    {
        public HashSet<string> keyWord;
        public HashSet<char> arithmeticOperator;
        public HashSet<string> assignmentOperator;
        public HashSet<string> comparisingOperator;
        public HashSet<string> logicOperator;
        public HashSet<char> parentheses;
        public HashSet<char> punctuation;
        public void Create()
        {
            keyWord = new HashSet<string>
            {
                "bool",                 "break",                "byte",                "case",
                "class",                "const",                "double",              "else",
                "float",                "for",                  "if",                  "int",
                "namespace",            "new",                  "null",                "private",
                "protected",            "public",               "return",              "static",
                "string",               "switch",              "this",                 "true",
                "using",                "void",                "while",                "write",
                "read",                 "char"
            };
            arithmeticOperator = new HashSet<char>
            {
                '=',                '+',                '-',
                '*',                '/',                '%'
            };
            /*assignmentOperator = new HashSet<String>
            {
                "=",                "+=",                "-=",                "*=",
                "/=",               "--",                "++" 
            };
            comparisingOperator = new HashSet<String>
            {
                "==",                ">",                "<",
                "!=",                ">=",                "<="
            };
            logicOperator = new HashSet<String>
            {
                "!",                "&&",                "||"
            };*/
            parentheses = new HashSet<char>
            {
                '[',                ']',                '(',
                ')',                '{',                 '}'
            };
            punctuation = new HashSet<char>
            {
                '.',                ';',                ',',               ':'             
                
            };
        }
    }

}
