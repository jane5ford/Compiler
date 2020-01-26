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

        public Node ParseBlock()
        {

            Token t = lexer.GetNext();
            if (t.value == "{")
            {
                var sl = ParseStatementList();
                if ((t = lexer.GetNext()).value == "}")
                    return sl;
                lexer.PutBack(t);
                return new NodeError();
            }
            lexer.PutBack(t);
            return ParseStatement();
        }
        public Node ParseStatementList()
        {
            Token t;
            List<Node> list = new List<Node>();
            t = lexer.GetNext();
            if (t.type == TokenType.PARETHESES) { lexer.PutBack(t); return null; }
            lexer.PutBack(t);
            var current = ParseStatement();
            if (current is NodeError || current == null)
                return new NodeError();
            list.Add(current);
            var st = ParseStatementList();
            if (st == null || st is NodeError) { }
            else
            {
                NodeList next = (NodeList)st;
                foreach (Node n in next.list)
                    list.Add(n);
            }
            NodeList nl = new NodeList() { name = "block", list = list };
            return nl;
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
        public Node GetBinaryOp(Node left, Node right, string op, ResType opType, ResType resType)
        {
            var defleft = DefineResType(left);
            var defright = DefineResType(right);
            if ((defleft == opType || defleft == ResType.IDENTIFIER) && (defright == opType || defright == ResType.IDENTIFIER || right is NodeMethodExpression))
                return new NodeBinaryOp
                {
                    left = left,
                    right = right,
                    op = op,
                    resType = resType
                };
            return new NodeError();
        }
        public Node ParseExpression()
        {
            var left = ParseTerm();
            var t = lexer.GetNext();
            if (t.value == "||")
            {
                var right = ParseExpression();
                return GetBinaryOp(left, right, t.value, ResType.BOOL, ResType.BOOL);
            }
            if (t.value == "+" || t.value == "-")
            {
                var right = ParseExpression();
                return GetBinaryOp(left, right, t.value, ResType.NUM, ResType.NUM);
            }
            if (t.type == TokenType.ASSIGNMENT_OPERATOR)
            {
                if (left is NodeIdentifier)
                {
                    var right = ParseExpression();
                    if (right == null)
                        return new NodeUnaryOp
                        {
                            left = left,
                            op = t.value,
                            resType = ResType.IDENTIFIER
                        };
                    return new NodeBinaryOp
                    {
                        left = left,
                        right = right,
                        op = t.value,
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
                return GetBinaryOp(left, right, t.value, ResType.BOOL, ResType.BOOL);
            }
            if (t.value == "*" || t.value == "/")
            {
                var right = ParseTerm();
                return GetBinaryOp(left, right, t.value, ResType.NUM, ResType.NUM);
            }
            if (t.value == "(")
            {
                if (left is NodeIdentifier)
                {

                    var parameters = ParseInputParameterList();
                    if ((lexer.GetNext().value == ")"))
                    {
                        return new NodeMethodExpression() { identifier = (NodeIdentifier)left, parameterList = (NodeList)parameters };
                    }
                }
                return new NodeError();
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
                var defleft = DefineResType(left);
                var defright = DefineResType(right);
                if (defleft == defright || (defleft == ResType.IDENTIFIER
                    && defright != ResType.IDENTIFIER) || (defright == ResType.IDENTIFIER
                    && defleft != ResType.IDENTIFIER))
                    return new NodeBinaryOp
                    {
                        left = left,
                        right = right,
                        op = t.value,
                        resType = ResType.BOOL
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
                var right = ParseExpression();
                return GetBinaryOp(left, right, t.value, ResType.NUM, ResType.BOOL);
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
                var rtype = DefineResType(e);
                if (rtype == ResType.BOOL || rtype == ResType.IDENTIFIER)
                    return new NodeUnaryOp
                    {
                        left = e,
                        op = t.value,
                        resType = ResType.BOOL
                    };
                return new NodeError();
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
                            name = op,
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
                    ((NodeCondSection)pc.list[0]).elif = true;
                    return pc;
                }
                lexer.PutBack(t);
                List<Node> sect = new List<Node>();
                sect.Add(new NodeCondSection() { op = op, statement = ParseBlock() });
                return new NodeList() { name = op, list = sect };
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
                return new NodeError();
            }
            if (t.value == "for")
            {
                if (lexer.GetNext().value == "(")
                {
                    var initializer = ParseExpression();
                    if (DefineResType(initializer) == ResType.IDENTIFIER)
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
                return new NodeError();
            }
            if (t.value == "foreach")
            {
                if (lexer.GetNext().value == "(")
                {
                    var vt = lexer.GetNext();
                    if (vt.type == TokenType.VARTYPE)
                    {
                        var id = ParseFactor();
                        if (id is NodeIdentifier)
                            if (lexer.GetNext().value == "in")
                            {
                                var list = ParseExpression(); //нет определения списков
                                if (DefineResType(list) == ResType.LIST)
                                    if (lexer.GetNext().value == ")")
                                    {
                                        var s = ParseBlock();
                                        return new NodeForeachStatement()
                                        {
                                            vt = vt.value,
                                            id = (NodeIdentifier)id,
                                            list = list,
                                            statement = s
                                        };
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
                if (t.value == "break")
                    return new NodeJumpStatement() { value = t.value };
            if (t.value == "return")
            {
                //сделать после парсинга методов и функций
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
                    return new NodeVariableDeclaration() { type = t.value, id = e };
                return new NodeError();
            }
            lexer.PutBack(t);
            return null;
        }
        public Node ParseNamespaceDeclaration()
        {
            Token t = lexer.GetNext();
            if (t.value == "namespace")
            {
                if ((t = lexer.GetNext()).type == TokenType.IDENTIFIER)
                    return new NodeNamespaceDeclaration() { name = t.value, body = ParseNamespaceBody() };
                return new NodeError();
            }
            lexer.PutBack(t);
            return new NodeError();
        }
        private Node ParseNamespaceBody()
        {
            Token t = lexer.GetNext();
            if (t.value == "{")
            {
                var c = ParseClassDeclaration();
                if (lexer.GetNext().value == "}") return c;
                return new NodeError();
            }
            lexer.PutBack(t);
            return new NodeError();
        }
        private Node ParseClassDeclaration()
        {
            Token t = lexer.GetNext();
            List<string> modifiers = new List<string>();
            if (t.value == "public" || t.value == "private" || t.value == "protected")
            {
                modifiers.Add(t.value);
                t = lexer.GetNext();
            }
            if (t.value == "abstract" || t.value == "static")
            {
                modifiers.Add(t.value);
                t = lexer.GetNext();
            }
            if (t.value == "class")
            {
                var identifier = ParseFactor();
                if (identifier is NodeIdentifier)
                    return new NodeClassDeclaration() { modifiers = modifiers, identifier = (NodeIdentifier)identifier, body = ParseClassBody() };
            }
            if (modifiers.Count == 0) lexer.PutBack(t);
            return new NodeError();
        }
        private Node ParseClassBody()
        {
            Token t = lexer.GetNext();
            if (t.value == "{")
            {
                var cm = ParseClassMemberDeclarations();
                if (lexer.GetNext().value == "}") return cm;
                return new NodeError();
            }
            lexer.PutBack(t);
            return new NodeError();
        }
        private Node ParseClassMemberDeclarations()
        {
            Token t = lexer.GetNext();
            if (t.value != "}" && t.type != TokenType.END_OF_FILE)
            {
                lexer.PutBack(t);
                List<Node> list = new List<Node>();
                list.Add(ParseClassMemberDeclaration());
                var cmd = ParseClassMemberDeclarations();
                if (cmd is null) { }
                else
                {
                    NodeList next = (NodeList)cmd;
                    foreach (Node n in next.list)
                        list.Add(n);
                }
                return new NodeList() { list = list, name = "Class Member Declarations" };
            }
            lexer.PutBack(t);
            return null;
        }
        private Node ParseClassMemberDeclaration()
        {
            Node p;
            if ((p = ParseVariableDeclaration()) == null)
            {
                if ((p = ParseConstructorDeclaration()) == null)
                {
                    return null;
                }
                return p;
            }
            Token t = lexer.GetNext();
            if (t.type == TokenType.PUNCTUATION)
                return p;
            lexer.PutBack(t);
            return new NodeError();
        }
        private Node ParseConstructorDeclaration()
        {
            Token t = lexer.GetNext();
            string modifier = null;
            if (t.value == "public" || t.value == "private" || t.value == "protected")
                modifier = t.value;
            else lexer.PutBack(t);
            t = lexer.GetNext();
            if (t.type == TokenType.IDENTIFIER)
            {
                lexer.PutBack(t);
                var identifier = ParseFactor();
                if (identifier is NodeIdentifier)
                {
                    if ((t = lexer.GetNext()).value == "(")
                    {
                        var pl = ParseParameterList();
                        {
                            if ((t = lexer.GetNext()).value == ")")
                            {
                                return new NodeConstructorDeclaration()
                                {
                                    modifier = modifier,
                                    identifier = (NodeIdentifier)identifier,
                                    parameterList = (NodeList)pl,
                                    body = ParseBlock()
                                };
                            }
                        }
                    }
                }
                return new NodeError();
            }
            if (t.value == "void" || t.type == TokenType.VARTYPE)
            {
                string type = t.value;
                var identifier = ParseFactor();
                if (identifier is NodeIdentifier)
                {
                    if (lexer.GetNext().value == "(")
                    {
                        var pl = ParseParameterList();
                        if (lexer.GetNext().value == ")")
                            return new NodeMethodDeclaration()
                            {
                                modifier = modifier,
                                type = type,
                                identifier = (NodeIdentifier)identifier,
                                parameterList = (NodeList)pl,
                                body = ParseBlock()
                            };
                    }
                }
                return new NodeError();
            }
            lexer.PutBack(t);
            return null;
        }
        private Node ParseParameterList()
        {
            Token t;
            List<Node> list = new List<Node>();
            var current = ParseParameter();
            if (current is NodeParameter)
            {
                list.Add(current);
                if ((t = lexer.GetNext()).value == ",")
                {
                    NodeList next = (NodeList)ParseParameterList();
                    if (next != null)
                        foreach (Node n in next.list)
                            list.Add(n);
                    else if (current is NodeError) return new NodeError();
                    return new NodeList() { name = "Parameters", list = list };
                }
                lexer.PutBack(t);
                return new NodeList() { name = "Parameters", list = list };
            }
            if (current is NodeError) return new NodeError();
            return null;
        }
        private Node ParseParameter()
        {
            Token t = lexer.GetNext();
            if (t.type == TokenType.VARTYPE)
            {
                var identifier = ParseFactor();
                if (identifier is NodeIdentifier)
                    return new NodeParameter() { type = t.value, identifier = (NodeIdentifier)identifier };
                return new NodeError();
            }
            lexer.PutBack(t);
            return null;
        }
        private Node ParseInputParameterList()
        {
            Token t;
            List<Node> list = new List<Node>();
            var current = ParseInputParameter();
            if (current is NodeInputParameter)
            {
                list.Add(current);
                if ((t = lexer.GetNext()).value == ",")
                {
                    NodeList next = (NodeList)ParseInputParameterList();
                    if (next != null)
                        foreach (Node n in next.list)
                            list.Add(n);
                    else if (current is NodeError) return new NodeError();
                    return new NodeList() { name = "Parameters", list = list };
                }
                lexer.PutBack(t);
                return new NodeList() { name = "Parameters", list = list };
            }
            if (current is NodeError) return new NodeError();
            return null;
        }
        private Node ParseInputParameter()
        {
            Token t = lexer.GetNext();
            string type = null;
            Node parameter;
            if (t.type == TokenType.VARTYPE)
            {
                type = t.value;
            }
            else lexer.PutBack(t);
            var exp = ParseExpression();
            if (exp == null || exp is NodeError)
            {
                if (type != null) lexer.PutBack(t);
                return null;
            }
            parameter = exp;
            if (type != null)
            {
                if (exp is NodeIdentifier) { }
                else return new NodeError();
            }
            return new NodeInputParameter() { type = type, parameter = parameter };
        }
    }
}