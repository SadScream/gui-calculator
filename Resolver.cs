using System;
using System.Collections.Generic;
using System.Globalization;

namespace Lab4
{
    class Resolver
    {
        // класс для основной работы с операндами

        private List<String> history = new();
        private readonly Dictionary<Action, string> expressions = new();

        private Action CurrentExpression = null;
        private double Result;

        public Action EventListener = () => { };

        public OperandManager operand;
        public FunctionManager function;

        private readonly CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();

        public Resolver()
        {
            ci.NumberFormat.NumberDecimalSeparator = ",";
            expressions.Add(Sum, "+");
            expressions.Add(Dif, "-");
            expressions.Add(Div, "/");
            expressions.Add(Mult, "*");

            operand = new OperandManager();
            function = new FunctionManager(this);
        }

        public List<String> GetHistory()
        {
            return history;
        }

        public double GetResult()
        {
            return Result;
        }

        public void SetExpression(Action newExpression)
        {
            CurrentExpression = newExpression;
        }

        public Action GetOperator()
        {
            return CurrentExpression;
        }

        public void PutNum(string textNum)
        {
            /*
             * добавление цифры к активному операнду
             */

            operand.Active().Add(textNum);
        }

        public void PutComma()
        {
            /*
             * добавление запятой к активному операнду, если возможно
             */

            if (operand.Active().GetFunction() == null && !operand.Active().GetText().Contains(","))
            {
                operand.Active().Add(",");
            }
        }

        public void PutNegative()
        {
            /*
             * преобразование активного операнда в отрицательное число, либо,
             * если это функция, обертывание ее в функцию negate
             */

            if (operand.Active().GetText() == "0")
            {
                return;
            }
            if (operand.Active().GetFunction() != null)
            {
                operand.Active().AddFunction(function, function.Negate);
                return;
            }
            string CurrentNum = operand.Active().GetText();
            operand.Active().SetByStr("-" + CurrentNum);
        }

        public void PutOperator(Action e)
        {
            /*
             * вставка оператора в выражение
             */

            CurrentExpression = e;

            if (operand.LeftIsActive())
            {
                operand.SetRightActive(true);
            } else
            {
                operand.SetLeftActive(true);
            }
        }

        public void PutFunction(Func<double, double> f)
        {
            /*
             * Обертывание текущего активного операнда в функцию
             * Для обертывания в negate есть отельная функция PutNegative
             */

            if (f == function.Negate)
            {
                PutNegative();
                return;
            }
            operand.Active().AddFunction(function, f);
            FunctionDone();
        }

        public bool Solve()
        {
            /*
             * Вычисляет выражение, если оно введено полностью
             */

            if (CurrentExpression != null)
            {
                CurrentExpression();
                ExpressionDone();
                return true;
            }
            return false;
        }

        public void Sum()
        {
            Result = operand.Left.GetNumber() + operand.Right.GetNumber();
        }

        public void Dif()
        {
            Result = operand.Left.GetNumber() - operand.Right.GetNumber();
        }

        public void Div()
        {
            Result = operand.Left.GetNumber() / operand.Right.GetNumber();
        }

        public void Mult()
        {
            Result = operand.Left.GetNumber() * operand.Right.GetNumber();
        }

        private void FunctionDone()
        {
            // журналирование для операций с одним операндом

            history.Add(String.Format("{0} = {1}",
                operand.Active().GetText(),
                operand.Active().GetNumber()));

            EventListener();
        }

        private void ExpressionDone()
        {
            // журналирование для операций с двумя операндами

            string left = operand.Left.GetText(), 
                operatorSymbol = expressions.GetValueOrDefault(CurrentExpression),
                right = operand.Right.GetText();

            history.Add(String.Format("{0:F} {1} {2:F} = {3}", 
                left,
                operatorSymbol, 
                right, 
                Result));

            EventListener();
        }

        public string GetExpressionStr()
        {
            /*
             * Переводит имеющиеся операнды и оператор в одну строку
             */

            string r = "";

            r += operand.Left.GetText();

            if (CurrentExpression != null)
            {
                r += " " + expressions.GetValueOrDefault(CurrentExpression);

                if (operand.RightIsActive() && !operand.Right.WaitForInput())
                {
                    r += " " + operand.Right.GetText();
                }
            }

            return r;
        }
    }
}
