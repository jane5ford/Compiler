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
        public Node ParseAdditiveExpression()
        {
            var left = ParseMultiplicativeExpression();
            var t = lexer.GetNext();
            if (t.value == "+" || t.value == "-")
            {
                var right = ParseAdditiveExpression();
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
        private Node ParseMultiplicativeExpression()
        {
            var left = ParseNumFactor();
            var t = lexer.GetNext();
            if (t.value == "*" || t.value == "/")
            {
                var right = ParseMultiplicativeExpression();
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

        private Node ParseNumFactor()
        {
            var t = lexer.GetNext();
            if (t.value == "(")
            {
                var e = ParseAdditiveExpression();
                if (lexer.GetNext().value != ")") throw new Exception("error");
                return e;
            }
            if (t.type == TokenType.IDENTIFIER)
            {
                return new NodeIdentifier { name = t.value };
            }
            if (t.type == TokenType.INT) { return new NodeIntLiteral { value = int.Parse(t.value) }; }
            lexer.PutBack(t);
            throw new Exception("error");
        }

        //public Node ParseEqualityExp()
        //{
        //    var left = ParseLogicFactor();
        //    var t = lexer.GetNext();
        //    if (t.value == "==")
        //    {
        //        var right = ParseEqualityExp();
        //        return new NodeBinaryOp
        //        {
        //            left = left,
        //            right = right,
        //            op = t.value
        //        };
        //    }
        //    lexer.PutBack(t);
        //    return left;
        //}

        
        public Node ParseOrExpression()
        {
            var left = ParseAndExpression();
            var t = lexer.GetNext();
            if (t.value == "||")
            {
                var right = ParseOrExpression();
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

        
        private Node ParseAndExpression()
        {
            var left = ParseEqualityExpression();
            var t = lexer.GetNext();
            if (t.value == "&&")
            {
                var right = ParseAndExpression();
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

        public Node ParseEqualityExpression()
        {
            var left = ParseRelationalExpession();
            var t = lexer.GetNext();
            if (t.value == "==" || t.value == "!=")
            {
                var right = ParseEqualityExpression();
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

        public Node ParseRelationalExpession()
        {
            var left = ParseLogicFactor();
            Token t = lexer.GetNext();
            if (t.value == "<" || t.value == ">" || t.value == "<=" || t.value == ">=")
            {
                var op = t.value;
                var right = ParseAdditiveExpression();
                return new NodeBinaryOp()
                {
                    left = left,
                    right = right,
                    op = op
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
                var e = ParseOrExpression();
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

        private Node ParseAssignment()
        {
            var left = ParseNumFactor();
            Token t = lexer.GetNext();
            if (t.type == TokenType.ASSIGNMENT_OPERATOR)
            {
                var right = ParseExpression();
            }
        }

        public Node ParseExpression()
        {

        }

        public Node ParseNewLine()
        {
            var line = new NodeLine();
            line.units = new List<NodeUnit>();
            var u = ParseNewUnit();
            while (u.value != ";")
            {
                line.units.Add(u);
                u = ParseNewUnit();
            }
            return line;
        }

        public NodeUnit ParseNewUnit()
        {
            var t = lexer.GetNext();
            return new NodeUnit(t);
        }

        private NodeList ParseStatement()
        {
            List<Node> lines = new List<Node>();
            Token t = lexer.GetNext();
            if (t.value == "{")
            {
                t = lexer.GetNext();
                while (t.value != "}")
                {
                    lexer.PutBack(t);
                    var s = ParseNewLine();
                    lines.Add(s);
                    t = lexer.GetNext();
                }                
            }
            else
            {
                lexer.PutBack(t);
                var s = ParseNewLine();
                lines.Add(s);
            }
            return new NodeList() { list = lines };
        }

        public NodeList ParseConditional()
        {
            Token t;
            if ((t = lexer.GetNext()).value == "if")
            {
                List<Node> sections = new List<Node>();
                string op = "if";
                
                if (lexer.GetNext().value == "(")
                {
                    var exp = ParseOrExpression();
                    if (lexer.GetNext().value != ")") throw new Exception("error");
                    NodeList nl  = ParseStatement();
                    sections.Add(new NodeCondSection() { op = op, condition = exp, statement = nl });
                    NodeList pes = ParseElifSection();
                    if (pes != null)
                    {
                        List<Node> list = pes.list;
                        foreach (Node p in list) { sections.Add(p); };
                    }
                    
                    return new NodeList()
                    {
                        sectionName = op,
                        list = sections
                    };
                }
            }
            throw new Exception("not conditional");
        }

        private NodeList ParseElifSection()
        {
            Token t;
            if ((t = lexer.GetNext()).value == "else") 
            {
                string op = t.value;
                if ((t = lexer.GetNext()).value == "if")
                {
                    lexer.PutBack(t);
                    NodeList pc = ParseConditional();
                    ((NodeCondSection) pc.list[0]).elif = true;
                    return pc;
                }
                lexer.PutBack(t);
                List<Node> sect = new List<Node>();
                sect.Add(new NodeCondSection() { op = op, statement = ParseStatement() });
                return new NodeList() { sectionName = op, list = sect };
            }
            lexer.PutBack(t);
            return null;
        }



    }
}
