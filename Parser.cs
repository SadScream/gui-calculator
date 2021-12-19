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
        private string Buffer = ""; // прочитанное на данный момент имя функции/число
        private bool Ok = true;
        private int Position = 0; // переменная, по которой итерируется считывающий цикл для хранения позиция
        private State Current; // текущее состояние
        private Dictionary<string, Func<Decimal, Decimal>> FMap = new(); // соответствие строки и функции по типу sqr, sqrt
        private Dictionary<char, Action> OpMap = new(); // соответствие строки и оператора по типу +, -
        private Stack<Alphabet> AlphabetStack = new Stack<Alphabet>(); // стек букв
        private Stack<string> FunctionStack = new Stack<string>(); // стек функций

        private Resolver resolver;

        public Parser(Resolver parent)
        {
            resolver = parent;

            FillFunctionNames();
            FillOperatorNames();
        }

        public bool EvalOperand()
        {
            /*
             * Преобразовывает прочитанные данные в операнд,
             * применяет над ним функции, сохраненные в процессе
             * чтения в стеке
             */

            string fname;
            bool negative = false;
            Func<Decimal, Decimal> tempf;

            foreach (char number in Buffer)
            {
                if (number == '-')
                    negative = true;
                else if (number == ',')
                    resolver.PutComma();
                else
                    resolver.PutNum(number.ToString());
            }

            if (negative) resolver.PutNegative();
            
            while (FunctionStack.Count() != 0) {
                fname = FunctionStack.Pop();

                if (FMap.TryGetValue(fname, out tempf))
                    resolver.PutFunction(tempf);
                else
                    return false;
            }

            return true;
        }

        public int GetPosition()
        {
            return Position;
        }

        public void ReplaceOnStack(Stack<Alphabet> stack, Alphabet item)
        {
            stack.Pop();
            stack.Push(item);
        }

        public void ReplaceOnStack(Stack<string> stack, string item)
        {
            stack.Pop();
            stack.Push(item);
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
            Action _tmp;
            return OpMap.TryGetValue(c, out _tmp);
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
            /*
             * магазинный автомат
             * получает на вход очередной символ обрабатываемой строки и
             * решает, как с ним нужно поступить и в какое новое состояние
             * перейти
             */

            switch (Current) {
                case State.Start: // стартовое состояние
                    // в любом случае заносим новый символ в буфер
                    // это может быть либо цифра, либо лат. буква, либо знак -
                    PutInBuffer(symbol);

                    if (Letter(symbol))
                    {
                        // если это буква, значит собираемся читать имя функции
                        Current = State.Function;
                        AlphabetStack.Push(Alphabet.F);
                        return true;
                    }
                    if (IsNumber(symbol))
                    {
                        // если цифра, то читаем число
                        Current = State.SInteger;
                        AlphabetStack.Push(Alphabet.I);
                        return true;
                    }
                    if (symbol == '-')
                    {
                        // если -, то читаем отрицательное число
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
                        // когда встретили открывающую строку,
                        // заканчиваем читать имя функции и переходим обратно в стартовое состояние
                        // откуда мы сможем прочесть вложенную функцию или же аргумент настоящей функции

                        FunctionStack.Push(Buffer);
                        ClearBuffer();
                        Current = State.Start;
                        ReplaceOnStack(AlphabetStack, Alphabet.B);
                        return true;
                    }
                    return false;
                case State.Negative:
                    if (IsNumber(symbol))
                    {
                        // читаем цифру и продолжаем читать число дальше

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
                        // если встретили запятую, то начинаем читать
                        // действительное число

                        PutInBuffer(symbol);
                        ReplaceOnStack(AlphabetStack, Alphabet.D);
                        Current = State.SDoube;
                        return true;
                    }
                    if (symbol == ')')
                    {
                        // если встретили закрывающую скобку, то
                        // переходим в состояние, из которого будем
                        // считывать закрывающие скобки до конца

                        AlphabetStack.Pop();
                        Current = State.Close;
                        return true;
                    }
                    if (IsOperator(symbol))
                    {
                        // если прочитали символ, являющийся оператором, то
                        // переходим обратно в стартовое состояние, однако
                        // кладем на стэк значение S, сигнализирующее о том,
                        // что далее мы собираемся читать уже второй операнд.
                        // EvalOperand преобразует прочитанное на данный момент значение
                        // в операнд. PutOperator применяет прочитанный оператор

                        ReplaceOnStack(AlphabetStack, Alphabet.S);
                        bool success = EvalOperand();
                        resolver.PutOperator(OpMap.GetValueOrDefault(symbol));
                        ClearBuffer();
                        Current = State.Start;
                        return success;
                    }
                    return false;
                case State.SDoube: // работает аналогично SInteger
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
                        ReplaceOnStack(AlphabetStack, Alphabet.S);
                        bool success = EvalOperand();
                        resolver.PutOperator(OpMap.GetValueOrDefault(symbol));
                        ClearBuffer();
                        Current = State.Start;
                        return success;
                    }
                    return false;
                case State.Close:
                    if (symbol == ')')
                    {
                        // удаляем закрывающие скобки со стэка
                        AlphabetStack.Pop();
                        return true;
                    }
                    if (IsOperator(symbol))
                    {
                        ReplaceOnStack(AlphabetStack, Alphabet.S);
                        bool success = EvalOperand();
                        resolver.PutOperator(OpMap.GetValueOrDefault(symbol));
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
            // устанавливаем начальные значения
            Current = State.Start;
            ClearBuffer();
            AlphabetStack.Clear();
            FunctionStack.Clear();
            Ok = true;
            AlphabetStack.Push(Alphabet.Z);

            // запихиваем в автомат очередной символ
            for (Position = 0; Position < s.Length; Position++) {
                if (s[Position] == ' ') continue;
                if (s[Position] == '=' || s[Position] == '\n') break;

                Ok = Step(s[Position]);
                if (!Ok) return -1;
            }

            string result = "";

            while (AlphabetStack.Count != 0 ) // читаем остаток стэка
                result += AlphabetStack.Pop();

            if (result == "ISZ" || result == "BSZ" || result == "DSZ" || // два операнда
                result == "BZ" || result == "IZ" || result == "DZ") // только один операнд
            {
                // т.к. в автомате функция EvalOperand вызывается только после чтения символа оператора, то вызываем ее здесь для случая,
                // когда есть второй операнд, либо есть только один

                if (EvalOperand())
                {
                    return 0;
                }
            }

            return -1;
        }

        public void FillFunctionNames()
        {
            foreach (KeyValuePair<Func<Decimal, Decimal>, string> entry in resolver.FunctionManager.GetFunctionMap())
            {
                FMap.Add(entry.Value.Split("(")[0], entry.Key);
            }
        }

        public void FillOperatorNames()
        {
            foreach (KeyValuePair<Action, string> entry in resolver.GetExpressions())
            {
                OpMap.Add(entry.Value[0], entry.Key);
            }
        }
    }
}
