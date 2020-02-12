using System;
using System.Collections.Generic;

namespace etap1
{
    public enum Type
    {
        NUMBER,
        FUNCTION,
        OPERATOR,
        BRACKET,
        UNKNOWN
    }

    public class Token
    {
        public string value;
        public Type type;

        public Token(string value, Type type)
        {
            this.value = value;
            this.type = type;
        }
    }

    public class Point
    {
        public double x;
        public double y;

        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class Rpn
    {
        private string input { get; set; }
        private List<Token> infixTokens { get; set; }
        private List<Token> rpnTokens { get; set; }

        public Rpn(string input)
        {
            this.input = input;

            this.infixTokens = new List<Token>();
            this.rpnTokens = new List<Token>();

            tokenize();
            fixNegativeValues();
            convertInfixToRpn();
        }

        public bool IsFormulaValid()
        {
            for(int i = 0; i < this.infixTokens.Count; i++)
            {
                if(this.infixTokens[i].type == Type.NUMBER)
                {
                    int dots = 0;
                    foreach(char c in this.infixTokens[i].value)
                    {
                        if(c == '.')
                            dots++;
                        if(dots > 1)
                            return false;
                    }
                }
            }
            for(int i = 0; i < this.infixTokens.Count-1; i++)
            {
                if(this.infixTokens[i].type == Type.OPERATOR && this.infixTokens[i+1].type == Type.OPERATOR)
                {
                    return false;
                }
            }
            return true;
        }

        public List<Token> GetTokens()
        {
            if(!IsFormulaValid())
                throw new RpnInvalidFormulaException();
            return this.rpnTokens;
        }

        private void tokenize()
        {
            string lastValue = "";
            Type lastType = Type.UNKNOWN;

            foreach(char c in this.input)
            {
                if(Char.IsDigit(c) || c == '.' || (c == 'x' && lastValue != "e"))
                {
                    lastValue += c;
                    lastType = Type.NUMBER;
                }
                else if(Char.IsLetter(c))
                {
                    lastValue += c;
                    lastType = Type.FUNCTION;
                }
                else if(c == '(' || c == ')')
                {
                    if(lastType == Type.NUMBER || lastType == Type.FUNCTION)
                    {
                        this.infixTokens.Add(new Token(lastValue, lastType));
                        lastValue = "";
                    }
                    this.infixTokens.Add(new Token(c.ToString(), Type.BRACKET));
                    lastType = Type.BRACKET;
                }
                else if(c == '+' || c == '-' || c == '*' || c == '/' || c == '^')
                {
                    if(lastType == Type.NUMBER || lastType == Type.FUNCTION)
                    {
                        this.infixTokens.Add(new Token(lastValue, lastType));
                        lastValue = "";
                    }
                    this.infixTokens.Add(new Token(c.ToString(), Type.OPERATOR));
                    lastType = Type.OPERATOR;
                }
            }
            if(lastType == Type.NUMBER || lastType == Type.FUNCTION)
            {
                this.infixTokens.Add(new Token(lastValue, lastType));
            }
        }

        private void fixNegativeValues()
        {
            for(int i = 0; i < this.infixTokens.Count-1; i++)
            {
                if(i == 0 && this.infixTokens[i].value == "-" && this.infixTokens[i+1].type == Type.NUMBER)
                {
                    this.infixTokens[i+1].value = "-" + this.infixTokens[i+1].value;
                    this.infixTokens.RemoveAt(i);
                }
                if(i == 0 && this.infixTokens[i].value == "-" && this.infixTokens[i+1].value == "(")
                {
                    this.infixTokens[i].value = "-1";
                    this.infixTokens[i].type = Type.NUMBER;
                    this.infixTokens.Insert(i+1, new Token("*", Type.OPERATOR));
                }
                if(i == 0 && this.infixTokens[i].value == "-" && this.infixTokens[i+1].type == Type.FUNCTION)
                {
                    this.infixTokens[i].value = "-1";
                    this.infixTokens[i].type = Type.NUMBER;
                    this.infixTokens.Insert(i+1, new Token("*", Type.OPERATOR));
                }
                if(i > 2 && this.infixTokens[i].type == Type.NUMBER && this.infixTokens[i-1].value == "-" && this.infixTokens[i-2].value == "(")
                {
                    this.infixTokens[i].value = "-" + this.infixTokens[i].value;
                    this.infixTokens[i].type = Type.NUMBER;
                    this.infixTokens.RemoveAt(i-1);
                }
            }
        }

        private void convertInfixToRpn()
        {
            Queue<Token> Q = new Queue<Token>();
            Stack<Token> S = new Stack<Token>();

            Dictionary<string, int> prededence = new Dictionary<string, int>();
            prededence.Add("abs", 4);
            prededence.Add("cos", 4);
            prededence.Add("exp", 4);
            prededence.Add("log", 4);
            prededence.Add("sin", 4);
            prededence.Add("sqrt", 4);
            prededence.Add("tan", 4);
            prededence.Add("cosh", 4);
            prededence.Add("sinh", 4);
            prededence.Add("tanh", 4);
            prededence.Add("acos", 4);
            prededence.Add("asin", 4);
            prededence.Add("atan", 4);
            prededence.Add("^", 3);
            prededence.Add("/", 2);
            prededence.Add("*", 2);
            prededence.Add("+", 1);
            prededence.Add("-", 1);
            prededence.Add("(", 0);

            for(int i = 0; i < this.infixTokens.Count; i++)
            {
                if(this.infixTokens[i].value == "(")
                {
                    S.Push(this.infixTokens[i]);
                    continue;
                }
                else if(this.infixTokens[i].value == ")")
                {
                    while(S.Peek().value != "(")
                    {
                        Q.Enqueue(S.Pop());
                    }
                    S.Pop();
                    continue;
                }
                else if(prededence.ContainsKey(this.infixTokens[i].value))
                {
                    while(S.Count > 0 && prededence[this.infixTokens[i].value] <= prededence[S.Peek().value])
                    {
                        Q.Enqueue(S.Pop());
                    }
                    S.Push(this.infixTokens[i]);
                    continue;
                }
                else if(this.infixTokens[i].type == Type.NUMBER)
                {
                    Q.Enqueue(this.infixTokens[i]);
                    continue;
                }
                else {
                    throw new RpnInvalidFormulaException("Invalid character");
                }
            }

            while(S.Count > 0)
            {
                Q.Enqueue(S.Pop());
            }

            this.rpnTokens.AddRange(Q.ToArray());
        }

        public double Calculate(double x)
        {
            Stack<double> S = new Stack<double>();
            for(int i = 0; i < this.rpnTokens.Count; i++)
            {
                if(this.rpnTokens[i].type == Type.NUMBER)
                {
                    if(this.rpnTokens[i].value == "x")
                    {
                        S.Push(x);
                    }
                    else
                    {
                        S.Push(Convert.ToDouble(this.rpnTokens[i].value));
                    }
                }
                else if(this.rpnTokens[i].type == Type.FUNCTION)
                {
                    double temp = S.Pop();
                    switch(this.rpnTokens[i].value)
                    {
                        case "abs":
                            temp = Math.Abs(temp);
                            break;
                        case "cos":
                            temp = Math.Cos(temp);
                            break;
                        case "exp":
                            temp = Math.Exp(temp);
                            break;
                        case "log":
                            temp = Math.Log10(temp);
                            break;
                        case "sin":
                            temp = Math.Sin(temp);
                            break;
                        case "sqrt":
                            temp = Math.Sqrt(temp);
                            break;
                        case "tan":
                            temp = Math.Tan(temp);
                            break;
                        case "cosh":
                            temp = Math.Cosh(temp);
                            break;
                        case "sinh":
                            temp = Math.Sinh(temp);
                            break;
                        case "tanh":
                            temp = Math.Tanh(temp);
                            break;
                        case "acos":
                            temp = Math.Acos(temp);
                            break;
                        case "asin":
                            temp = Math.Asin(temp);
                            break;
                        case "atan":
                            temp = Math.Atan(temp);
                            break;
                    }
                    S.Push(temp);
                }
                else if(this.rpnTokens[i].type == Type.OPERATOR)
                {
                    double a = S.Pop();
                    double b = S.Pop();
                    switch(this.rpnTokens[i].value)
                    {
                        case "+":
                            a += b;
                            break;
                        case "-":
                            a = b - a;
                            break;
                        case "*":
                            a *= b;
                            break;
                        case "/":
                            a = b / a;
                            break;
                        case "^":
                            a = Math.Pow(b, a);
                            break;
                    }
                    S.Push(a);
                }
            }
            return S.Pop();
        }

        public double[] Calculate(double xMin, double xMax, int points)
        {
            double delta = (xMax - xMin) / (points - 1);
            List<double> xValues = new List<double>();
            for(int i = 0; i < points-1; i++)
            {
                xValues.Add(xMin + i*delta);
            }
            xValues.Add(xMax);

            double[] results = new double[points];
            for(int i = 0; i < points; i++)
            {
                results[i] = Calculate(xValues[i]);
            }
            return results;
        }

        public List<Point> CalculateXY(double xMin, double xMax, int points)
        {
            List<Point> res = new List<Point>();
            double delta = (xMax - xMin) / (points - 1);
            for(int i = 0; i < points-1; i++)
            {
                double x = xMin + i*delta;
                double y = Calculate(x);
                res.Add(new Point(x, y));
            }
            res.Add(new Point(xMax, Calculate(xMax)));
            return res;
        }
    }

    public class RpnInvalidFormulaException : Exception
    {
        public RpnInvalidFormulaException()
        { }

        public RpnInvalidFormulaException(string message) : base(message)
        { }

        public RpnInvalidFormulaException(string message, Exception inner) : base(message, inner)
        { }
    }
}