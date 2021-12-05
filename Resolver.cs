using System;
using System.Collections.Generic;
using System.Globalization;

namespace Lab4
{
    class Resolver
    {
        // класс для основной работы с операндами

        private List<String> history = new();
        private Dictionary<Action, string> Expressions = new();

        private Action CurrentExpression = null;
        private double Result;

        // функция, которая будет вызываться всякий раз после выполнения какой-либо
        // математической операции, т.е. сложение/деление/взятие косинуса/возведение в квадрат и т.д.
        public Action EventListener = () => { };

        public OperandHandler OperandManager;
        public FunctionHandler FunctionManager;

        private readonly CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();

        public Resolver()
        {
            ci.NumberFormat.NumberDecimalSeparator = ",";
            Expressions.Add(Sum, "+");
            Expressions.Add(Dif, "-");
            Expressions.Add(Div, "/");
            Expressions.Add(Mult, "*");

            OperandManager = new OperandHandler();
            FunctionManager = new FunctionHandler(this);
        }

        public Dictionary<Action, string> GetExpressions()
        {
            return Expressions;
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

            OperandManager.Active().Add(textNum);
        }

        public void PutComma()
        {
            /*
             * добавление запятой к активному операнду, если возможно
             */

            if (OperandManager.Active().GetFunction() == null && !OperandManager.Active().GetText().Contains(","))
            {
                OperandManager.Active().Add(",");
            }
        }

        public void PutNegative()
        {
            /*
             * преобразование активного операнда в отрицательное число, либо,
             * если это функция, обертывание ее в функцию negate
             */

            if (OperandManager.Active().GetText() == "0")
            {
                return;
            }
            if (OperandManager.Active().GetFunction() != null)
            {
                OperandManager.Active().AddFunction(FunctionManager, FunctionManager.Negate);
                return;
            }
            string CurrentNum = OperandManager.Active().GetText();
            OperandManager.Active().SetByStr("-" + CurrentNum);
        }

        public void PutOperator(Action e)
        {
            /*
             * вставка оператора в выражение
             */

            CurrentExpression = e;

            if (OperandManager.LeftIsActive())
            {
                OperandManager.SetRightActive(true);
            } else
            {
                OperandManager.SetLeftActive(true);
            }
        }

        public void PutFunction(Func<double, double> f)
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
            OperandManager.Active().AddFunction(FunctionManager, f);
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
            Result = OperandManager.Left.GetNumber() + OperandManager.Right.GetNumber();
        }

        public void Dif()
        {
            Result = OperandManager.Left.GetNumber() - OperandManager.Right.GetNumber();
        }

        public void Div()
        {
            Result = OperandManager.Left.GetNumber() / OperandManager.Right.GetNumber();
        }

        public void Mult()
        {
            Result = OperandManager.Left.GetNumber() * OperandManager.Right.GetNumber();
        }

        private void FunctionDone()
        {
            // журналирование для операций с одним операндом

            history.Add(String.Format("{0} = {1}",
                OperandManager.Active().GetText(),
                OperandManager.Active().GetNumber()));

            EventListener();
        }

        private void ExpressionDone()
        {
            // журналирование для операций с двумя операндами

            string left = OperandManager.Left.GetText(), 
                operatorSymbol = Expressions.GetValueOrDefault(CurrentExpression),
                right = OperandManager.Right.GetText();

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

            r += OperandManager.Left.GetText();

            if (CurrentExpression != null)
            {
                r += " " + Expressions.GetValueOrDefault(CurrentExpression);

                if (OperandManager.RightIsActive() && !OperandManager.Right.WaitForInput())
                {
                    r += " " + OperandManager.Right.GetText();
                }
            }

            return r;
        }
    }
}
