using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler
{
    class AParser
    {
        int col = -1;
        string currentLine = "45+(10*2)+25/5";

        public AParser()
        {
            Console.WriteLine(Formula());
        }

        int Formula() { return Expression(Term(Factor())); }

        int Expression(int left)
        {
            char ch = getChar(); int right;
            if (ch != '+' && ch != '-')
            {
                returnChar(ch);
                return left;
            }
            right = Term(Factor());
            if (ch == '+')
            {
                return Expression(left + right);                
            }
            return Expression(left - right);
        }

        int Term(int left)
        {
            char ch = getChar(); int right;
            if (ch != '*' && ch != '/')
            {
                returnChar(ch);
                return left;
            }
            right = Factor();
            if (ch == '*')
            {
                return Term(left * right);
            }
            if (right == 0) throw new NotImplementedException(); //делить на 0 нельзя
            return Term(left / right);
        }

        int Factor()
        {
            char ch = getChar();
            if (char.IsDigit(ch)) return getValue(ch);
            if (ch == '(')
            {
                int result = Formula(); // ??
                if (getChar() == ')') return result;
                return 0;
            }
            throw new NotImplementedException(); // не цифра
        }

        char getChar() 
        {
            if (col < currentLine.Length - 1)
            {
                col++;
                return currentLine[col];
            }
            return ' ';
        }

        void returnChar(char ch) 
        {
            col--;
        }

        int getValue(char ch) 
        {
            int value = (int) char.GetNumericValue(ch);
            while (char.IsDigit(ch = getChar()))
                value = value * 10 + (int)char.GetNumericValue(ch);
            returnChar(ch);    
            return value;
        }
        
    }

}
