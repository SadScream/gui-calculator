using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    class FunctionManager
    {
        public Dictionary<Func<double, double>, string> functions = new();
        Resolver Parent;

        public FunctionManager(Resolver Parent)
        {
            this.Parent = Parent;
            functions.Add(Negate, "negate({0})");
            functions.Add(Sqrt, "sqrt({0})");
            functions.Add(Sqr, "sqr({0})");
            functions.Add(Rev, "reverse({0})");
            functions.Add(Perc, "proc({0})");
            functions.Add(Cos, "cos({0})");
            functions.Add(Sin, "sin({0})");
            functions.Add(Tg, "tg({0})");
            functions.Add(Ctg, "ctg({0})");
            functions.Add(Ln, "ln({0})");
            functions.Add(Lg, "lg({0})");
        }

        public string GetFormat(Func<double, double> f)
        {
            /*
             * возвращает строковый формат, в котором представляется функция
             */

            return functions[f];
        }

        public double Negate(double arg)
        {
            double r = -1 * arg;
            return r;
        }

        public double Sqrt(double arg)
        {
            double r = Math.Sqrt(arg);
            return r;
        }

        public double Sqr(double arg)
        {
            double r = arg * arg;
            return r;
        }

        public double Rev(double arg)
        {
            double r = 1 / arg;
            return r;
        }

        public double Perc(double arg)
        {
            if (Parent.operand.LeftIsActive() || Parent.operand.Active().WaitForInput())
            {
                Parent.operand.Active().SetDefault();
                return 0;
            }

            double l = Parent.operand.Left.GetNumber();

            if (Parent.GetOperator() == null)
            {
                l = 0;
            }
            double r = l / 100.0 * arg;
            return r;
        }

        public double Cos(double arg)
        {
            double r = Math.Cos(arg);
            return r;
        }

        public double Sin(double arg)
        {
            double r = Math.Sin(arg);
            return r;
        }

        public double Tg(double arg)
        {
            double r = Math.Tan(arg);
            return r;
        }

        public double Ctg(double arg)
        {
            double r = 1.0 / Math.Tan(arg);
            return r;
        }

        public double Ln(double arg)
        {
            double r = Math.Log(arg);
            return r;
        }

        public double Lg(double arg)
        {
            double r = Math.Log10(arg);
            return r;
        }
    }
}
