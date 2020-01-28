using System.Collections.Generic;

namespace Compiler
{
    abstract class Node
    {
        public sealed override string ToString() => ToString("", true);

        public abstract string ToString(string indent, bool last);

        protected struct LogDecoration
        {
            public string Indent;
            public string Prefix;
        }

        protected LogDecoration GetLogDecoration(string indent, bool last)
        {
            return new LogDecoration
            {
                Indent = indent + (last ? "  " : "| "),
                Prefix = indent + (last ? "└─" : "├─")
            };
        }

    }
    public enum ResType
    {
        NUM, BOOL, LIST, ELEMENT, STRING, CHAR, IDENTIFIER, ERROR
    }
    class NodeIdentifier : Node
    {
        public string name;
        public string type;
        public override string ToString(string indent, bool last)
        {
            var decoration = GetLogDecoration(indent, last);
            if (type == null)
                return decoration.Prefix + string.Format("Variable {0} is not declarated\n", name);
            return decoration.Prefix + string.Format(" NodeIdentifier {0} {1}\n", type, name);
        }
    }
    class NodeIntLiteral : Node
    {
        public int value;
        public override string ToString(string indent, bool last) =>
            GetLogDecoration(indent, true).Prefix + string.Format(" NodeIntValue {0}\n", value);
    }
    class NodeFloatLiteral : Node
    {
        public double value;
        public override string ToString(string indent, bool last) =>
            GetLogDecoration(indent, true).Prefix + string.Format(" NodeFloatValue {0}\n", value);
    }
    class NodeDoubleLiteral : Node
    {
        public double value;
        public override string ToString(string indent, bool last) =>
            GetLogDecoration(indent, true).Prefix + string.Format(" NodeDoubleValue {0}\n", value);
    }
    class NodeBoolLiteral : Node
    {
        public bool value;
        public override string ToString(string indent, bool last) =>
           GetLogDecoration(indent, true).Prefix + string.Format(" NodeBoolValue {0}\n", value);
    }
    class NodeCharLiteral : Node
    {
        public char value;
        public override string ToString(string indent, bool last) =>
           GetLogDecoration(indent, true).Prefix + string.Format(" NodeCharValue {0}\n", value);
    }
    class NodeStringLiteral : Node
    {
        public string value;
        public override string ToString(string indent, bool last) =>
           GetLogDecoration(indent, true).Prefix + string.Format(" NodeStringValue {0}\n", value);
    }
    class NodeVariableType : Node
    {
        public string value;
        public override string ToString(string indent, bool last) =>
           GetLogDecoration(indent, true).Prefix + string.Format(" NodeVariableType {0}\n", value);
    }
    class NodeBinaryOp : Node
    {
        public string op;
        public ResType resType;
        public Node left;
        public Node right;
        public override string ToString(string indent, bool last)
        {
            var decoration = GetLogDecoration(indent, last);
            var res = decoration.Prefix + string.Format(" Binary {0}\n", op);
            res += left.ToString(decoration.Indent, false);
            res += right.ToString(decoration.Indent, true);
            return res;
        }
    }
    class NodeUnaryOp : Node
    {
        public string op;
        public ResType resType;
        public Node left;
        public override string ToString(string indent, bool last)
        {
            var decoration = GetLogDecoration(indent, last);
            var res = decoration.Prefix + string.Format(" Unary {0}\n", op);
            res += left.ToString(decoration.Indent, true);
            return res;
        }
    }
    class NodeCondSection : Node
    {
        public string op;
        public bool elif = false;
        public Node condition;
        public Node statement;
        public override string ToString(string indent, bool last)
        {
            var decoration = GetLogDecoration(indent, last);
            if (elif) op = "else " + op;
            var res = decoration.Prefix + string.Format(" Operator {0}\n", op);
            if (condition != null) res += condition.ToString(decoration.Indent, false);
            res += statement.ToString(decoration.Indent, true);
            return res;
        }
    }
    class NodeList : Node
    {
        public string name;
        public List<Node> list;
        public override string ToString(string indent, bool last)
        {
            var decoration = GetLogDecoration(indent, last);
            var res = decoration.Prefix + string.Format(" {0}\n", name);
            foreach (Node nl in list)
                if (nl == list[list.Count - 1]) res += nl.ToString(decoration.Indent, true);
                else res += nl.ToString(decoration.Indent, false);
            return res;
        }
    }

    class NodeWhileStatement : Node
    {
        public Node expression;
        public Node statement;
        public override string ToString(string indent, bool last)
        {
            var decoration = GetLogDecoration(indent, last);
            var res = decoration.Prefix + string.Format(" Iteration While\n");
            res += expression.ToString(decoration.Indent, false);
            res += statement.ToString(decoration.Indent, true);
            return res;
        }
    }
    class NodeForStatement : Node
    {
        public Node initializer;
        public NodeBinaryOp condition;
        public Node iterator;
        public Node statement;
        public Node jump;
        public override string ToString(string indent, bool last)
        {
            var decoration = GetLogDecoration(indent, last);
            var res = decoration.Prefix + string.Format(" Iteration For\n");
            res += initializer.ToString(decoration.Indent, false);
            res += condition.ToString(decoration.Indent, false);
            res += iterator.ToString(decoration.Indent, false);
            res += statement.ToString(decoration.Indent, true);
            return res;
        }
    }

    class NodeForeachStatement : Node
    {
        public string vt;
        public NodeIdentifier id;
        public Node list;
        public Node statement;
        public override string ToString(string indent, bool last)
        {
            var decoration = GetLogDecoration(indent, last);
            var res = decoration.Prefix + string.Format(" Iteration Foreach\n");
            res += decoration.Prefix + string.Format(" {0}\n", vt);
            res += id.ToString(decoration.Indent, false);
            res += list.ToString(decoration.Indent, false);
            res += statement.ToString(decoration.Indent, true);
            return res;
        }
    }
    class NodeJumpStatement : Node
    {
        public string value;
        public Node expr;
        public override string ToString(string indent, bool last)
        {
            var decoration = GetLogDecoration(indent, last);
            var res = decoration.Prefix + string.Format(" NodeJumpValue {0}\n", value);
            if (expr != null) res += expr.ToString(decoration.Indent, true);
            return res;
        }
    }

    class NodeVariableDeclaration : Node
    {
        public string type;
        public NodeList variables;
        public override string ToString(string indent, bool last)
        {
            var decoration = GetLogDecoration(indent, last);
            var res = decoration.Prefix + string.Format(" VariableDeclaration\n");
            res += variables.ToString(decoration.Indent, true);
            return res;
        }
    }

    class NodeNamespaceDeclaration : Node
    {
        public string name;
        public Node body;
        public override string ToString(string indent, bool last)
        {
            var decoration = GetLogDecoration(indent, last);
            var res = decoration.Prefix + string.Format(" Namespace {0}\n", name);
            res += body.ToString(decoration.Indent, true);
            return res;
        }
    }
    class NodeClassDeclaration : Node
    {
        public List<string> modifiers;
        public NodeIdentifier identifier;
        public Node body;
        public override string ToString(string indent, bool last)
        {
            var decoration = GetLogDecoration(indent, last);
            string res = "";
            string name = identifier.name;
            if (modifiers.Count == 0)
                res = decoration.Prefix + string.Format(" class {0}\n", name);
            if (modifiers.Count == 1)
                res = decoration.Prefix + string.Format(" {0} class {1}\n", modifiers[0], name);
            if (modifiers.Count == 2)
                res = decoration.Prefix + string.Format(" {0} {1} class {2}\n", modifiers[0], modifiers[1], name);
            if (modifiers.Count == 3)
                res = decoration.Prefix + string.Format(" {0} {1} {2} class {3}\n", modifiers[0], modifiers[1], modifiers[2], name);
            res += body.ToString(decoration.Indent, true);
            return res;
        }
    }
    class NodeConstructorDeclaration : Node
    {
        public string modifier;
        public NodeIdentifier identifier;
        public NodeList parameterList;
        public Node body;
        public override string ToString(string indent, bool last)
        {
            var decoration = GetLogDecoration(indent, last);
            string res;
            if (modifier == null) res = decoration.Prefix + string.Format(" {0}\n", identifier.name);
            else res = decoration.Prefix + string.Format(" {0} {1}\n", modifier, identifier.name);
            if (parameterList != null) res += parameterList.ToString(decoration.Indent, false);
            if (body != null) res += body.ToString(decoration.Indent, true);
            return res;
        }
    }
    class NodeMethodDeclaration : Node
    {
        public string modifier;
        public string type;
        public NodeIdentifier identifier;
        public NodeList parameterList;
        public Node body;
        public override string ToString(string indent, bool last)
        {
            var decoration = GetLogDecoration(indent, last);
            string res;
            if (modifier == null) res = decoration.Prefix + string.Format(" {0} {1}\n", type, identifier.name);
            else res = decoration.Prefix + string.Format(" {0} {1} {2}\n", modifier, type, identifier.name);
            if (parameterList != null) res += parameterList.ToString(decoration.Indent, false);
            if (body != null) res += body.ToString(decoration.Indent, true);
            return res;
        }
    }
    class NodeParameter : Node
    {
        public NodeIdentifier identifier;
        public override string ToString(string indent, bool last)
        {
            var decoration = GetLogDecoration(indent, last);
            var res = identifier.ToString(decoration.Indent, true);
            return res;
        }
    }
    class NodeInputParameter : Node
    {
        public string type;
        public Node parameter;
        public override string ToString(string indent, bool last)
        {
            var decoration = GetLogDecoration(indent, last);
            var res = decoration.Prefix + string.Format(" Type {0}\n", type);
            res += parameter.ToString(decoration.Indent, true);
            return res;
        }
    }
    class NodeMethodExpression : Node
    {
        public NodeIdentifier identifier;
        public NodeList parameterList;
        public override string ToString(string indent, bool last)
        {
            var decoration = GetLogDecoration(indent, last);
            var res = decoration.Prefix + string.Format(" Method {0}\n", identifier.name);
            if (parameterList != null) res += parameterList.ToString(decoration.Indent, true);
            return res;
        }
    }
    class NodeEmptyStatement : Node
    {
        public override string ToString(string indent, bool last) =>
            GetLogDecoration(indent, true).Prefix + string.Format(" EmptyStatement\n");
    }
    class NodeError : Node
    {
        public string message;
        public override string ToString(string indent, bool last) =>
           GetLogDecoration(indent, true).Prefix + string.Format(" Error {0}\n", message);
    }
}