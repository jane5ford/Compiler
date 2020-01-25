using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler
{
    class Dictionary
    {
        public HashSet<string> keyWord;
        public HashSet<string> varType;
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
                "bool",                 "break",                "false",                "case",
                "class",                "const",                                        "else",
                "float",                "for",                  "if",                  
                "namespace",            "new",                  "null",                 "private",
                "protected",            "public",               "return",               "static",
                                        "switch",               "this",                 "true",
                "using",                "void",                 "while",                "write",
                "read",                 
            };
            varType = new HashSet<string>
            {
                "bool",                 "char",                 "double",               "float", 
                "int",                  "string",               "var"
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
