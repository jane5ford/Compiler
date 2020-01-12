using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.Json;

namespace Compiler
{    
    class Lexer
    {
        Dictionary dictionary;
        int col = -1;
        int row;
        StreamReader sr;
        string currentLine;
        char ch;
        public bool hasValue = false;
        public bool hasSep = false;
        public Lexer(StreamReader sr)
        {
            Dictionary dictionary = new Dictionary();
            dictionary.Create();
            this.dictionary = dictionary;
            this.sr = sr;
            currentLine = sr.ReadLine();
        }

        private char getChar()
        {
            if (currentLine == null) return '\0';
            if (col < currentLine.Length - 1)
            {
                col++;
                return currentLine[col];
            }
            currentLine = sr.ReadLine();
            row++;
            if (currentLine == null) return '\0';
            col = -1; 
            return ' '; 
        }

        public Token getNext()
        {
            ch = getChar();
            TokenType t;
            while ((ch == ' ') || ch == '\t')
            {
                ch = getChar();
            }
            if (ch == '\0')
            {
                return new Token(TokenType.END_OF_FILE);
            }
            int pos = col;
            string value = "";
            if (char.IsLetter(ch))
            {
                value += ch;
                while (char.IsLetterOrDigit(ch = getChar())) 
                    value += ch;
                col--;
                if (dictionary.keyWord.Contains(value))
                    t = TokenType.KEYWORD;
                else t = TokenType.IDENTIFIER;
                return new Token(pos, row, t, value);
            }
            else if (char.IsDigit(ch))
            {
                value += ch;
                while (char.IsDigit(ch = getChar()))
                    value += ch;
                if (ch == '.')
                {
                    value += ch;
                    while (char.IsDigit(ch = getChar()))
                    {
                        value += ch;
                    }
                    if (ch == 'f')
                    {
                        value += ch;
                        t = TokenType.FLOAT;
                    }
                    else
                    {
                        ch = getChar();
                        t = TokenType.ERROR;
                    }
                }
                else if (char.IsLetter(ch)) 
                {
                    value += ch;
                    ch = getChar();
                    t = TokenType.ERROR; 
                }
                else t = TokenType.INT;
                return new Token(pos, row, t, value);
            }
            else
            {
                value += ch;
                if (dictionary.parentheses.Contains(ch)) t = TokenType.PARETHESES;
                else if (dictionary.punctuation.Contains(ch)) t = TokenType.PUNCTUATION;
                else if (dictionary.arithmeticOperator.Contains(ch))
                {
                    if ((ch = getChar()) == '=')
                    {
                        if (value[0] == '=') t = TokenType.COMPARISING_OPERATOR;
                        else t = TokenType.ASSIGNMENT_OPERATOR;
                        value += ch;
                    }
                    else
                    {
                        col--;
                        if ((ch == '+' || ch == '-' ) && value[0] == (ch = getChar()))
                        {
                            t = TokenType.ASSIGNMENT_OPERATOR;
                            value += ch;                            
                        }
                        else
                        {
                            t = TokenType.ARITHMETIC_OPERATOR;
                            if (ch == '+' || ch == '-') col--;
                        }
                    }
                }
                else if (ch == '<' || ch == '>' || ch == '!')
                {
                    if ((ch = getChar()) == '=') value += ch;
                    else col--;
                    if (value[0] == '!' && ch != '=') t = TokenType.LOGIC_OPERATOR;
                    else t = TokenType.COMPARISING_OPERATOR;
                }
                else if (ch == '&' || ch == '|')  
                {
                    if ((ch = getChar()) == value[0])
                    {
                        t = TokenType.LOGIC_OPERATOR;
                        value += ch;
                    }
                    else
                    {
                        col--;
                        t = TokenType.ERROR;
                    }
                }
                else t = TokenType.ERROR;
                return new Token(pos, row, t, value);
            }
        }


    }
}
