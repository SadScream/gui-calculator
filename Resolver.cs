using System;
using System.Collections.Generic;
using System.Globalization;

namespace Lab4
{
    class Resolver
    {
        public List<String> history = new List<String>();

        private Func<double, double> CurrentFunction = null;
        private Func<double> CurrentOperation = null;
        public Action EventListener = () => { };

        private string LeftOperand = "0", RightOperand = "0", Operator = "";
        private double LeftCalculated = 0, RightCalculated = 0, Result;

        private readonly CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();

        public Resolver()
        {
            ci.NumberFormat.NumberDecimalSeparator = ",";
        }

        public void SetOperator(string v)
        {
            Operator = v;
        }

        public void SetOperand(string v, string format = "{0}")
        {
            double calculated;
            bool left = (CurrentOperation == null);

            if (left && LeftOperand == "0" || !left && RightOperand == "0")
                CurrentFunction = null;

            if (CurrentFunction == null)
            {
                calculated = GetFromStr(v);
            } else
            {
                calculated = CurrentFunction(GetFromStr(v));
                v = String.Format(format, v);
            }

            if (left)
            {
                LeftOperand = v;
                LeftCalculated = calculated;
            } else
            {
                RightOperand = v;
                RightCalculated = calculated;
            }
        }

        public string GetOperandStr()
        {
            if (CurrentOperation == null)
                return LeftOperand;
            else
                return RightOperand;
        }

        public double GetOperandDec()
        {
            if (CurrentOperation == null)
                return LeftCalculated;
            else
                return RightCalculated;
        }

        public void SetCurrentOperation(Func<double> op)
        {
            /*
             * sum, dif, div, mult etc.
             */
            CurrentOperation = op;
        }

        public Func<double> GetCurrentOperation()
        {
            return CurrentOperation;
        }

        public void SetCurrentFunction(Func<double, double> f)
        {
            /*
             * sqrt, sqr, revers etc.
             */
            CurrentFunction = f;
        }

        public Func<double, double> GetCurrentFunction()
        {
            return CurrentFunction;
        }

        public double Sum()
        {
            Result = LeftCalculated + RightCalculated;
            OperationDone();
            return Result;
        }

        public double Dif()
        {
            Result = LeftCalculated - RightCalculated;
            OperationDone();
            return Result;
        }

        public double Div()
        {
            Result = LeftCalculated / RightCalculated;
            OperationDone();
            return Result;
        }

        public double Mult()
        {
            Result = LeftCalculated * RightCalculated;
            OperationDone();
            return Result;
        }

        public double Sqrt(double arg)
        {
            double r = Math.Sqrt(arg);
            OperationDone("sqrt({0}) = {1}", arg, r);
            return r;
        }

        public double Sqr(double arg)
        {
            double r = arg * arg;
            OperationDone("sqr({0}) = {1}", arg, r);
            return r;
        }

        public double Rev(double arg)
        {
            double r = 1 / arg;
            OperationDone("1/{0} = {1}", arg, r);
            return r;
        }

        public double Perc(double arg)
        {
            double l = LeftCalculated;

            if (Operator == "")
            {
                l = 0;
            }
            double r = l / 100.0 * arg;
            OperationDone("{0}% = {1}", arg, r);
            return r;
        }

        public double Cos(double arg)
        {
            double r = Math.Cos(arg);
            OperationDone("cos({0}) = {1}", arg, r);
            return r;
        }

        public double Sin(double arg)
        {
            double r = Math.Sin(arg);
            OperationDone("sin({0}) = {1}", arg, r);
            return r;
        }

        public double Tg(double arg)
        {
            double r = Math.Tan(arg);
            OperationDone("tg({0}) = {1}", arg, r);
            return r;
        }

        public double Ctg(double arg)
        {
            double r = 1.0 / Math.Tan(arg);
            OperationDone("ctg({0}) = {1}", arg, r);
            return r;
        }

        public double Ln(double arg)
        {
            double r = Math.Log(arg);
            OperationDone("ln({0}) = {1}", arg, r);
            return r;
        }

        public double Lg(double arg)
        {
            double r = Math.Log10(arg);
            OperationDone("lg({0}) = {1}", arg, r);
            return r;
        }

        private void OperationDone(string formattedResult, double arg, double result)
        {
            // для операции с одним операндом

            history.Add(String.Format(formattedResult, arg, result));
            EventListener();
        }

        private void OperationDone()
        {
            // для операций с двумя операндами

            history.Add(String.Format("{0:F} {1} {2:F} = {3}", LeftOperand, Operator, RightOperand, Result));
            EventListener();
        }

        public string GetExpressionStr(bool WaitingForRight = false)
        {
            string r = "";

            r += LeftOperand;

            if (Operator != "")
            {
                r += " " + Operator;

                if (!WaitingForRight)
                {
                    r += " " + RightOperand;
                }
            }

            return r;
        }

        public void Clear()
        {
            CurrentFunction = null;
            CurrentOperation = null;
            LeftOperand = "0";
            RightOperand = "0";
            Operator = "";
            LeftCalculated = 0;
            RightCalculated = 0;
            Result = 0;
        }

        private double GetFromStr(string str)
        {
            return double.Parse(str, ci); ;
        }
    }
}
