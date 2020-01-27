using System;
using System.IO;

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
            TokenType t = TokenType.ERROR;
            if (oldToken != null)
            {
                Token old = oldToken;
                oldToken = null;
                return old;
            }
            ch = GetChar();
            while ((ch == ' ') || ch == '\t') 
                ch = GetChar();
            if (ch == '\0') 
                return new Token(TokenType.END_OF_FILE);
            int pos = col;
            int str = row;
            string value = ch.ToString();
            if (char.IsLetter(ch))
            {
                while (char.IsLetterOrDigit(ch = GetChar()))
                    value += ch;
                if (dictionary.keyWord.Contains(value))
                {
                    if (value == "true" || value == "false") t = TokenType.BOOLEAN;
                    else t = TokenType.KEYWORD;
                }
                else if (dictionary.varType.Contains(value))
                    t = TokenType.VARTYPE;
                else t = TokenType.IDENTIFIER;
                PutChar();
                return new Token(pos, str, t, value);
            }
            if (char.IsDigit(ch))
            {
                while (char.IsDigit(ch = GetChar()))
                    value += ch;
                if (ch == '.')
                {
                    value += ch;
                    while (char.IsDigit(ch = GetChar()))
                    {
                        t = TokenType.FLOAT;
                        value += ch;
                    }
                    PutChar();
                }
                //if (ch == 'f' || ch == 'F')
                //{
                //    value += ch;
                //    t = TokenType.FLOAT;
                //}
                else {
                    PutChar();
                    //if (t == TokenType.FLOAT) 
                        t = TokenType.INT; 
                }
                if (t == TokenType.FLOAT)
                {
                    if ((ch = GetChar())== 'e')
                    {
                        value += ch;
                        while (char.IsDigit(ch = GetChar()))
                            value += ch;
                        if (ch == '-' || ch == '+')
                        {
                            value += ch;
                            while (char.IsDigit(ch = GetChar()))
                                value += ch;
                        }
                    }
                    PutChar();
                }
                return new Token(pos, str, t, value);
            }
            if (ch == '"')
            {
                while ((ch = GetChar()) != '"')
                    value += ch;
                if (ch == '"')
                {
                    value += ch;
                    t = TokenType.STRING;
                }
                else
                {
                    PutChar();
                    t = TokenType.ERROR;
                }
                return new Token(pos, str, t, value);
            }
            if (ch == '\'')
            {
                value += GetChar();
                if ((ch = GetChar()) == '\'')
                {
                    value += ch;
                    t = TokenType.CHAR;
                }
                else { PutChar(); t = TokenType.ERROR; }
                return new Token(pos, str, t, value);
            }
            else
            {
                value += GetChar();
                if (dictionary.assignmentOperator.Contains(value)) t = TokenType.ASSIGNMENT_OPERATOR;
                else if (dictionary.comparisingOperator.Contains(value)) t = TokenType.COMPARISING_OPERATOR;
                else if (dictionary.logicOperator.Contains(value)) t = TokenType.LOGIC_OPERATOR;
                else
                {
                        ch = value[1];
                        value = value[0].ToString();
                        PutChar();
                    if (dictionary.assignmentOperator.Contains(value)) t = TokenType.ASSIGNMENT_OPERATOR;
                    else if (dictionary.comparisingOperator.Contains(value)) t = TokenType.COMPARISING_OPERATOR;
                    else if (dictionary.logicOperator.Contains(value)) t = TokenType.LOGIC_OPERATOR;
                    else if (dictionary.arithmeticOperator.Contains(value)) t = TokenType.ARITHMETIC_OPERATOR;
                    else if (dictionary.parentheses.Contains(value)) t = TokenType.PARETHESES;
                    else if (dictionary.punctuation.Contains(value)) t = TokenType.PUNCTUATION;
                    else t = TokenType.ERROR;
                }
                return new Token(pos, str, t, value);
            }
        }
        public void PutBack(Token token)
        {
            if (oldToken != null) throw new NotImplementedException();
            oldToken = token;
        }
        private void PutChar()
        {
            if (ch != ' ' && ch != '\t') col--;
        }
    }
}