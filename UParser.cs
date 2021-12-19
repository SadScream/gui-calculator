using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    //класс исключающий для ошибок для анализатора
    class ParserException : ApplicationException
    {
        public ParserException(string str) : base(str) { }
        public override string ToString()
        { return Message; }
    }

    class UParser
    {
        //перчисляем типы лексем.
        enum Types { NONE, DELIMITER, FUNCTION, NUMBER };
        // Перечисляем типы ошибок.
        enum Errors { SYNTAX, UNBALPARENS, NOEXP, FUNCNOTEXIST, DIVBYZERO };

        private Dictionary<string, Func<Decimal, Decimal>> FMap = new(); // соответствие строки и функции по типу sqr, sqrt
        private Dictionary<char, Action> OpMap = new(); // соответствие строки и оператора по типу +, -
        Stack<string> FunctionStack = new Stack<string>(); // стек функций
        string exp; // Ссылка на строку выражения,
        int expIdx; // Текущий индекс в выражении,
        string token; // Текущая лексема.
        Types tokType; // Тип лексемы.

        private ExpressionHandler resolver;

        public UParser(ExpressionHandler parent)
        {
            resolver = parent;

            FillFunctionNames();
        }

        public KeyValuePair<Decimal, string> Evaluate(string expstr)
        {
            Operand operand = new Operand();
            exp = expstr;
            expIdx = 0;
            KeyValuePair<Decimal, string> result;

            try
            {
                GetToken();
                if (token == "")
                {
                    SyntaxErr(Errors.NOEXP); // Выражение отсутствует,
                    result = new KeyValuePair<Decimal, string>((Decimal)0.0, "empty error");
                    return result;
                }
                EvalExp2(operand);
                if (token != "") // Последняя лексема должна быть нулевой.
                    SyntaxErr(Errors.SYNTAX);
                result = new KeyValuePair<Decimal, string>(operand.GetNumber(), "");
                return result;
            }
            catch (ParserException exc)
            {
                // При желании добавляем здесь обработку ошибок.
                result = new KeyValuePair<Decimal, string>((Decimal)0.0, exc.ToString());
                return result;
            }
        }

        // Складываем или вычитаем два члена выражения.
        void EvalExp2(Operand operand)
        {
            string op;
            Operand partialOperand = new Operand();

            EvalExp3(operand);

            while ((op = token) == "+" || op == "-")
            {
                GetToken();
                EvalExp3(partialOperand);

                if (tokType == Types.DELIMITER && token == "%")
                {
                    Console.WriteLine("perc1 - l: {0}; r: {1}%", operand.GetNumber(), partialOperand.GetNumber());
                    partialOperand.SetByNum(operand.GetNumber() / (Decimal)100.0 * partialOperand.GetNumber());
                    GetToken();
                }

                switch (op)
                {
                    case "-":
                        Console.WriteLine("minus - l: {0}; r: {1}", operand.GetNumber(), partialOperand.GetNumber());
                        operand.SetByNum(operand.GetNumber() - partialOperand.GetNumber());
                        break;
                    case "+":
                        Console.WriteLine("plus - l: {0}; r: {1}", operand.GetNumber(), partialOperand.GetNumber());
                        operand.SetByNum(operand.GetNumber() + partialOperand.GetNumber());
                        break;
                }
            }
        }

        // Выполняем умножение или деление двух множителей.
        void EvalExp3(Operand operand)
        {
            string op;
            Operand partialOperand = new Operand();
            EvalExp4(operand);
            while ((op = token) == "*" || op == "/")
            {
                GetToken();
                EvalExp4(partialOperand);

                if (tokType == Types.DELIMITER && token == "%")
                {
                    Console.WriteLine("perc2 - l: {0}; r: {1}%", operand.GetNumber(), partialOperand.GetNumber());
                    partialOperand.SetByNum(operand.GetNumber() / (Decimal)100.0 * partialOperand.GetNumber());
                    GetToken();
                }

                switch (op)
                {
                    case "*":
                        Console.WriteLine("mult - l: {0}; r: {1}", operand.GetNumber(), partialOperand.GetNumber());
                        operand.SetByNum(operand.GetNumber() * partialOperand.GetNumber());
                        break;
                    case "/":
                        Console.WriteLine("div - l: {0}; r: {1}", operand.GetNumber(), partialOperand.GetNumber());
                        if (partialOperand.GetNumber() == (Decimal)0.0)
                            SyntaxErr(Errors.DIVBYZERO);
                        operand.SetByNum(operand.GetNumber() / partialOperand.GetNumber());
                        break;
                }
            }
        }

        // Выполненяем операцию унарного + или -.
        void EvalExp4(Operand operand)
        {
            string op;

            op = "";
            if ((tokType == Types.DELIMITER) && token == "+" || token == "-")
            {
                op = token;
                GetToken();
            }
            EvalExp5(operand);
            if (op == "-") operand.SetByNum(-operand.GetNumber());
        }

        // обрабатываем выражение в круглых скобках
        void EvalExp5(Operand operand)
        {
            if ((token == "("))
            {
                GetToken();
                EvalExp2(operand);
                if (token != ")")
                    SyntaxErr(Errors.UNBALPARENS);
                GetToken();
            } else if (tokType == Types.FUNCTION)
            {
                EvalExp6(operand);
            }
            else
            {
                Atom(operand);
            }
        }

        // Обрабатываем функцию
        void EvalExp6(Operand operand)
        {
            Func<Decimal, Decimal> tempf;

            if (FMap.TryGetValue(token, out tempf))
                FunctionStack.Push(token);
            else
                SyntaxErr(Errors.FUNCNOTEXIST);

            GetToken();
            if (token != "(")
            {
                SyntaxErr(Errors.UNBALPARENS);
            }
            else
            {
                GetToken();

                EvalExp2(operand);

                if (token != ")")
                    SyntaxErr(Errors.UNBALPARENS);

                FMap.TryGetValue(FunctionStack.Pop(), out tempf);
                operand.SetByNum(tempf(operand.GetNumber()));

                GetToken();
            }
        }

        // Получаем значение числа
        void Atom(Operand operand)
        {
            switch (tokType)
            {
                case Types.NUMBER:
                    try
                    {
                        operand.SetByStr(token);
                    }
                    catch (FormatException)
                    {
                        operand.SetByNum((Decimal)0.0);
                        SyntaxErr(Errors.SYNTAX);
                    }
                    GetToken();
                    return;
                default:
                    operand.SetByNum((Decimal)0.0);
                    SyntaxErr(Errors.SYNTAX);
                    break;
            }
        }

        // Обрабатываем синтаксическую ошибку
        void SyntaxErr(Errors error)
        {
            string[] err ={
                         "Синтаксическая ошибка",
                         "Дисбаланс скобок",
                         "Выражение отсутствет",
                         "Функция не существует",
                         "Деление на нуль"};
            throw new ParserException(err[(int)error]);
        }

        // получем следующую лексему.
        void GetToken()
        {
            tokType = Types.NONE;
            token = "";
            if (expIdx == exp.Length) return; // Конец выражения.

            // Опускаем пробел.
            while (expIdx < exp.Length && Char.IsWhiteSpace(exp[expIdx])) ++expIdx;

            // Хвостовой пробел завершает выражение.
            if (expIdx == exp.Length) return;

            if (IsDelim(exp[expIdx]))
            {
                token += exp[expIdx];
                expIdx++;
                tokType = Types.DELIMITER;
            }
            else if (IsLetter(exp[expIdx]))
            {
                // Это функция?
                while (!IsDelim(exp[expIdx]))
                {
                    token += exp[expIdx];
                    expIdx++;
                    if (expIdx >= exp.Length) break;
                }

                //FunctionStack.Push(token);
                tokType = Types.FUNCTION;
            }
            else if (IsNumber(exp[expIdx]))
            {
                // Это число?
                while (!IsDelim(exp[expIdx]))
                {
                    token += exp[expIdx];
                    expIdx++;
                    if (expIdx >= exp.Length) break;
                }
                tokType = Types.NUMBER;
            } else
            {
                // неожиданный символ
                Console.WriteLine("Неожиданный токен '{0}'", exp[expIdx]);

                if (exp[expIdx] == '=')
                {
                    return;
                }

                SyntaxErr(Errors.SYNTAX);
            }
        }

        public void FillFunctionNames()
        {
            foreach (KeyValuePair<Func<Decimal, Decimal>, string> entry in resolver.FunctionManager.GetFunctionMap())
            {
                FMap.Add(entry.Value.Split("(")[0], entry.Key);
            }
        }

        public bool IsNumber(char c)
        {
            return (c >= '0' && c <= '9');
        }

        public bool IsLetter(char c)
        {
            return ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'));
        }

        bool IsDelim(char c)
        {
            if ((" +-/*()%".IndexOf(c) != -1))
                return true;
            return false;
        }
    }
}
