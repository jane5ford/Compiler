using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler
{
    class Dictionary
    {
        public HashSet<string> keyWord;
        public HashSet<string> varType;
        public HashSet<string> arithmeticOperator;
        public HashSet<string> assignmentOperator;
        public HashSet<string> comparisingOperator;
        public HashSet<string> logicOperator;
        public HashSet<string> parentheses;
        public HashSet<string> punctuation;
        public void Create()
        {
            keyWord = new HashSet<string>
            {
                "break",                "false",                "case",
                "class",                "const",                                        "else",
                "for",                  "if",
                "namespace",            "new",                  "null",                 "private",
                "protected",            "public",               "return",               "static",
                "switch",               "this",                 "true",
                "using",                "void",                 "while",                "write",
                "read",                 
            };
            varType = new HashSet<string>
            {
                "bool",                 "char",                 "double",               "float", 
                "int",                  "string",               "var",                  "byte"
            };
            arithmeticOperator = new HashSet<string>
            {
                "=",                "+",                "-",
                "*",                "/",                "%"
            };
            assignmentOperator = new HashSet<String>
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
            };
            parentheses = new HashSet<string>
            {
                "[",                "]",                "(",
                ")",                "{",                 "}"
            };
            punctuation = new HashSet<string>
            {
                ".",                ";",                ",",               ":"             
            };
        }
    }

}
