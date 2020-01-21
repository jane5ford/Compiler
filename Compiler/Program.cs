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
            for (int i = 1; i <= 15; i++)
            {
                StreamReader test = File.OpenText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
                    @"C:\Users\ellia\source\repos\Compiler\Compiler\lexer_tests\tests\" + i + ".txt"));
                StreamReader result = File.OpenText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    @"C:\Users\ellia\source\repos\Compiler\Compiler\lexer_tests\results\" + i + ".txt"));
                Lexer lexer = new Lexer(test);
                bool isCorrect = false;
                while (true)
                {
                    Token token = lexer.getNext();
                    if (token.type == TokenType.END_OF_FILE) break;
                    string res = token.row.ToString() + "\t" + token.col.ToString() + "\t" + token.type.ToString() + "\t" + token.value;
                    //if (i==13) Console.WriteLine(res);
                    if (res == result.ReadLine()) { isCorrect = true; }
                    else { isCorrect = false; break; }
                }
                Console.WriteLine("Test {0} : {1}", i, isCorrect);
            }
        }
    }
}
