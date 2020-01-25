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
       
        private ResType DefineResType(Node operand)
        {
            if (operand is NodeBinaryOp)
            {
                NodeBinaryOp o = (NodeBinaryOp)operand;
                return o.resType;
            }
            if (operand is NodeUnaryOp)
            {
                NodeUnaryOp o = (NodeUnaryOp)operand;
                return o.resType;
            }
            if (operand is NodeBoolLiteral) return ResType.BOOL;
            if (operand is NodeIntLiteral || operand is NodeFloatLiteral || operand is NodeDoubleLiteral) return ResType.NUM;
            if (operand is NodeIdentifier) return ResType.IDENTIFIER;
            return ResType.ERROR;
        }
        public Node ParseStatement()
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
        private Node ParseNewLine()
        {
            Token t;
            var vd = ParseVariableDeclaration();
            if (vd is NodeVariableDeclaration) { return vd; }
            //тут проблема в том, что после проверки токен поглощается нодом
            var j = ParseJumpStatement();
            if (j != null)
            {
                t = lexer.GetNext();
                if (t.type == TokenType.PUNCTUATION)
                {
                    return j;
                }
                lexer.PutBack(t);
                return new NodeError();
            }
            var c = ParseConditional();
            if (c != null)
            {
                return c;
            }
            var e = ParseExpression();
            t = lexer.GetNext();
            if (t.type == TokenType.PUNCTUATION)
            {
                return e;
            }
            lexer.PutBack(t);
            return new NodeError();
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
                return new NodeError();
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
                return new NodeError();
            }
            if (t.type == TokenType.ASSIGNMENT_OPERATOR)
            {
                if (left is NodeIdentifier)
                {
                    var op = t.value;
                    var right = ParseExpression();
                    if (right == null)
                    {
                        return new NodeUnaryOp
                        {
                            left = left,
                            op = op,
                            resType = ResType.IDENTIFIER
                        };
                    }
                    return new NodeBinaryOp
                    {
                        left = left,
                        right = right,
                        op = op,
                        resType = ResType.IDENTIFIER
                    };
                }
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
                return new NodeError();
            }
            if (t.value == "*" || t.value == "/")
            {
                var right = ParseTerm();
                
                    return new NodeBinaryOp
                    {
                        left = left,
                        right = right,
                        op = t.value,
                        resType = ResType.NUM
                    };
                
            }
            lexer.PutBack(t);
            return left;
        }
        private Node ParseEqualityExpression()
        {
            var left = ParseRelationalExpession();
            var t = lexer.GetNext();
            if (t.value == "==" || t.value == "!=")
            {
                var right = ParseEqualityExpression();
                if (DefineResType(left) == DefineResType(right) || (DefineResType(left) == ResType.IDENTIFIER 
                    && DefineResType(right) != ResType.IDENTIFIER) || (DefineResType(right) == ResType.IDENTIFIER
                    && DefineResType(left) != ResType.IDENTIFIER))
                {
                    return new NodeBinaryOp
                    {
                        left = left,
                        right = right,
                        op = t.value
                    };
                }
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
                if ((DefineResType(left) == ResType.NUM || DefineResType(left) == ResType.IDENTIFIER) &&
                     (DefineResType(right) == ResType.NUM || DefineResType(right) == ResType.IDENTIFIER))
                {
                    return new NodeBinaryOp()
                    {
                        left = left,
                        right = right,
                        op = op,
                        resType = ResType.BOOL
                    };
                }
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
                if (lexer.GetNext().value != ")") return new NodeError();
                return e;
            }
            if (t.value == "!")
            {
                //Console.WriteLine(lexer.GetNext().value);
                var e = ParseExpression();
                var op = t.value;
                {
                    return new NodeUnaryOp
                    {
                        left = e,
                        op = op,
                        resType = ResType.BOOL
                    };
                }
            }
            if (t.type == TokenType.IDENTIFIER) { return new NodeIdentifier { name = t.value }; }
            if (t.type == TokenType.INT) { return new NodeIntLiteral { value = int.Parse(t.value) }; }
            if (t.type == TokenType.FLOAT) { return new NodeFloatLiteral { value = Convert.ToDouble(t.value) }; }
            if (t.type == TokenType.DOUBLE) { return new NodeDoubleLiteral { value = Convert.ToDouble(t.value) }; }
            if (t.type == TokenType.BOOLEAN) { return new NodeBoolLiteral { value = Convert.ToBoolean(t.value) }; }
            if (t.type == TokenType.CHAR) { return new NodeCharLiteral { value = t.value[0] }; }
            if (t.type == TokenType.STRING) { return new NodeStringLiteral { value = t.value }; }
            if (t.value == "bool" || t.value == "char" || t.value == "double" || t.value == "float" 
                || t.value == "int" || t.value == "string" || t.value == "var") { return new NodeVariableType { value = t.value }; }
            lexer.PutBack(t);
            return null;
        }
        
        public Node ParseConditional()
        {
            Token t;
            if ((t = lexer.GetNext()).value == "if")
            {
                List<Node> sections = new List<Node>();
                string op = "if";
                
                if (lexer.GetNext().value == "(")
                {
                    var exp = ParseExpression();
                    if (lexer.GetNext().value != ")") return new NodeError();
                    NodeList nl  = (NodeList)ParseStatement();
                    sections.Add(new NodeCondSection() { op = op, condition = exp, statement = nl });
                    NodeList pes = (NodeList)ParseElifSection();
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
            lexer.PutBack(t);
            return null;
        }
        private Node ParseElifSection()
        {
            Token t;
            if ((t = lexer.GetNext()).value == "else") 
            {
                string op = t.value;
                if ((t = lexer.GetNext()).value == "if")
                {
                    lexer.PutBack(t);
                    NodeList pc = (NodeList)ParseConditional();
                    ((NodeCondSection) pc.list[0]).elif = true;
                    return pc;
                }
                lexer.PutBack(t);
                List<Node> sect = new List<Node>();
                sect.Add(new NodeCondSection() { op = op, statement = (NodeList)ParseStatement() });
                return new NodeList() { sectionName = op, list = sect };
            }
            lexer.PutBack(t);
            return null;
        }
        public Node ParseIterationStatement()
        {
            Token t = lexer.GetNext();
            if (t.value == "while")
            {
                if (lexer.GetNext().value == "(")
                {
                    var op = t.value;
                    var e = ParseExpression();
                    if (DefineResType(e) == ResType.BOOL)
                    {
                        if (lexer.GetNext().value != ")") return new NodeError();
                        var s = ParseStatement();
                        return new NodeWhileStatement()
                        {
                            expression = e,
                            statement = s,
                        };
                    }
                }
            }
            if (t.value == "for")
            {
                if (lexer.GetNext().value == "(")
                {
                    var op = t.value;
                    var initializer = ParseExpression();
                    if (DefineResType(initializer) == ResType.IDENTIFIER)
                    {
                        if (lexer.GetNext().value == ";")
                        {
                            var condition = ParseRelationalExpession();
                            if (lexer.GetNext().value == ";")
                            {
                                var iterator = ParseExpression();
                                if (DefineResType(iterator) == ResType.IDENTIFIER && lexer.GetNext().value == ")")
                                {
                                    var s = ParseStatement(); //= ParseEmbeddedStatement();
                                    return new NodeForStatement()
                                    {
                                        initializer = initializer,
                                        condition = (NodeBinaryOp)condition,
                                        iterator = iterator,
                                        statement = s,
                                    };
                                }

                            }
                        }
                    }
                }
                return new NodeError();
            }
            if (t.value == "foreach")
            {
                var op = t.value;
                if (lexer.GetNext().value == "(")
                {
                    var vt = ParseFactor();
                    if (vt is NodeVariableType)
                    {
                        var id = ParseFactor();
                        if (id is NodeIdentifier)
                        {
                            if (lexer.GetNext().value == "in")
                            {
                                var list = ParseExpression(); //нет определения списков
                                if (DefineResType(list) == ResType.LIST) 
                                {
                                    if (lexer.GetNext().value == ")")
                                    {
                                        var s = ParseStatement();
                                        return new NodeForeachStatement()
                                        {
                                            vt = (NodeVariableType)vt, id = (NodeIdentifier)id, list = list, statement = s
                                        };
                                    }
                                }
                            }
                        } 
                    }
                }
                return new NodeError();
            }
            return new NodeError();
        }
        private Node ParseJumpStatement()
        {
            Token t = lexer.GetNext();
            if (t.value == "break" || t.value == "return")
            {
                return new NodeJumpStatement() { value = t.value };
            }
            lexer.PutBack(t);
            return null;
        }
        private Node ParseVariableDeclaration()
        {
            var vt = ParseFactor();
            if (vt is NodeVariableType)
            {
                var e = ParseExpression();
                if (DefineResType(e) == ResType.IDENTIFIER)
                {
                    NodeVariableType vtype = (NodeVariableType)vt;
                    Console.WriteLine("works");
                    return new NodeVariableDeclaration() { type = vtype.value, id = e };
                }
                return new NodeError();
            }
            return vt;
        }
    }
}