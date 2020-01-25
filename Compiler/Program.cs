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
            int i = 33;
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
            if (i > 0 && i < 9) node = parser.ParseExpression();
            if (i == 9) node = parser.ParseStatement();
            if (i > 9 && i < 21) node = parser.ParseConditional();
            if (i > 20 && i < 22) node = parser.ParseRelationalExpession();
            if (i > 23 && i < 27) node = parser.ParseExpression();
            if (i > 26 && i < 31) node = parser.ParseIterationStatement();
            //if (i == 32) node = parser.ParseConditional();
            if (i > 30 && i < 34) node = parser.ParseStatement();
            if (i > 33) node = parser.ParseNamespaceDeclaration();
            Console.WriteLine(node.ToString());
        }
    }
}
