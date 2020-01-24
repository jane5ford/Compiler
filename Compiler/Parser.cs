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
        //public Node ParseAdditiveExpression()
        //{
        //    var left = ParseMultiplicativeExpression();
        //    var t = lexer.GetNext();
        //    if (t.value == "+" || t.value == "-")
        //    {
        //        var right = ParseAdditiveExpression();
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
        //private Node ParseMultiplicativeExpression()
        //{
        //    var left = ParseFactor();
        //    var t = lexer.GetNext();
        //    if (t.value == "*" || t.value == "/")
        //    {
        //        var right = ParseMultiplicativeExpression();
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
        private ResType DefineResType(Node operand)
        {
            if (operand is NodeBinaryOp)
            {
                NodeBinaryOp o = (NodeBinaryOp)operand;
                return o.resType;
            }
            if (operand is NodeBoolLiteral) return ResType.BOOL;
            if (operand is NodeIntLiteral || operand is NodeFloatLiteral || operand is NodeDoubleLiteral) return ResType.NUM;
            if (operand is NodeIdentifier) return ResType.IDENTIFIER;
            throw new Exception("error");
        }

        public Node ParseExpression()
        {
            var left = ParseTerm();
            var t = lexer.GetNext();
            if (t.value == "||")
            {
                var right = ParseExpression();
                if ((DefineResType(left) == ResType.BOOL || DefineResType(left) == ResType.IDENTIFIER) &&
                    (DefineResType(right) == ResType.BOOL || DefineResType(right) == ResType.IDENTIFIER))
                {
                    return new NodeBinaryOp
                    {
                        left = left,
                        right = right,
                        op = t.value,
                        resType = ResType.BOOL
                    };
                }
                throw new Exception("error");
            }
            if (t.value == "+" || t.value == "-")
            {
                var right = ParseExpression();
                if ((DefineResType(left) == ResType.NUM || DefineResType(left) == ResType.IDENTIFIER) &&
                     (DefineResType(right) == ResType.NUM || DefineResType(right) == ResType.IDENTIFIER))
                {
                    return new NodeBinaryOp
                    {
                        left = left,
                        right = right,
                        op = t.value,
                        resType = ResType.NUM
                    };
                }
                throw new Exception("error");
            }
            lexer.PutBack(t);
            return left;
        }

        private Node ParseTerm()
        {
            var left = ParseEqualityExpression();
            var t = lexer.GetNext();
            if (t.value == "&&")
            {
                var right = ParseTerm();
                if ((DefineResType(left) == ResType.BOOL || DefineResType(left) == ResType.IDENTIFIER) && 
                    (DefineResType(right) == ResType.BOOL || DefineResType(right) == ResType.IDENTIFIER))
                {
                    return new NodeBinaryOp
                    {
                        left = left,
                        right = right,
                        op = t.value,
                        resType = ResType.BOOL
                    };
                }
                throw new Exception("error");
            }
            if (t.value == "*" || t.value == "/")
            {
                var right = ParseTerm();
                if ((DefineResType(left) == ResType.NUM || DefineResType(left) == ResType.IDENTIFIER) &&
                    (DefineResType(right) == ResType.NUM || DefineResType(right) == ResType.IDENTIFIER))
                {
                    return new NodeBinaryOp
                    {
                        left = left,
                        right = right,
                        op = t.value,
                        resType = ResType.NUM
                    };
                }
                throw new Exception("error");
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
            var left = ParseFactor();
            Token t = lexer.GetNext();
            if (t.value == "<" || t.value == ">" || t.value == "<=" || t.value == ">=")
            {
                var op = t.value;
                var right = ParseExpression();
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
        private Node ParseFactor()
        {
            var t = lexer.GetNext();
            if (t.value == "(")
            {
                var e = ParseExpression();
                if (lexer.GetNext().value != ")") throw new Exception("error");
                return e;
            }
            if (t.type == TokenType.IDENTIFIER) { return new NodeIdentifier { name = t.value }; }
            if (t.type == TokenType.INT) { return new NodeIntLiteral { value = int.Parse(t.value) }; }
            if (t.type == TokenType.FLOAT) { return new NodeFloatLiteral { value = Convert.ToDouble(t.value) }; }
            if (t.type == TokenType.DOUBLE) { return new NodeDoubleLiteral { value = Convert.ToDouble(t.value) }; }
            if (t.type == TokenType.BOOLEAN) { return new NodeBoolLiteral { value = Convert.ToBoolean(t.value) }; }
            if (t.type == TokenType.CHAR) { return new NodeCharLiteral { value = t.value[0] }; }
            if (t.type == TokenType.STRING) { return new NodeStringLiteral { value = t.value }; }
            lexer.PutBack(t);
            throw new Exception("error");
        }

        //private Node ParseLogicFactor()
        //{
        //    var t = lexer.GetNext();
        //    if (t.value == "(")
        //    {
        //        var e = ParseOrExpression();
        //        if (lexer.GetNext().value != ")") throw new Exception("error");
        //        return e;
        //    }
        //    if (t.type == TokenType.IDENTIFIER)
        //    {
        //        return new NodeIdentifier { name = t.value };
        //    }
        //    if (t.type == TokenType.BOOLEAN) { return new NodeBoolean { value = t.value }; }
        //    throw new Exception("error");
        //}

        private Node ParseAssignment()
        {
            var left = ParseFactor();
            if (left is NodeIdentifier)
            {
                Token t = lexer.GetNext();
                if (t.type == TokenType.ASSIGNMENT_OPERATOR)
                {
                    var op = t.value;
                    var right = ParseExpression(); // ??
                    return new NodeBinaryOp
                    {
                        left = left,
                        right = right,
                        op = op
                    };
                }
            }
            return left;
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
                    var exp = ParseExpression();
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
