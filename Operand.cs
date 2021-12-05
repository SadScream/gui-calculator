using System;
using System.Globalization;

namespace Lab4
{
    class Operand
    {
        // класс для непосредственной работы с операндом

        private string Format = "{0}"; // то, как будет выводиться операнд. Например, может быть sqrt({0})
        private Func<double, double> Function = null; // последняя функция, в которую был обернут операнд
        private string Text = "0"; // текстовое представление операнда
        private double Number = 0; // числовое представление операнда
        private bool waitForInputStart = true; // начат ли ввод цифр в оператор. Становится true при модификации через Add/AddFunction
        private readonly CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone(); // для правильного вывода

        public Operand(bool waitForInputStart = false)
        {
            ci.NumberFormat.NumberDecimalSeparator = ",";
            this.waitForInputStart = waitForInputStart;
        }

        public void SetDefault()
        {
            /*
             * возвращает значения всех свойств к изначальным
             */

            Format = "{0}";
            Function = null;
            Text = "0";
            Number = 0;
            waitForInputStart = true;
        }

        public void DeleteFunctions()
        {
            Format = "{0}";
            Function = null;
        }

        public void SetWaiting(bool waitForInputStart)
        {
            this.waitForInputStart = waitForInputStart;
        }

        public bool WaitForInput()
        {
            return waitForInputStart;
        }

        public string GetFormat()
        {
            return Format;
        }

        public Func<double, double> GetFunction()
        {
            return Function;
        }

        public void AddFunction(FunctionHandler FManager, Func<double, double> f)
        {
            /*
             * обертывает операнд в указанную функцию
             */

            waitForInputStart = false;

            if (Function != null)
            {
                Number = Function(Number);
            }

            Function = f;
            AddFormat(FManager.GetFormat(f));
        }

        public void SetFormat(string f)
        {
            Format = f;
        }

        public void AddFormat(string f)
        {
            Format = String.Format(f, Format);
        }

        public string GetText()
        {
            return string.Format(Format, Text);
        }

        public double GetNumber()
        {
            if (Function != null)
            {
                return Function(Number);
            }
            return Number;
        }

        public void SetByStr(string s)
        {
            /*
             * Устанавливает числовое и строковое значение операнда по строке
             * Автоматически удаляет функции, в которые обурнут операнд
             */

            DeleteFunctions();
            Text = s;
            Number = double.Parse(s);
        }

        public void SetByNum(double num)
        {
            /*
             * Устанавливает числовое и строковое значение операнда по числу
             * Автоматически удаляет функции, в которые обурнут операнд
             */

            DeleteFunctions();
            Number = num;
            Text = num.ToString();
        }

        public void Add(string d)
        {
            /*
             * Добавляет новый символ к операнду
             */

            waitForInputStart = false;

            if (Text == "0" && d != ",")
            {
                Text = "";
            }
            Text += d;
            Number = double.Parse(Text, ci);
        }
    }
}
