using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ExprC_
{
    public class ExprAST
    {
        public ExprAST() 
        {

        }

        public virtual float codeGen()
        {
            return 0.0f;
        }
    }

    public class NumberExprAST : ExprAST
    {
        float val;
        public NumberExprAST(float val)
        {
            this.val = val;
        }

        public override float codeGen()
        {
            return val;
        }
    }

    public class VariableExprAST : ExprAST
    {
        string name;
        Dictionary<string, float> variableValues;
        
        public VariableExprAST(string name, Dictionary<string, float> variableValues)
        {
            this.variableValues = variableValues;
            this.name = name;
        }

        public override float codeGen()
        {
            return variableValues[name];
        }
    }

    public class BinaryExprAST : ExprAST
    {
        ExprAST LHS, RHS;
        string op;
        public BinaryExprAST(string op, ExprAST LHS, ExprAST RHS)
        {
            this.op = op;
            this.LHS = LHS;
            this.RHS = RHS;
        }

        public override float codeGen()
        {
            if(op == "+")
            {
                return LHS.codeGen() + RHS.codeGen();
            }
            else if(op == "-")
            {
                return LHS.codeGen() - RHS.codeGen();
            }
            else if (op == "*")
            {
                return LHS.codeGen() * RHS.codeGen();
            }
            else if (op == "/")
            {
                return LHS.codeGen() / RHS.codeGen();
            }
            else if(op == ">")
            {
                if (LHS.codeGen() > RHS.codeGen()) { return 1.0f; } else { return 0.0f; }
            }
            else if (op == "<")
            {
                if (LHS.codeGen() < RHS.codeGen()) { return 1.0f; } else { return 0.0f; }
            }
            else if(op == "and")
            {
                if(LHS.codeGen() == 1.0f && RHS.codeGen() == 1.0f) {  return 1.0f; } else { return 0.0f; }
            }
            else if (op == "or")
            {
                if (LHS.codeGen() == 1.0f || RHS.codeGen() == 1.0f) { return 1.0f; } else { return 0.0f; }
            }
            else if(op == "=")
            {
                if (LHS.codeGen() == RHS.codeGen()) { return 1.0f; } else { return 0.0f; }
            }
            else
            {
                Console.WriteLine("Error");
            }

            return 0.0f;
        }
    }



    public class Parser
    {
        Lexer lexer;
        Token curTok;
        Dictionary<string, int> BinopPrecedence;
        Dictionary<string, float> variableValues;

        public Parser(string code)
        {
            lexer = new Lexer(code);
            BinopPrecedence = new Dictionary<string, int>();
            BinopPrecedence.Add("or", 2);
            BinopPrecedence.Add("and", 5);
            BinopPrecedence.Add("<", 10);
            BinopPrecedence.Add("=", 10);
            BinopPrecedence.Add(">", 10);
            BinopPrecedence.Add("+", 20);
            BinopPrecedence.Add("-", 20);
            BinopPrecedence.Add("*", 40);

            variableValues = new Dictionary<string, float>();

            variableValues.Add("a", 13.0f);
        }

        //( expr )
        ExprAST ParseParenExpr() 
        {
            curTok = lexer.GetToken();
            var V = ParseExpression();
            if(V == null)
            {
                //Error
            }

            if(curTok.type != Token.TokenType.BRC)
            {
                //Error
            }
            curTok = lexer.GetToken();

            return V;
        }

        ExprAST ParseIdentiExpr()
        {
            string n = curTok.val;
            curTok = lexer.GetToken();

            return new VariableExprAST(n, variableValues);
        }

        ExprAST ParseNumExpr()
        {
            float num = float.Parse(curTok.val, CultureInfo.InvariantCulture.NumberFormat);
            curTok = lexer.GetToken();
            return new NumberExprAST(num);
        }

        int GetTokPrecedence(string tok)
        {
            if (BinopPrecedence.ContainsKey(tok))
            {
                int TokPrec = BinopPrecedence[tok];
                if (TokPrec <= 0) { return -1; }
                return TokPrec;
            }
            else
            {
                return -1;
            }
            
        }

        ExprAST ParseBinOpRHS(int ExprPrec, ExprAST LHS)
        {
            while (true)
            {
                int TokPrec = GetTokPrecedence(curTok.val);

                if (TokPrec < ExprPrec)
                    return LHS;

                string BinOp = curTok.val;
                curTok = lexer.GetToken();

                var RHS = ParsePrimary();
                if(RHS == null)
                {
                    //Error
                }

                int NextPrec = GetTokPrecedence(curTok.val);
                if(TokPrec < NextPrec)
                {
                    RHS = ParseBinOpRHS(TokPrec + 1, RHS);
                    if (RHS == null)
                    {
                        return null;
                    }
                }

                LHS = new BinaryExprAST(BinOp, LHS, RHS);
            }
        }

        ExprAST ParseExpression()
        {
            var LHS = ParsePrimary();
            if (LHS == null) 
            {
                //Error
                Console.WriteLine("Error");
            }
            else
            {
                //Console.WriteLine(curTok.val);
            }

            return ParseBinOpRHS(0, LHS);
        }

        //primary
        ExprAST ParsePrimary()
        {
            //Console.WriteLine(curTok.val);
            switch (curTok.type)
            {
                case Token.TokenType.IDENTI:
                    return ParseIdentiExpr();

                case Token.TokenType.NUM:
                    return ParseNumExpr();

                case Token.TokenType.BRO:
                    return ParseParenExpr();

                default:
                    return null;
            }
        }

        public void Parse()
        {
            curTok = lexer.GetToken();
            Console.WriteLine(curTok.type);
            var expr = ParseExpression();
            if (expr != null)
            {
                var result = expr.codeGen();
                Console.WriteLine(result);
            }
        }
    }
}
