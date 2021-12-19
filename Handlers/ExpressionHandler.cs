using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

namespace Lab4
{
    class ExpressionHandler
    {
        // класс для основной работы с операндами

        private List<String> history = new(); // история

        private Decimal Result; // результат применения операции
        private string Error = ""; // последняя ошибка
        private string[] Operands = { "+", "-", "/", "*", "%" };

        // функция, которая будет вызываться всякий раз после выполнения какой-либо
        // математической операции, т.е. сложение/деление/взятие косинуса/возведение в квадрат и т.д.
        // устанавливается классом HistoryWindow, для того, чтоб добавлять новый элемент в историю
        // всякий раз после выполнения операции
        public Action EventListener = () => { };

        private UParser parser;
        private string Expression;
        public Operand LastOperand;
        public UFunctionHandler FunctionManager;

        private readonly CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();

        public ExpressionHandler()
        {
            ci.NumberFormat.NumberDecimalSeparator = ",";

            LastOperand = new Operand();
            FunctionManager = new UFunctionHandler(this);
            parser = new UParser(this);
            SetExpressionToDefault();
        }

        public List<String> GetHistory()
        {
            return history;
        }

        public Decimal GetResult()
        {
            return Result;
        }

        public string GetError()
        {
            return Error;
        }

        public void PutNum(string textNum)
        {
            /*
             * добавление цифры к активному операнду
             */

            LastOperand.Add(textNum);
        }

        public void PutComma()
        {
            /*
             * добавление запятой к активному операнду, если возможно
             */

            if (LastOperand.GetFunction() == null && !LastOperand.GetText().Contains(","))
            {
                LastOperand.Add(",");
            }
        }

        public bool PutNegative()
        {
            /*
             * преобразование активного операнда в отрицательное число, либо,
             * если это функция, обертывание ее в функцию negate
             * возвращает true, если было преобразовано в отрицательное
             * и false, если из отрицательного преобразовалось в положительное,
             * либо был ноль
             */

            if (LastOperand.GetText() == "0")
            {
                return false;
            }
            if (LastOperand.GetFunction() != null)
            {
                LastOperand.AddFunction(FunctionManager, FunctionManager.Negate);
                return false;
            }
            Decimal CurrentNum = LastOperand.GetNumber();
            LastOperand.SetByNum(-1 * CurrentNum);

            return CurrentNum > 0;
        }

        public bool PutOperator(string op)
        {
            /*
             * вставка оператора в выражение
             */

            if (!Operands.Contains(op)) {
                return false;
            }

            string space = " ", expr = Expression;

            if (op == "%")
            {
                if (Expression.Length >= 2)
                {
                    string lastOp = Expression[Expression.Length - 2].ToString();

                    if (lastOp != "%" && Operands.Contains(lastOp))
                    {
                        expr = expr.Remove(Expression.Length-3);
                    }
                }

                KeyValuePair<Decimal, string> result = parser.Evaluate(expr);

                if (result.Value.Length == 0)
                {
                    LastOperand.SetByNum(result.Key / 100 * LastOperand.GetNumber());
                    space = "";
                } else
                {
                    Error = result.Value;
                    return false;
                }
            }

            //if (!LastOperand.WaitForInput())
                Expression = Expression + LastOperand.GetText();

            Expression += space + op + space;

            Console.WriteLine(Expression);
            SetOperandToDefault();
            return true;
        }

        public void PutFunction(Func<Decimal, Decimal> f)
        {
            /*
             * Обертывание текущего активного операнда в функцию
             * Для обертывания в negate есть отельная функция PutNegative
             */

            if (f == FunctionManager.Negate)
            {
                PutNegative();
                return;
            }
            LastOperand.AddFunction(FunctionManager, f);
        }

        public bool Solve()
        {
            /*
             * Вычисляет выражение, если оно введено полностью
             */
            
            Expression = Expression + LastOperand.GetText();
            Console.WriteLine("going to solve: {0}", Expression);

            KeyValuePair<Decimal, string> result = parser.Evaluate(Expression); // result & error
            Result = result.Key;

            Console.WriteLine("solver - result: {0}, error: {1}", result.Key, result.Value);

            if (result.Value.Length == 0)
            {
                ExpressionDone();
                LastOperand.SetByNum(Result);
                SetExpressionToDefault();
                return true;
            } else
            {
                Error = result.Value;
            }

            return false;
        }

        private void ExpressionDone()
        {
            // журналирование для операций с двумя операндами

            history.Add(String.Format("{0} = {1}", Expression, Result));
            EventListener();
        }

        public void SetToDefault()
        {
            SetOperandToDefault();
            SetExpressionToDefault();
        }

        public void SetOperandToDefault()
        {
            LastOperand.SetDefault();
        }

        public void SetExpressionToDefault()
        {
            Expression = "";
        }

        public string GetExpression()
        {
            return Expression;
        }
    }
}
