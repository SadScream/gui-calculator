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

        private Resolver OpHandler;
        private bool WaitForRightOperand = false;

        private Font defaultFont = new("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        private Font middleFont = new("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        private Font smallFont = new("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);

        private HistoryWindow historyWindow;

        public MainWindow()
        {
            InitializeComponent();
            AdditionalDisplay(true);
            OpHandler = new Resolver();
            MaxInputLength = DefaultInputLength;
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

        private void Solve(bool DoubleOp=true)
        {
            CurrentExpressionLabel.Text = OpHandler.GetExpressionStr();

            if (OpHandler.GetCurrentOperation() != null && DoubleOp)
            {
                Func<double> RunCurrentOperation = OpHandler.GetCurrentOperation();

                CurrentNumberLabel.Text = RunCurrentOperation().ToString(); // исполняем текущую операцию
                OpHandler.SetCurrentFunction(null);
                OpHandler.SetOperand("0"); // устанавливаем значение правого операнда на 0
                OpHandler.SetOperator(""); // убираем оператор
                OpHandler.SetCurrentOperation(null); // удаляем текущую операцию

                // т.к. текущая операция удалена, то теперь левый операнд поменяется на CurrentNumberLabel.Text
                OpHandler.SetOperand(CurrentNumberLabel.Text);
            } else
            {
                double r = OpHandler.GetOperandDec();
                CurrentNumberLabel.Text = r.ToString();
            }

            // print history
            for (int i = 0; i < OpHandler.history.Count(); i++)
            {
                Console.WriteLine(OpHandler.history[i]);
            }

            Console.WriteLine("\n");
            //
        }

        private void SolveClicked(object sender, EventArgs e)
        {
            Solve();
        }

        private void IntKeyClicked(object sender, EventArgs e)
        {
            // обрабатываем нажатие на клавишу с цифрой

            if (WaitForRightOperand)
            {
                // если истинно, то в данный момент CurrentNumberLaber содержит последнее
                // вычесленное/введенное число, которое использовалось бы как второй операнд,
                // если бы пользователь не начал ввод, а так как он начал, то зануляем текущее значение

                CurrentNumberLabel.Text = "0";
                WaitForRightOperand = false;
            }

            if (OpHandler.GetCurrentFunction() != null)
            {
                CurrentNumberLabel.Text = "0";
                OpHandler.SetCurrentFunction(null);
            }

            if (CurrentNumberLabel.Text.Length >= MaxInputLength)
            {
                return;
            }

            if (CurrentNumberLabel.Text == "0")
            {
                CurrentNumberLabel.Text = "";

                if (OpHandler.GetCurrentOperation() != null)
                {
                    CurrentExpressionLabel.Text = OpHandler.GetExpressionStr(true);
                }
            }
                
            CurrentNumberLabel.Text = CurrentNumberLabel.Text + ((Button)sender).Text;
            OpHandler.SetOperand(CurrentNumberLabel.Text);
            DecreaseFontSize();
        }

        private void DoubleOperatorClicked(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            string operation = b.Name;

            if (OpHandler.GetCurrentOperation() != null && !WaitForRightOperand)
            {
                // если в данный момент уже введено какое-то выражение и вновь производится клик
                // по оператору, то решаем это выражение и устанавливаем в качестве левого операнда результат
                Solve();
            }

            OpHandler.SetOperator(b.Text);

            switch (operation)
            {
                case "PlusButton":
                    OpHandler.SetCurrentOperation(OpHandler.Sum);
                    break;
                case "MinusButton":
                    OpHandler.SetCurrentOperation(OpHandler.Dif);
                    break;
                case "MuliplyButton":
                    OpHandler.SetCurrentOperation(OpHandler.Mult);
                    break;
                case "DivisionButton":
                    OpHandler.SetCurrentOperation(OpHandler.Div);
                    break;
            }

            WaitForRightOperand = true;
            CurrentExpressionLabel.Text = OpHandler.GetExpressionStr(WaitForRightOperand);

            // устанавливаем в качестве правого оператора то же число, что и для левого оператора,
            // чтоб в случае, если пользователь сразу нажмет '=', посчитать результат для одного и того же числа
            // как в оригнинальном калькуляторе
            OpHandler.SetOperand(CurrentNumberLabel.Text);
        }

        private void SingleOperatorClicked(object sender, EventArgs e)
        {
            // была выбрана операция, для которой требуется один операнд
            // например, возведение в квадрат, взятие квадратного корня, нат. логарифм и тд

            string operation = ((Button)sender).Name;
            string currentStrValue = CurrentNumberLabel.Text;
            double inputValue = OpHandler.GetOperandDec(), r;

            switch (operation)
            {
                case "SquareButton":
                    OpHandler.SetCurrentFunction(OpHandler.Sqr);
                    OpHandler.SetOperand(inputValue.ToString(), "sqr({0})");
                    break;
                case "RadicalButton":
                    OpHandler.SetCurrentFunction(OpHandler.Sqrt);
                    OpHandler.SetOperand(inputValue.ToString(), "sqrt({0})");
                    break;
                case "CosButton":
                    OpHandler.SetCurrentFunction(OpHandler.Cos);
                    OpHandler.SetOperand(inputValue.ToString(), "cos({0})");
                    break;
                case "SinButton":
                    OpHandler.SetCurrentFunction(OpHandler.Sin);
                    OpHandler.SetOperand(inputValue.ToString(), "sin({0})");
                    break;
                case "TgButton":
                    OpHandler.SetCurrentFunction(OpHandler.Tg);
                    OpHandler.SetOperand(inputValue.ToString(), "tg({0})");
                    break;
                case "CtgButton":
                    OpHandler.SetCurrentFunction(OpHandler.Ctg);
                    OpHandler.SetOperand(inputValue.ToString(), "ctg({0})");
                    break;
                case "LnButton":
                    OpHandler.SetCurrentFunction(OpHandler.Ln);
                    OpHandler.SetOperand(inputValue.ToString(), "ln({0})");
                    break;
                case "LgButton":
                    OpHandler.SetCurrentFunction(OpHandler.Lg);
                    OpHandler.SetOperand(inputValue.ToString(), "lg({0})");
                    break;
                case "ReverseButton":
                    r = OpHandler.Rev(inputValue);
                    OpHandler.SetOperand(r.ToString());
                    break;
                case "PercentButton":
                    r = OpHandler.Perc(inputValue);
                    OpHandler.SetOperand(r.ToString());
                    break;
            }

            CurrentNumberLabel.Text = OpHandler.GetOperandDec().ToString();
            CurrentExpressionLabel.Text = OpHandler.GetExpressionStr();
            SetInputSettings(DefaultInputLength, IncreaseFontSize);
            Solve(false);
        }

        private void EditingButtonClicked(object sender, EventArgs e)
        {
            /*
             * была нажата клавиша +/- | , | del | ce | c
             */

            string operation = ((Button)sender).Name;
            string currentValue = OpHandler.GetOperandStr();

            switch (operation)
            {
                case "PosOrNegButton":
                    if (currentValue == "0")
                        break;

                    SByte koeff = -1;

                    if (CurrentNumberLabel.Text[0] != '-')
                    {
                        koeff = 1;
                        OpHandler.SetOperand("-" + currentValue);
                        CurrentNumberLabel.Text = OpHandler.GetOperandStr();
                    }
                    else
                    {
                        OpHandler.SetOperand(currentValue.Substring(1));
                        CurrentNumberLabel.Text = OpHandler.GetOperandStr();
                    }

                    UpdateInputSettings(koeff, DecreaseFontSize);
                    break;
                case "CommaButton":
                    if (!CurrentNumberLabel.Text.Contains(','))
                    {
                        OpHandler.SetOperand(currentValue + ",");
                        CurrentNumberLabel.Text = OpHandler.GetOperandStr();
                        UpdateInputSettings(1, DecreaseFontSize);
                    }
                    break;
                case "DeleteButton":
                    string operand = OpHandler.GetOperandStr();

                    if (operand == "0" || WaitForRightOperand || OpHandler.GetCurrentFunction() != null)
                        return;

                    if (operand.Length == 1)
                    {
                        OpHandler.SetOperand("0");
                        CurrentNumberLabel.Text = OpHandler.GetOperandStr();
                        return;
                    }
                    else if (CurrentNumberLabel.Text.Length == 2 && CurrentNumberLabel.Text.First() == '-')
                    {
                        OpHandler.SetOperand("0");
                        CurrentNumberLabel.Text = OpHandler.GetOperandStr();
                        UpdateInputSettings(-1, delegate () { });
                        return;
                    }

                    char LastCh = CurrentNumberLabel.Text.Last();

                    if (LastCh == ',')
                    {
                        UpdateInputSettings(-1, delegate () { });
                    }

                    OpHandler.SetOperand(CurrentNumberLabel.Text.Remove(CurrentNumberLabel.Text.Length - 1));
                    CurrentNumberLabel.Text = OpHandler.GetOperandStr();
                    IncreaseFontSize();
                    break;
                case "CancelEntryButton":
                    bool cf = (OpHandler.GetCurrentFunction() == null),
                         co = (OpHandler.GetCurrentOperation() == null);

                    CurrentNumberLabel.Text = "0";

                    if (co)
                    {
                        OpHandler.SetOperand(CurrentNumberLabel.Text);
                        OpHandler.SetCurrentFunction(null);
                        return;
                    }

                    if (!cf)
                    {
                        OpHandler.SetCurrentFunction(null);
                        OpHandler.SetOperand(CurrentNumberLabel.Text);
                    }

                    if (!co)
                    {
                        WaitForRightOperand = true;
                    }

                    CurrentExpressionLabel.Text = OpHandler.GetExpressionStr(WaitForRightOperand);
                    SetInputSettings(DefaultInputLength, IncreaseFontSize);
                    break;
                case "ClearButton":
                    OpHandler.Clear();
                    CurrentExpressionLabel.Text = "0";
                    CurrentNumberLabel.Text = "0";
                    break;
            }
        }

        private void HistoryButtonClicked(object sender, EventArgs e)
        {
            if (historyWindow != null && historyWindow.Visible)
                return;

            historyWindow = new HistoryWindow(OpHandler);
            historyWindow.Show();
        }

        private void AdditionalButtonClicked(object sender, EventArgs e)
        {
            AdditionalDisplay(additionalLayout.Visible);
        }

        private void AdditionalDisplay(bool hide)
        {
            if (hide)
            {
                HistoryButton.Location = new System.Drawing.Point(225, 12);
                mainLayout.Location = new System.Drawing.Point(12, 41);
                ClientSize = new System.Drawing.Size(271, 388);
                additionalLayout.Hide();
            }
            else
            {
                HistoryButton.Location = new System.Drawing.Point(288, 12);
                mainLayout.Location = new System.Drawing.Point(75, 41);
                ClientSize = new System.Drawing.Size(334, 388);
                additionalLayout.Show();
            }
        }
    }
}
