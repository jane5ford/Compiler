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
            for (int i = 17; i <= 17; i++)
            {
                StreamReader test = File.OpenText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    @"C:\Users\ellia\source\repos\Compiler\Compiler\lexer_tests\tests\" + i + ".txt"));
                //StreamReader result = File.OpenText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                //    @"C:\Users\ellia\source\repos\Compiler\Compiler\lexer_tests\results\" + i + ".txt"));
                Lexer lexer = new Lexer(test);
                bool isCorrect = false;
                while (true)
                {
                    Token token = lexer.GetNext();
                    if (token.type == TokenType.END_OF_FILE) break;
                    //string res = token.row.ToString() + "\t" + token.col.ToString() + "\t" + token.type.ToString() + "\t" + token.value;
                    string res = token.ToString();
                    if (i == 17) Console.WriteLine(res);
                    //if (res == result.ReadLine()) { isCorrect = true; }
                    //else { isCorrect = false; break; }
                }
                Console.WriteLine("Test {0} : {1}", i, isCorrect);
            }
        }
        static void TestParser()
        {
            for(int i = 1; i < 42; i++)
            {
                StreamReader test2 = File.OpenText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                   @"C:\Users\ellia\source\repos\Compiler\Compiler\parser_tests\" + i + ".txt"));
                Lexer lexer2 = new Lexer(test2);
                Parser parser = new Parser(lexer2);
                Node node = new NodeError();
                if (i < 34) node = parser.ParseBlock();
                if (i > 33) node = parser.ParseNamespaceDeclaration();
                if (node is NodeError)
                    Console.WriteLine("Test {0} is error", i);
                //else Console.WriteLine("Test {0} works", i);
                else Console.WriteLine("Test {0}: \n{1}", i, node);
                
                //while (true)
                //{
                //    Token token = lexer2.GetNext();
                //    if (token.type == TokenType.END_OF_FILE) break;
                //    string res = token.ToString();
                //    Console.WriteLine(res);
                //}
            }         
        }
    }
}