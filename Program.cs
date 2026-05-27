using ExprC_;
using System;
using System.IO;
using System.Text;

namespace MyApp
{
    internal class Program
    {
        static void Main(string[] args)
        {

            string code;
            using (StreamReader reader = new StreamReader("test.txt", Encoding.UTF8))
            {
                code = reader.ReadToEnd();
            }

            /*
            S
            Lexer lexer = new Lexer(code);
            Token tok = lexer.GetToken();
            while(tok.type != Token.TokenType.EOF && tok != null)
            {
                Console.WriteLine(tok.type);
                tok = lexer.GetToken();
            }
            */

            Parser parser = new Parser(code);
            parser.Parse();
        }
    }

}