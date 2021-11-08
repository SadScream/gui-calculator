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

        public void setOperator(string v)
        {
            Operator = v;
        }

        public void setOperand(string v, string format = "{0}")
        {
            double calculated;
            bool left = (CurrentOperation == null);

            if (left && LeftOperand == "0" || !left && RightOperand == "0")
                CurrentFunction = null;

            if (CurrentFunction == null)
            {
                calculated = getFromStr(v);
            } else
            {
                calculated = CurrentFunction(getFromStr(v));
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

        public string getOperandStr()
        {
            if (CurrentOperation == null)
                return LeftOperand;
            else
                return RightOperand;
        }

        public double getOperandDec()
        {
            if (CurrentOperation == null)
                return LeftCalculated;
            else
                return RightCalculated;
        }

        public void setCurrentOperation(Func<double> op)
        {
            /*
             * sum, dif, div, mult etc.
             */
            CurrentOperation = op;
        }

        public Func<double> getCurrentOperation()
        {
            return CurrentOperation;
        }

        public void setCurrentFunction(Func<double, double> f)
        {
            /*
             * sqrt, sqr, revers etc.
             */
            CurrentFunction = f;
        }

        public Func<double, double> getCurrentFunction()
        {
            return CurrentFunction;
        }

        public double Sum()
        {
            Result = LeftCalculated + RightCalculated;
            history.Add(String.Format("{0:F6} {1} {2:F} = {3}", LeftOperand, Operator, RightOperand, Result));
            EventListener();
            return Result;
        }

        public double Dif()
        {
            Result = LeftCalculated - RightCalculated;
            history.Add(String.Format("{0:F} {1} {2:F} = {3}", LeftOperand, Operator, RightOperand, Result));
            EventListener();
            return Result;
        }

        public double Div()
        {
            Result = LeftCalculated / RightCalculated;
            history.Add(String.Format("{0:F} {1} {2:F} = {3}", LeftOperand, Operator, RightOperand, Result));
            EventListener();
            return Result;
        }

        public double Mult()
        {
            Result = LeftCalculated * RightCalculated;
            history.Add(String.Format("{0:F} {1} {2:F} = {3}", LeftOperand, Operator, RightOperand, Result));
            EventListener();
            return Result;
        }

        public double Sqrt(double arg)
        {
            double r = Math.Sqrt(arg);
            history.Add(String.Format("sqrt({0}) = {1}", arg, r));
            EventListener();
            return r;
        }

        public double Sqr(double arg)
        {
            double r = arg * arg;
            history.Add(String.Format("sqr({0}) = {1}", arg, r));
            EventListener();
            return r;
        }

        public double Rev(double arg)
        {
            double r = 1 / arg;
            history.Add(String.Format("1/{0} = {1}", arg, r));
            EventListener();
            return r;
        }

        public double Perc(double arg)
        {
            double r = LeftCalculated / 100.0 * arg;
            history.Add(String.Format("% {0} = {1}", arg, r));
            EventListener();
            return r;
        }

        public string getExpressionStr(bool WaitingForRight = false)
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

        private double getFromStr(string str)
        {
            return double.Parse(str, ci); ;
        }
    }
}
