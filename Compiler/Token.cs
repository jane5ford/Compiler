using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Compiler
{

    class Token
    {
        public int col { get; set; }
        public int row { get; set; }
        public TokenType type { get; set; }
        public string value { get; set; }
        public Token(int col, int row, TokenType type, string value)
        {
            this.col = col;
            this.row = row;
            this.type = type;
            this.value = value;
        }

        public Token() { }
        public Token(TokenType type) { this.type = type;  }
        public Token(int col, int row) { this.col = col; this.row = row; }
        public Token(int col) { this.col = col; }

    }
}
