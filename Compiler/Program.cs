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
            //TestLexer();
            TestParser();
        }

        static void TestLexer()
        {
            for (int i = 16; i <= 16; i++)
            {
                StreamReader test = File.OpenText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    @"C:\Users\ellia\source\repos\Compiler\Compiler\lexer_tests\tests\" + i + ".txt"));
                StreamReader result = File.OpenText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    @"C:\Users\ellia\source\repos\Compiler\Compiler\lexer_tests\results\" + i + ".txt"));
                Lexer lexer = new Lexer(test);
                bool isCorrect = false;
                while (true)
                {
                    Token token = lexer.GetNext();
                    if (token.type == TokenType.END_OF_FILE) break;
                    //string res = token.row.ToString() + "\t" + token.col.ToString() + "\t" + token.type.ToString() + "\t" + token.value;
                    string res = token.ToString();
                    //if (i==16) Console.WriteLine(res);
                    if (res == result.ReadLine()) { isCorrect = true; }
                    else { isCorrect = false; break; }
                }
                Console.WriteLine("Test {0} : {1}", i, isCorrect);
            }
        }
        static void TestParser()
        {
            int i = 17;
            StreamReader test2 = File.OpenText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    @"C:\Users\ellia\source\repos\Compiler\Compiler\parser_tests\" + i + ".txt"));
            Lexer lexer2 = new Lexer(test2);
            //while (true)
            //{
            //    Token token = lexer2.GetNext();
            //    if (token.type == TokenType.END_OF_FILE) break;
            //    string res = token.ToString();
            //    Console.WriteLine(res);
            //}
            Parser parser = new Parser(lexer2);
            Node node = null;
            if (i == 1) node = parser.ParseExpression();
            if (i > 1 && i < 9) node = parser.ParseOrExp();
            if (i == 9) node = parser.ParseNewLine();
            if (i > 9) node = parser.ParseConditional();
            Console.WriteLine(node.ToString());
        }
    }
}
