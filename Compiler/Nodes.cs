using System;
using System.Collections.Generic;
using System.Text;

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

    class NodeIdentifier : Node
    {
        public string name;
        public override string ToString(string indent, bool last) =>
            GetLogDecoration(indent, true).Prefix + string.Format(" NodeIdentifier {0}\n", name);
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

    public enum ResType
    {
        NUM, BOOL, LIST, ELEMENT, STRING, IDENTIFIER
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

    class NodeLine : Node
    {
        public List<NodeUnit> units;
        private string res;

        public override string ToString(string indent, bool last)
        {
            foreach (NodeUnit u in units)
                res += u.value + " ";
            return GetLogDecoration(indent, true).Prefix + string.Format(" NodeLine {0}\n", res);
        }
    }

    class NodeUnit : Node
    {
        public Token token { get; set; }
        public String value;
        public NodeUnit(Token token)
        {
            this.token = token;
            value = token.value;
        }
        public override string ToString(string indent, bool last) =>
           GetLogDecoration(indent, true).Prefix + string.Format(" Node {0}\n", value);
    }

    class NodeCondSection : Node
    {
        public string op;
        public bool elif = false;
        public Node condition;
        public NodeList statement;
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
        public string sectionName;
        public List<Node> list;
        public override string ToString(string indent, bool last)
        {
            var decoration = GetLogDecoration(indent, last);
            var res = decoration.Prefix + string.Format(" Statement {0}\n", sectionName);
            foreach (Node nl in list)
                if (nl == list[list.Count - 1]) res += nl.ToString(decoration.Indent, true);
                else res += nl.ToString(decoration.Indent, false);
            return res;
        }
    }



}
