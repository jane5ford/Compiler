﻿using System;
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
        public override string ToString(string indent, bool last)
        {
            var decoration = GetLogDecoration(indent, true);
            var res = decoration.Prefix + string.Format(" NodeIntValue {0}\n", value);
            return res;
        }
    }
    class NodeBinaryOp : Node
    {
        public string op;
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
    
}
