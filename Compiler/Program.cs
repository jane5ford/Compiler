using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.Json;

namespace Compiler
{
    class Program
    {
        
        static void Main(string[] args)
        {
            StreamReader code = File.OpenText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"tests\08.txt"));
            Lexer lexer = new Lexer(code);
            Console.WriteLine("row \t col \t type \t value \t");
            while (true)
            {
                Token token = lexer.getNext();
                if (token.type == TokenType.END_OF_FILE) break;
                string res = token.row + "\t" + token.col.ToString() + "\t" + token.type.ToString() + "\t" + token.value.PadLeft(10) + "\t";
                Console.WriteLine(res);
            }

            //Parser parser = new Parser();
        }
    }
}
