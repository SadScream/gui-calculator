using System;
using System.Globalization;
using System.Collections.Generic;

namespace Lab4
{
    class Operand
    {
        // класс для непосредственной работы с операндом

        private string Format = "{0}"; // то, как будет выводиться операнд. Например, может быть sqrt({0})
        private Func<Decimal, Decimal> Function = null; // последняя функция, в которую был обернут операнд
        private string Text = "0"; // текстовое представление операнда
        private Decimal Number = 0; // числовое представление операнда
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

            DeleteFunctions();
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

        public Func<Decimal, Decimal> GetFunction()
        {
            return Function;
        }

        public void AddFunction(FunctionHandler FManager, Func<Decimal, Decimal> f)
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

        public void AddFunction(UFunctionHandler FManager, Func<Decimal, Decimal> f)
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

        public void AddFormat(string f)
        {
            Format = String.Format(f, Format);
        }

        public void SetFormat(string f)
        {
            Format = f;
        }

        public string GetText()
        {
            return string.Format(Format, Text);
        }

        public Decimal GetNumber()
        {
            if (Function != null)
            {
                return Function(Number);
            }
            return Number;
        }

        public void SetByStr(string s) {
            /*
             * Устанавливает числовое и строковое значение операнда по строке. Автоматически удаляет функции, в которые обурнут операнд
             */

            DeleteFunctions();
            Text = s;
            Number = Decimal.Parse(s);
        }

        public void SetByNum(Decimal num) {
            /*
             * Устанавливает числовое и строковое значение операнда по числу. Автоматически удаляет функции, в которые обурнут операнд
             */

            DeleteFunctions();
            Number = num;
            Text = num.ToString();
        }

        public void Add(string d) {
            /*
             * Добавляет новый символ к операнду
             */

            waitForInputStart = false;

            if (Text == "0" && d != ",")
            {
                Text = "";
            }
            Text += d;
            Number = Decimal.Parse(Text, ci);
        }
    }
}
