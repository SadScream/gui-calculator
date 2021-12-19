using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    class UFunctionHandler
    {
        private Dictionary<Func<Decimal, Decimal>, string> functions = new();
        private ExpressionHandler ExpHandler;

        public UFunctionHandler(ExpressionHandler parent)
        {
            ExpHandler = parent;
            functions.Add(Negate, "negate({0})");
            functions.Add(Sqrt, "sqrt({0})");
            functions.Add(Sqr, "sqr({0})");
            functions.Add(Rev, "reverse({0})");
            functions.Add(Cos, "cos({0})");
            functions.Add(Sin, "sin({0})");
            functions.Add(Tg, "tg({0})");
            functions.Add(Ctg, "ctg({0})");
            functions.Add(Ln, "ln({0})");
            functions.Add(Lg, "lg({0})");
            functions.Add(Exp, "exp({0})");
        }

        public Dictionary<Func<Decimal, Decimal>, string> GetFunctionMap()
        {
            return functions;
        }

        public string GetFormat(Func<Decimal, Decimal> f)
        {
            /*
             * возвращает строковый формат, в котором представляется функция
             */

            return functions[f];
        }

        public Decimal Negate(Decimal arg)
        {
            Decimal r = -1 * arg;
            return r;
        }

        public Decimal Exp(Decimal arg)
        {
            Decimal r = (Decimal)Math.Exp((double)arg);
            return r;
        }

        public Decimal Sqrt(Decimal arg)
        {
            Decimal r = (Decimal)Math.Sqrt((double)arg);
            return r;
        }

        public Decimal Sqr(Decimal arg)
        {
            Decimal r = arg * arg;
            return r;
        }

        public Decimal Rev(Decimal arg)
        {
            Decimal r = 1 / arg;
            return r;
        }

        public Decimal Cos(Decimal arg)
        {
            Decimal r = (Decimal)Math.Cos((double)arg);
            return r;
        }

        public Decimal Sin(Decimal arg)
        {
            Decimal r = (Decimal)Math.Sin((double)arg);
            return r;
        }

        public Decimal Tg(Decimal arg)
        {
            Decimal r = (Decimal)Math.Tan((double)arg);
            return r;
        }

        public Decimal Ctg(Decimal arg)
        {
            Decimal r = (Decimal)(1.0 / Math.Tan((double)arg));
            return r;
        }

        public Decimal Ln(Decimal arg)
        {
            Decimal r = (Decimal)Math.Log((double)arg);
            return r;
        }

        public Decimal Lg(Decimal arg)
        {
            Decimal r = (Decimal)Math.Log10((double)arg);
            return r;
        }
    }
}
