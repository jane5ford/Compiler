using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler
{
    class Parser
    {
        Lexer lexer;
        public Parser(Lexer lexer)
        {
            this.lexer = lexer;
        }
        public Node ParseExpression()
        {
            var left = ParseTerm();
            var t = lexer.GetNext();
            if (t.value == "+" || t.value == "-")
            {
                var right = ParseExpression();
                return new NodeBinaryOp
                {
                    left = left,
                    right = right,
                    op = t.value
                };
            }
            lexer.PutBack(t);
            return left;
        }
        private Node ParseTerm()
        {
            var left = ParseFactor();
            var t = lexer.GetNext();
            if (t.value == "*" || t.value == "/")
            {
                var right = ParseTerm();
                return new NodeBinaryOp
                {
                    left = left,
                    right = right,
                    op = t.value
                };
            }
            lexer.PutBack(t);
            return left;
        }

        private Node ParseFactor()
        {
            var t = lexer.GetNext();
            if (t.value == "(")
            {
                var e = ParseExpression();
                if (lexer.GetNext().value != ")") throw new Exception("error");
                return e;
            }
            if (t.type == TokenType.IDENTIFIER)
            {
                return new NodeIdentifier { name = t.value };
            }
            if (t.type == TokenType.INT) { return new NodeIntLiteral { value = int.Parse(t.value) }; }
            throw new Exception("error");
        }
        
        public Node ParseEqualityExp()
        {
            var left = ParseLogicFactor();
            var t = lexer.GetNext();
            if (t.value == "==")
            {
                var right = ParseEqualityExp();
                return new NodeBinaryOp
                {
                    left = left,
                    right = right,
                    op = t.value
                };
            }
            lexer.PutBack(t);
            return left;
        }

        public Node ParseOrExp() 
        {
            var left = ParseAndExp();
            var t = lexer.GetNext();
            if (t.value == "||")
            {
                var right = ParseOrExp();
                return new NodeBinaryOp
                {
                    left = left,
                    right = right,
                    op = t.value
                };
            }
            if(t.value == "==" || t.value == "!=")
            {
                var right = ParseLogicFactor();
                return new NodeBinaryOp
                {
                    left = left,
                    right = right,
                    op = t.value
                };
            }
            lexer.PutBack(t);
            return left;
        }

        private Node ParseAndExp()
        {
            var left = ParseLogicFactor();
            var t = lexer.GetNext();
            if (t.value == "&&")
            {
                var right = ParseAndExp();
                return new NodeBinaryOp
                {
                    left = left,
                    right = right,
                    op = t.value
                };
            }
            lexer.PutBack(t);
            return left;
        }

        private Node ParseLogicFactor()
        {
            var t = lexer.GetNext();
            if (t.value == "(")
            {
                var e = ParseOrExp();
                if (lexer.GetNext().value != ")") throw new Exception("error");
                return e;
            }
            if (t.type == TokenType.IDENTIFIER)
            {
                return new NodeIdentifier { name = t.value };
            }
            if (t.type == TokenType.BOOLEAN) { return new NodeBoolean { value = t.value }; }
            throw new Exception("error");
        }


        //public Node ParseDeclaration()
        //{

        //}

        public Node ParseNewLine()
        {
            var t = lexer.GetNext();
            if (t.value != ";") ParseNewLine(); // TODO
            return new NodeLine { value = t.value };
        }

        //public Node ParseConditional()
        //{
        //    var t = lexer.GetNext();
        //    if (t.value == "if")
        //    {
        //        if (lexer.GetNext().value == "(")
        //        {
        //            var condition  = ParseLogicExp();
        //            if (lexer.GetNext().value != ")") throw new Exception("error");
        //            var body = ParseBody();
        //        }

        //    }
        //    //elif
        //    //else
        //    //endif
        //}


    }
}
