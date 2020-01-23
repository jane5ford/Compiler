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
            if (t.value == "==" || t.value == "!=")
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
            //line.units.Add(u); //добавление в строку ";"
            return line;
        }

        public NodeUnit ParseNewUnit()
        {
            var t = lexer.GetNext();
            return new NodeUnit(t);
        }

        //public Node ParseCondition()
        //{
        //    var t = lexer.GetNext();
        //    if (t.value == "if")
        //    {
        //        string op = t.value;
        //        if (lexer.GetNext().value == "(")
        //        {
        //            var e = ParseOrExp();
        //            if (lexer.GetNext().value != ")") throw new Exception("error");
        //            List<Node> stList = new List<Node>();
        //            NodeStatement s = ParseStatement();
        //            stList.Add(s);
        //            var elst = ParseElse();
        //            stList.Add(elst);
        //            return new NodeCondition()
        //            {
        //                condition = e,
        //                statements = stList
        //            };
        //        }
        //        throw new Exception("error");
        //    }
        //    else { lexer.PutBack(t); return ParseStatement(); } 
        //    throw new Exception("error");
        //}

        //private Node ParseElse()
        //{
        //    Token t = lexer.GetNext();
        //    if (t.value == "else")
        //    {
        //        t = lexer.GetNext();
        //        if (t.value == "if")
        //        {
        //            if (lexer.GetNext().value == "(")
        //            {
        //                var e2 = ParseOrExp();
        //                if (lexer.GetNext().value != ")") throw new Exception("error");
        //                List<Node> stList2 = new List<Node>();
        //                NodeStatement s2 = ParseStatement();
        //                stList2.Add(s2);
        //                var con = new NodeCondition()
        //                {
        //                    condition = e2,
        //                    statements = stList2
        //                };
        //                stList.Add(con);
        //            }
        //        }
        //        else
        //        {
        //            s = ParseStatement();
        //            stList.Add(s);
        //        }
        //    }
        //    else lexer.PutBack(t);
        //}

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
                //lexer.PutBack(t);
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
                    var exp = ParseOrExp();
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
