using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    public enum State { Start, Function, Negative, SInteger, SDoube, Close }
    public enum Alphabet
    {
        F, // function
        B, // bracket
        I, // integer
        D, // double
        S, // second operand
        Z, // bottom
    }

    class Parser
    {
        private string Buffer = "";
        private bool Ok = true;
        private State Current;
        public Dictionary<string, Func<double, double>> FMap = new();
        public Dictionary<string, Action> OpMap = new();
        private Stack<Alphabet> AlphabetStack = new Stack<Alphabet>();
        private Stack<string> FunctionStack = new Stack<string>();

        private Resolver OpHandler;

        public Parser(Resolver parent)
        {
            OpHandler = parent;

            FillFunctionNames();
            FillOperatorNames();
        }

        public bool EvalOperand(char Operator = ' ')
        {
            string op = Operator.ToString(),
                fname;
            bool negative = false;
            Func<double, double> tempf;

            foreach (char number in Buffer)
            {
                if (number == '-')
                {
                    negative = true;
                } else if (number == ',')
                {
                    OpHandler.PutComma();
                } else
                {
                    OpHandler.PutNum(number.ToString());
                }
            }

            if (negative)
            {
                OpHandler.PutNegative();
            }

            
            while (FunctionStack.Count() != 0)
            {
                fname = FunctionStack.Pop();

                if (FMap.TryGetValue(fname, out tempf))
                {
                    OpHandler.PutFunction(tempf);
                }
                else
                {
                    return false;
                }
            }

            if (Operator != ' ')
                OpHandler.PutOperator(OpMap.GetValueOrDefault(op));

            return true;
        }

        public void PutInBuffer(char c)
        {
            Buffer += c;
        }

        public void ClearBuffer()
        {
            Buffer = "";
        }

        public bool IsOperator(char c)
        {
            Action tmp;
            return OpMap.TryGetValue(c.ToString(), out tmp);
        }

        public bool IsNumber(char c)
        {
            return (c >= '0' && c <= '9');
        }

        public bool Letter(char c)
        {
            return ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'));
        }

        public bool Step(char symbol)
        {
            switch (Current) {
                case State.Start:
                    PutInBuffer(symbol);

                    if (Letter(symbol))
                    {
                        Current = State.Function;
                        AlphabetStack.Push(Alphabet.F);
                        return true;
                    }
                    if (IsNumber(symbol))
                    {
                        Current = State.SInteger;
                        AlphabetStack.Push(Alphabet.I);
                        return true;
                    }
                    if (symbol == '-')
                    {
                        Current = State.Negative;
                        return true;
                    }
                    return false;
                case State.Function:
                    if (Letter(symbol))
                    {
                        PutInBuffer(symbol);
                        return true;
                    }
                    if (symbol == '(')
                    {
                        FunctionStack.Push(Buffer);
                        ClearBuffer();
                        Current = State.Start;
                        AlphabetStack.Pop();
                        AlphabetStack.Push(Alphabet.B);
                        return true;
                    }
                    return false;
                case State.Negative:
                    if (IsNumber(symbol))
                    {
                        PutInBuffer(symbol);
                        Current = State.SInteger;
                        AlphabetStack.Push(Alphabet.I);
                        return true;
                    }
                    return false;
                case State.SInteger:
                    if (IsNumber(symbol))
                    {
                        PutInBuffer(symbol);
                        return true;
                    }
                    if (symbol == ',')
                    {
                        PutInBuffer(symbol);
                        AlphabetStack.Pop();
                        AlphabetStack.Push(Alphabet.D);
                        Current = State.SDoube;
                        return true;
                    }
                    if (symbol == ')')
                    {
                        AlphabetStack.Pop();
                        Current = State.Close;
                        return true;
                    }
                    if (IsOperator(symbol))
                    {
                        AlphabetStack.Pop();
                        AlphabetStack.Push(Alphabet.S);
                        bool success = EvalOperand(symbol);
                        ClearBuffer();
                        Current = State.Start;
                        return success;
                    }
                    return false;
                case State.SDoube:
                    if (IsNumber(symbol))
                    {
                        PutInBuffer(symbol);
                        return true;
                    }
                    if (symbol == ')')
                    {
                        AlphabetStack.Pop();
                        Current = State.Close;
                        return true;
                    }
                    if (IsOperator(symbol))
                    {
                        AlphabetStack.Pop();
                        AlphabetStack.Push(Alphabet.S);
                        bool success = EvalOperand(symbol);
                        ClearBuffer();
                        Current = State.Start;
                        return success;
                    }
                    return false;
                case State.Close:
                    if (symbol == ')')
                    {
                        AlphabetStack.Pop();
                        return true;
                    }
                    if (IsOperator(symbol))
                    {
                        AlphabetStack.Pop();
                        AlphabetStack.Push(Alphabet.S);
                        bool success = EvalOperand(symbol);
                        ClearBuffer();
                        Current = State.Start;
                        return success;
                    }
                    return false;
            }

            return false;
        }

        public int Parse(string s)
        {
            Current = State.Start;
            ClearBuffer();
            AlphabetStack.Clear();
            FunctionStack.Clear();
            Ok = true;
            AlphabetStack.Push(Alphabet.Z);

            Console.WriteLine(s);

            for (int i = 0; i < s.Length; i++) {
                if (s[i] == ' ') continue;
                if (s[i] == '=') break;
                Console.WriteLine("S: {0}", s[i]);
                Ok = Step(s[i]);


                //Alphabet[] array = new Alphabet[AlphabetStack.Count];
                //AlphabetStack.CopyTo(array, 0);

                //foreach (Alphabet a in array)
                //{
                //    Console.Write(a);
                //}
                //Console.WriteLine();

                Console.WriteLine("Ok: {0}", Ok);
                if (!Ok) return -1;
            }

            string result = "";

            while (AlphabetStack.Count != 0 )
            {
                result += AlphabetStack.Pop();
            }

            if (result == "ISZ" || result == "BSZ" || result == "SZ")
            {
                EvalOperand();
            }

            return 0;
        }

        public void FillFunctionNames()
        {
            foreach (KeyValuePair<Func<double, double>, string> entry in OpHandler.function.functions)
            {
                FMap.Add(entry.Value.Split("(")[0], entry.Key);
            }
        }

        public void FillOperatorNames()
        {
            foreach (KeyValuePair<Action, string> entry in OpHandler.expressions)
            {
                OpMap.Add(entry.Value, entry.Key);
            }
        }
    }
}
