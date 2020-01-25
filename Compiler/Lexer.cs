﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.Json;

namespace Compiler
{    
    class Lexer
    {
        Token oldToken;
        Dictionary dictionary;
        int col = -1;
        int row = 0;
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

        private char GetChar()
        {
            if (currentLine == null) return '\0';
            if (col < currentLine.Length - 1)
            {
                col++;
                return currentLine[col];
            }
            currentLine = sr.ReadLine();
            if (currentLine == null) return '\0';
            col = -1;
            row++;
            return ' ';
        }

        public Token GetNext() //проверить комментарии
        {
            TokenType t;
            if (oldToken != null)
            {
                Token to = oldToken; 
                oldToken = null;
                return to;
            } 
            ch = GetChar();
            while ((ch == ' ') || ch == '\t')
            {
                ch = GetChar();
            }
            if (ch == '\0')
            {
                return new Token(TokenType.END_OF_FILE);
            }
            int pos = col;
            int str = row;
            string value = "";
            if (char.IsLetter(ch))
            {
                value += ch;
                while (char.IsLetterOrDigit(ch = GetChar()))
                    value += ch;
                if (dictionary.keyWord.Contains(value))
                {
                    if (value == "true" || value == "false") t = TokenType.BOOLEAN; 
                    else t = TokenType.KEYWORD;
                }
                else if (dictionary.varType.Contains(value))
                {
                    t = TokenType.VARTYPE;
                }
                else t = TokenType.IDENTIFIER;
                if (ch != ' ' && ch != '\t') col--;
                return new Token(pos, str, t, value);
            }
            if (char.IsDigit(ch))
            {
                value += ch;
                while (char.IsDigit(ch = GetChar()))
                    value += ch;
                if (ch == '.')
                {
                    value += ch;
                    int k = 0;
                    while (char.IsDigit(ch = GetChar()))
                    { value += ch; k++; }
                    if (ch != ' ' && ch != '\t') col--;
                    if (k > 7) t = TokenType.DOUBLE;
                    else if (k == 0) t = TokenType.ERROR;
                    else t = TokenType.FLOAT;
                }
                else if (char.IsLetter(ch))
                {
                    value += ch;
                    t = TokenType.ERROR;
                }
                else
                {
                    t = TokenType.INT;
                    if (ch != ' ' && ch !='\t') col--;
                }
                return new Token(pos, str, t, value);
            }
            if (ch == '"')
            {
                value += ch;
                while ((ch = GetChar()) != '"')
                {
                    value += ch;
                }
                if (ch == '"')
                {
                    value += ch;
                    t = TokenType.STRING;
                }
                else
                {
                    if (ch != ' ' && ch != '\t') col--;
                    t = TokenType.ERROR;
                }
                return new Token(pos, str, t, value);
            }
            if (ch == '\'')
            {
                value += ch;
                value += GetChar();
                if ((ch = GetChar()) == '\'')
                {
                    value += ch;
                    t = TokenType.CHAR;
                }
                else { if (ch != ' ' && ch != '\t') col--; t = TokenType.ERROR; }
                return new Token(pos, str, t, value);
            }
            else
            {
                value += ch;
                if (dictionary.parentheses.Contains(ch)) t = TokenType.PARETHESES;
                else if (dictionary.punctuation.Contains(ch)) t = TokenType.PUNCTUATION;
                else if (dictionary.arithmeticOperator.Contains(ch))
                {
                    if (ch == '=')
                    {
                        if ((ch = GetChar()) == '=')
                        {
                            t = TokenType.COMPARISING_OPERATOR;
                            value += ch;
                        }
                        else
                        {
                            if (ch != ' ' && ch != '\t') col--;
                            t = TokenType.ASSIGNMENT_OPERATOR;
                        }

                    }
                    else
                    {
                        if (ch == '+' || ch == '-')
                        {
                            if (value[0] == (ch = GetChar()))
                            {
                                t = TokenType.ASSIGNMENT_OPERATOR;
                                value += ch;
                            }
                            else
                            {
                                t = TokenType.ARITHMETIC_OPERATOR;
                                if (ch != ' ' && ch != '\t') col--;
                            }
                        }
                        else
                        {
                            t = TokenType.ARITHMETIC_OPERATOR;
                        }
                    }
                }
                else if (ch == '<' || ch == '>' || ch == '!')
                {
                    if ((ch = GetChar()) == '=') 
                    {
                        value += ch;
                        if (value[0] == '!' && ch != '=') t = TokenType.LOGIC_OPERATOR;
                        else t = TokenType.COMPARISING_OPERATOR;
                    }
                    else 
                    { 
                        if (ch != ' ' && ch != '\t') col--; 
                        t = TokenType.COMPARISING_OPERATOR; 
                    }
                }
                else if (ch == '&' || ch == '|')
                {
                    if ((ch = GetChar()) == value[0])
                    {
                        t = TokenType.LOGIC_OPERATOR;
                        value += ch;
                    }
                    else
                    {
                        if (ch != ' ' && ch != '\t') col--;
                        t = TokenType.ERROR;
                    }
                }
                else { if (ch != ' ' && ch != '\t') col--; t = TokenType.ERROR; }
                return new Token(pos, str, t, value);
            }
        }

        public void PutBack(Token token)
        {
            if (oldToken != null) throw new NotImplementedException();
            oldToken = token;
        }

        
    }
}
