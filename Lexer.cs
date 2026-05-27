using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace ExprC_
{
    public class Token
    {
        public enum TokenType
        {
            AND,
            OR,

            BRO,
            BRC,
            VAR,
            NUM,
            EOF,

            EQ,
            SEMI,
            OP,

            GT,
            LT,
            

            IDENTI,
        }

        public TokenType type;
        public string val = "";

        public Token(TokenType type, string val)
        {
            this.type = type;
            this.val = val;
        }
    }

    public class Lexer
    {
        string code;
        int index = 0;

        public Lexer(string code)
        {
            this.code = code;
        }

        public Token GetToken()
        {
            //Skip whitesace usw
            if(index >= code.Length) { return new Token(Token.TokenType.EOF, "EOF"); }
            while ((code[index] == ' ' || code[index] == '\n' || code[index] == '\r') && index < code.Length) 
            {
                index++;
            }
            if (index >= code.Length) { return new Token(Token.TokenType.EOF, "EOF"); }

            if (Char.IsLetter(code[index]))
            {
                string str = "";
                while(index < code.Length && Char.IsLetterOrDigit(code[index]))
                {
                    str += code[index];
                    index++;
                }

                //Nachher meherere keywords adden
                if(str == "and") { return new Token(Token.TokenType.AND, "and"); }
                else if(str == "or") { return new Token(Token.TokenType.OR, "or"); }
                else { return new Token(Token.TokenType.IDENTI, str); }
            }

            if (Char.IsDigit(code[index])) 
            {
                string str = "";
                while (index < code.Length && (Char.IsDigit(code[index]) || code[index] == '.'))
                {
                    str += code[index];
                    index++;
                }

                return new Token(Token.TokenType.NUM, str);
            }

            if (code[index] == '(')
            {
                index++;
                return new Token(Token.TokenType.BRO, "(");
            }

            if (code[index] == ')')
            {
                index++;
                return new Token(Token.TokenType.BRC, ")");
            }

            if (code[index] == '=')
            {
                index++;
                return new Token(Token.TokenType.EQ, "=");
            }

            if (code[index] == '>')
            {
                index++;
                return new Token(Token.TokenType.GT, ">");
            }

            if (code[index] == '<')
            {
                index++;
                return new Token(Token.TokenType.LT, "<");
            }

            if (code[index] == ';')
            {
                index++;
                return new Token(Token.TokenType.SEMI, ";");
            }

            if (code[index] == '+' || code[index] == '*' || code[index] == '-' || code[index] == '/')
            {
                string str = "";
                str += code[index];
                Token tok = new Token(Token.TokenType.OP, str);
                index++;
                return tok;
            }

            

            return null;
        }
    }
}
