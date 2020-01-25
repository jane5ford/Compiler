﻿using System;
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
        public Node ParseStatementList()
        {
            List<Node> list = new List<Node>();
            var current = ParseStatement();
            if (current is NodeError)
            {
                return new NodeError();
            }
            list.Add(current);
            var st = ParseStatementList();
            if (st == null || st is NodeError) { }
            else
            {
                NodeList next = (NodeList)st;
                foreach (Node n in next.list)
                    list.Add(n);
            }
            return new NodeList() { list = list };
        }

        public Node ParseBlock()
        {
            Token t = lexer.GetNext();
            if (t.value == "{")
            {
                var sl = ParseStatementList();
                if ((t = lexer.GetNext()).value == "}")
                    return sl;
                return new NodeError();
            }
            lexer.PutBack(t);
            var s = ParseStatement();
            return s;
        }

        public Node ParseStatement()
        {
            Node p;
            Token t;
            if ((p = ParseExpression()) == null)
                if ((p = ParseJumpStatement()) == null)
                    if ((p = ParseVariableDeclaration()) == null)
                    {
                        if ((p = ParseConditional()) == null || p is NodeError)
                        {
                            if ((p = ParseIterationStatement()) is NodeError)
                            {
                                t = lexer.GetNext();
                                if (t.type == TokenType.PUNCTUATION) return new NodeEmptyStatement();
                                lexer.PutBack(t);
                                return new NodeError();
                            }
                            return p;
                        }
                        return p;
                    }
            t = lexer.GetNext();
            if (t.type == TokenType.PUNCTUATION)
                return p;
            if (t.type == TokenType.END_OF_FILE) return new NodeError();
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
                        op = t.value,
                        resType = ResType.BOOL
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
            //if (t.type == TokenType.VARTYPE) { return new NodeVariableType { value = t.value }; }
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
                    if (DefineResType(exp) == ResType.BOOL)
                    {
                        if (lexer.GetNext().value != ")") return new NodeError();

                        Node s = ParseBlock();
                        sections.Add(new NodeCondSection() { op = op, condition = exp, statement = s });
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
                    return new NodeError();
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
                sect.Add(new NodeCondSection() { op = op, statement = (NodeList)ParseBlock() });
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
                        var s = ParseBlock();
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
                                    var s = ParseBlock();
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
                    var vt = lexer.GetNext();
                    if (vt.type == TokenType.VARTYPE)
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
                                        var s = ParseBlock();
                                        return new NodeForeachStatement()
                                        {
                                            vt = vt.value, id = (NodeIdentifier)id, list = list, statement = s
                                        };
                                    }
                                }
                            }
                        } 
                    }
                }
                return new NodeError();
            }
            lexer.PutBack(t);
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
            Token t = lexer.GetNext();
            if (t.type == TokenType.VARTYPE)
            {
                var e = ParseExpression();
                if (DefineResType(e) == ResType.IDENTIFIER)
                {
                    return new NodeVariableDeclaration() { type = t.value, id = e };
                }
                return new NodeError();
            }
            lexer.PutBack(t);
            return null;
        }
        
    }
}