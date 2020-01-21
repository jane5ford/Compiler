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
            var t = lexer.getNext();
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
        public Node ParseTerm()
        {
            var left = ParseFactor();
            var t = lexer.getNext();
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

        public Node ParseFactor()
        {
            var t = lexer.getNext();
            if (t.value == "(")
            {
                var e = ParseExpression();
                if (lexer.getNext().value != ")") throw new Exception("error");
                return e;
            }
            if (t.type == TokenType.IDENTIFIER)
            {
                return new NodeIdentifier { name = t.value };
            }
            if (t.type == TokenType.INT) { return new NodeIntLiteral { value = int.Parse(t.value) }; }
            throw new Exception("error");
        }

        
    }
}
