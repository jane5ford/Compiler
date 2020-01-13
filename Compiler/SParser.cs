using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler
{
    class SParser
    {
        int lexClass;
        const int IF = 1;
        const int ELSE = 2;
        const int LBRACKET = 3;
        const int RBRACKET = 4;
        const int LBRACE = 5;
        const int RBRACE = 6;
        const int NUM = 7;
        const int PRINT = 8;
        const int SEMICOLON = 9;
        const int EQ = 10;
        void nextStep(int lc)
        {
            if (lexClass == lc)
                lexClass = getLC();
            else
                throw new NotImplementedException();
        }


        void S()
        {
            switch (getLC())
            {
                case IF:
                    E(); nextStep(LBRACKET); S(); nextStep(ELSE); S(); break;
                case LBRACE:
                    S(); L(); break;
                case PRINT:
                    E(); break;
                default:
                    throw new NotImplementedException(); break;
            }
        }

        void L()
        {
            switch (lexClass)
            {
                case RBRACE:
                    getLC(); break;
                case SEMICOLON:
                    getLC(); S(); L(); break;
                default:
                    throw new NotImplementedException(); break;
            }
        }

        void E()
        {
            nextStep(NUM); nextStep(EQ); nextStep(NUM);
        }

        void main()
        {
            lexClass = getLC();
            S();
        }

        int getLC()
        {
            return 0;
        }


    }
}
