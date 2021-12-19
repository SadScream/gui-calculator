using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Lab4
{

    public partial class MainWindow : Form
    {
        private const SByte DefaultInputLength = 16;
        private SByte MaxInputLength;

        private ExpressionHandler resolver;

        private Font defaultFont = new("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        private Font middleFont = new("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        private Font smallFont = new("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);

        private HistoryWindow historyWindow;

        public MainWindow()
        {
            InitializeComponent();
            AdditionalDisplay(true);
            resolver = new ExpressionHandler();
            MaxInputLength = DefaultInputLength;
            SetExpression(resolver.GetExpression());
        }

        public void SetExpression(string s)
        {
            CurrentExpressionLabel.Text = s;
        }

        public void SetNumber(string s)
        {
            CurrentNumberLabel.Text = s;
        }

        private void DecreaseFontSize()
        {
            // уменьшаем размер шрифта в зависимости от длины строки

            string currentValue = CurrentNumberLabel.Text;

            if (currentValue.Length == 13)
            {
                CurrentNumberLabel.Font = middleFont;
            }
            else if (currentValue.Length == 17)
            {
                CurrentNumberLabel.Font = smallFont;
            }
        }

        private void IncreaseFontSize()
        {
            // увеличиваем размер шрифта в зависимости от длины строки

            string currentValue = CurrentNumberLabel.Text;

            if (currentValue.Length >= 17)
            {
                return;
            } else if (currentValue.Length >= 13)
            {
                CurrentNumberLabel.Font = middleFont;
            } else
            {
                CurrentNumberLabel.Font = defaultFont;
            }
        }

        private void UpdateInputSettings(SByte AddMaxLen, Action f)
        {
            /*
             * AddMaxLen - насколько нужно увеличить или уменьшить MaxInputLength
             * f - функция, которая будет вызвана в итоге. Подразумевается DecreaseFontSize или IncreaseFontSize
             */

            MaxInputLength += AddMaxLen;
            f();
        }

        private void SetInputSettings(SByte NewMaxLen, Action f)
        {
            /*
             * NewMaxLen - какое новое значение примет MaxInputLength
             * f - функция, которая будет вызвана в итоге. Подразумевается DecreaseFontSize или IncreaseFontSize
             */

            MaxInputLength = NewMaxLen;
            f();
        }

        public void Solve()
        {
            /*
             * Выводит на CurrentNumberLabel результат вычисления
             */

            if (resolver.Solve()) // вычисляем
            {
                SetNumber(resolver.GetResult().ToString()); // выводим
                SetExpression(resolver.GetExpression());
                resolver.LastOperand.SetWaiting(true);

                //// полностью стираем выражение и в качестве левого операнда устанавливаем результат
                //resolver.OperandManager.Right.SetDefault();
                //resolver.OperandManager.Left.SetDefault();
                //resolver.SetExpression(null);

                //SetExpression(resolver.GetExpression());

                //resolver.OperandManager.Left.SetByStr(resolver.GetResult().ToString());
                //resolver.OperandManager.SetLeftActive(true);
            }
        }

        private void SolveClicked(object sender, EventArgs e)
        {
            Solve();
        }

        private void IntKeyClicked(object sender, EventArgs e)
        {
            // обрабатываем нажатие на клавишу с цифрой

            string number = ((Button)sender).Text;
            Operand activeOperand = resolver.LastOperand;

            if (activeOperand.GetFunction() != null)
            {
                // если текущий операнд является функцией, то при вводе числа
                // зануляем этот операнд и записываем на его место вводимое число

                resolver.SetOperandToDefault();
                SetExpression(resolver.GetExpression());
            }

            if (activeOperand.WaitForInput() && activeOperand.GetText() != "0")
            {
                // на случай, когда в качестве одного из операндов выступает вычисленное значение
                // например, когда введем 2+sqrt(1), то в поле для ввода вставится результат вычисление
                // функции sqrt(1). Тогда если после этого мы нажмем на какую-либо цифру, то этот результат
                // должен стереться и вместо него мы вставим вводимое число

                resolver.SetOperandToDefault();
                SetNumber(activeOperand.GetText());
            }

            if (CurrentNumberLabel.Text.Length + 1 >= MaxInputLength)
            {
                return;
            }

            resolver.PutNum(number);

            SetNumber(activeOperand.GetNumber().ToString());
            DecreaseFontSize();
        }

        private void DoubleOperatorClicked(object sender, EventArgs e)
        {
            // нажата кнопка +,-,*,/
            Button b = (Button)sender;
            string operation = b.Name;
            Decimal lastValue = resolver.LastOperand.GetNumber();

            //if (!resolver.LastOperand.WaitForInput())
            //{
            //    // если в данный момент уже введено какое-то выражение и вновь производится клик
            //    // по оператору, то решаем это выражение
            //    Console.WriteLine("double op clicked, waitforinput: {0}", resolver.LastOperand.WaitForInput());
            //    Solve();
            //}

            resolver.PutOperator(b.Text);

            //switch (operation)
            //{
            //    case "PlusButton":
            //        resolver.PutOperator("+");
            //        break;
            //    case "MinusButton":
            //        resolver.PutOperator("-");
            //        break;
            //    case "MuliplyButton":
            //        resolver.PutOperator("*");
            //        break;
            //    case "DivisionButton":
            //        resolver.PutOperator("/");
            //        break;
            //    case "PercentButton":
            //        resolver.PutOperator("%");
            //        break;
            //}

            SetExpression(resolver.GetExpression());
            resolver.LastOperand.SetByNum(lastValue);
            SetNumber(resolver.LastOperand.GetNumber().ToString());
            
            //if (resolver.OperandManager.RightIsActive())
            //{
            //    // устанавливаем в качестве правого оператора то же число, что и для левого оператора,
            //    // чтоб в случае, если пользователь сразу нажмет '=', посчитать результат для одного и того же числа
            //    // как в оригнинальном калькуляторе
            //    resolver.OperandManager.Right.SetByNum(resolver.OperandManager.Left.GetNumber()); 
            //}
        }

        private void SingleOperatorClicked(object sender, EventArgs e)
        {
            // была выбрана операция, для которой требуется один операнд
            // например, возведение в квадрат, взятие квадратного корня, нат. логарифм и тд

            string operation = ((Button)sender).Name;
            string currentStrValue = CurrentNumberLabel.Text;

            switch (operation)
            {
                case "SquareButton": resolver.PutFunction(resolver.FunctionManager.Sqr);break;
                case "RadicalButton": resolver.PutFunction(resolver.FunctionManager.Sqrt); break;
                case "CosButton": resolver.PutFunction(resolver.FunctionManager.Cos); break;
                case "SinButton":resolver.PutFunction(resolver.FunctionManager.Sin);break;
                case "TgButton":resolver.PutFunction(resolver.FunctionManager.Tg);break;
                case "CtgButton":resolver.PutFunction(resolver.FunctionManager.Ctg);break;
                case "LnButton":resolver.PutFunction(resolver.FunctionManager.Ln);break;
                case "LgButton":resolver.PutFunction(resolver.FunctionManager.Lg);break;
                case "ReverseButton":resolver.PutFunction(resolver.FunctionManager.Rev);break;
            }

            SetNumber(resolver.LastOperand.GetNumber().ToString());
            SetExpression(resolver.GetExpression() + resolver.LastOperand.GetText());
            SetInputSettings(DefaultInputLength, IncreaseFontSize);
        }

        private void EditingButtonClicked(object sender, EventArgs e)
        {
            /*
             * была нажата клавиша +/- | , | del | CE | C
             */

            string operation = ((Button)sender).Name;
            string currentValue = resolver.LastOperand.GetText();

            switch (operation)
            {
                case "OpenBracketButton":
                    resolver.PutBracket("(");
                    SetExpression(resolver.GetExpression());
                    break;
                case "CloseBracketButton":
                    resolver.PutBracket(")");
                    SetExpression(resolver.GetExpression());
                    break;
                case "PosOrNegButton":
                    if (resolver.PutNegative())
                        UpdateInputSettings(1, DecreaseFontSize);
                    else
                        UpdateInputSettings(-1, DecreaseFontSize);
                    if (resolver.LastOperand.GetFunction() != null)
                        SetExpression(resolver.GetExpression());
                    break;
                case "CommaButton":
                    resolver.PutComma();
                    UpdateInputSettings(1, DecreaseFontSize);
                    break;
                case "DeleteButton":
                    Operand activeOperand = resolver.LastOperand;

                    if (activeOperand.WaitForInput() || activeOperand.GetText() == "0" || activeOperand.GetFunction() != null)
                        return;

                    if (activeOperand.GetText().Length == 1 ||
                        CurrentNumberLabel.Text.Length == 2 && CurrentNumberLabel.Text.First() == '-')
                    {
                        activeOperand.SetByStr("0");
                        SetNumber(activeOperand.GetNumber().ToString());

                        if (CurrentNumberLabel.Text.First() == '-')
                            UpdateInputSettings(-1, delegate () { });

                        return;
                    }

                    if (CurrentNumberLabel.Text.Contains(','))
                    {
                        UpdateInputSettings(-1, delegate () { });
                    }

                    activeOperand.SetByStr(CurrentNumberLabel.Text.Remove(CurrentNumberLabel.Text.Length - 1));
                    SetNumber(activeOperand.GetNumber().ToString());
                    IncreaseFontSize();
                    break;
                case "CancelEntryButton":
                    resolver.LastOperand.SetDefault();
                    SetNumber(resolver.LastOperand.GetNumber().ToString());
                    SetExpression(resolver.GetExpression());
                    SetInputSettings(DefaultInputLength, IncreaseFontSize);
                    break;
                case "ClearButton":
                    Clear();
                    break;
            }
            SetNumber(resolver.LastOperand.GetText());
        }

        public void Clear()
        {
            resolver.SetToDefault();
            SetNumber(resolver.LastOperand.GetNumber().ToString());
            SetExpression(resolver.GetExpression());
        }

        private void HistoryButtonClicked(object sender, EventArgs e)
        {
            // создает окно с историей операций

            if (historyWindow != null && historyWindow.Visible)
                return;

            historyWindow = new HistoryWindow(this, resolver);
            historyWindow.Show();
        }

        private void AdditionalButtonClicked(object sender, EventArgs e)
        {
            AdditionalDisplay(additionalLayout.Visible);
        }

        private void AdditionalDisplay(bool hide)
        {
            int W = additionalLayout.Width;
            //PercentButton.Enabled = hide;

            if (hide)
            {
                HistoryButton.Location = new System.Drawing.Point(HistoryButton.Location.X - W, 12);
                mainLayout.Location = new System.Drawing.Point(mainLayout.Location.X - W, 41);
                ClientSize = new System.Drawing.Size(ClientSize.Width - W, 388);
                additionalLayout.Hide();
            }
            else
            {
                HistoryButton.Location = new System.Drawing.Point(HistoryButton.Location.X + W, 12);
                mainLayout.Location = new System.Drawing.Point(mainLayout.Location.X + W, 41);
                ClientSize = new System.Drawing.Size(ClientSize.Width + W, 388);
                additionalLayout.Show();
            }
        }
    }
}
