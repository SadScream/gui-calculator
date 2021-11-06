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

        public MainWindow()
        {
            InitializeComponent();

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
             * аналог ChangeState, но в отличие от нее устанавливает новые значения,
             * а не меняет старые
             */

            MaxInputLength = NewMaxLen;
            f();
        }

        private void Solve(bool DoubleOp=true)
        {
            CurrentExpressionLabel.Text = OpHandler.getExpressionStr();

            if (OpHandler.getCurrentOperation() != null && DoubleOp)
            {
                CurrentNumberLabel.Text = OpHandler.getCurrentOperation()().ToString();
                OpHandler.setOperand("0"); // устанавливаем значение правого операнда на 0
                OpHandler.setOperator(""); // убираем оператор
                OpHandler.setCurrentOperation(null); // удаляем текущую операцию

                // т.к. текущая операция удалена, то теперь левый операнд поменяется на значение аргумента
                OpHandler.setOperand(CurrentNumberLabel.Text);
            } else
            {
                double r = OpHandler.getOperandDec();
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

            if (OpHandler.getCurrentFunction() != null)
            {
                CurrentNumberLabel.Text = "0";
                OpHandler.setCurrentFunction(null);
            }

            if (CurrentNumberLabel.Text.Length >= MaxInputLength)
            {
                return;
            }

            if (CurrentNumberLabel.Text == "0")
            {
                CurrentNumberLabel.Text = "";

                if (OpHandler.getCurrentOperation() != null)
                {
                    CurrentExpressionLabel.Text = OpHandler.getExpressionStr(true);
                }
            }
                
            CurrentNumberLabel.Text = CurrentNumberLabel.Text + ((Button)sender).Text;
            OpHandler.setOperand(CurrentNumberLabel.Text);
            DecreaseFontSize();
        }

        private void DoubleOperatorClicked(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            string operation = b.Name;

            if (OpHandler.getCurrentOperation() != null)
            {
                // если в данный момент уже введено какое-то выражение и вновь производится клик
                // по оператору, то решаем это выражение и устанавливаем в качестве левого операнда результат
                Solve();
            }

            OpHandler.setOperator(b.Text);

            switch (operation)
            {
                case "PlusButton":
                    OpHandler.setCurrentOperation(OpHandler.Sum);
                    break;
                case "MinusButton":
                    OpHandler.setCurrentOperation(OpHandler.Dif);
                    break;
                case "MuliplyButton":
                    OpHandler.setCurrentOperation(OpHandler.Mult);
                    break;
                case "DivisionButton":
                    OpHandler.setCurrentOperation(OpHandler.Div);
                    break;
            }

            WaitForRightOperand = true;
            CurrentExpressionLabel.Text = OpHandler.getExpressionStr(WaitForRightOperand);

            // устанавливаем в качестве правого оператора то же число, что и для левого оператора,
            // чтоб в случае, если пользователь сразу нажмет '=', посчитать результат для одного и того же числа
            // как в оригнинальном калькуляторе
            OpHandler.setOperand(CurrentNumberLabel.Text);
        }

        private void SingleOperatorClicked(object sender, EventArgs e)
        {
            // была выбрана операция, для которой требуется один операнд
            // например, возведение в квадрат, взятие квадратного корня, нат. логарифм и тд

            string operation = ((Button)sender).Name;
            string currentStrValue = CurrentNumberLabel.Text;
            double inputValue = OpHandler.getOperandDec();

            switch (operation)
            {
                case "SquareButton":
                    OpHandler.setCurrentFunction(OpHandler.Sqr);
                    OpHandler.setOperand(inputValue.ToString(), "sqr({0})");
                    break;
                case "RadicalButton":
                    OpHandler.setCurrentFunction(OpHandler.Sqrt);
                    OpHandler.setOperand(inputValue.ToString(), "sqrt({0})");
                    break;
            }

            CurrentNumberLabel.Text = OpHandler.getOperandDec().ToString();
            CurrentExpressionLabel.Text = OpHandler.getExpressionStr();
            SetInputSettings(DefaultInputLength, IncreaseFontSize);
            Solve(false);
        }

        private void EditingButtonClicked(object sender, EventArgs e)
        {
            /*
             * была нажата клавиша +/- | , | del | ce | c
             */

            string operation = ((Button)sender).Name;
            string currentValue = OpHandler.getOperandStr();

            switch (operation)
            {
                case "PosOrNegButton":
                    if (currentValue == "0")
                        break;

                    SByte koeff = -1;

                    if (CurrentNumberLabel.Text[0] != '-')
                    {
                        koeff = 1;
                        OpHandler.setOperand("-" + currentValue);
                        CurrentNumberLabel.Text = OpHandler.getOperandStr();
                    }
                    else
                    {
                        OpHandler.setOperand(currentValue.Substring(1));
                        CurrentNumberLabel.Text = OpHandler.getOperandStr();
                    }

                    UpdateInputSettings(koeff, DecreaseFontSize);
                    break;
                case "CommaButton":
                    if (!CurrentNumberLabel.Text.Contains(','))
                    {
                        OpHandler.setOperand(currentValue + ",");
                        CurrentNumberLabel.Text = OpHandler.getOperandStr();
                        UpdateInputSettings(1, DecreaseFontSize);
                    }
                    break;
                case "DeleteButton":
                    string operand = OpHandler.getOperandStr();

                    if (operand == "0" || WaitForRightOperand || OpHandler.getCurrentFunction() != null)
                        return;

                    if (operand.Length == 1)
                    {
                        OpHandler.setOperand("0");
                        CurrentNumberLabel.Text = OpHandler.getOperandStr();
                        return;
                    }
                    else if (CurrentNumberLabel.Text.Length == 2 && CurrentNumberLabel.Text.First() == '-')
                    {
                        OpHandler.setOperand("0");
                        CurrentNumberLabel.Text = OpHandler.getOperandStr();
                        UpdateInputSettings(-1, delegate () { });
                        return;
                    }

                    char LastCh = CurrentNumberLabel.Text.Last();

                    if (LastCh == ',')
                    {
                        UpdateInputSettings(-1, delegate () { });
                    }

                    OpHandler.setOperand(CurrentNumberLabel.Text.Remove(CurrentNumberLabel.Text.Length - 1));
                    CurrentNumberLabel.Text = OpHandler.getOperandStr();
                    IncreaseFontSize();
                    break;
                case "CancelEntryButton":
                    bool cf = (OpHandler.getCurrentFunction() == null),
                         co = (OpHandler.getCurrentOperation() == null);

                    CurrentNumberLabel.Text = "0";

                    if (co)
                    {
                        OpHandler.setOperand(CurrentNumberLabel.Text);
                        OpHandler.setCurrentFunction(null);
                        return;
                    }

                    if (!cf)
                    {
                        OpHandler.setCurrentFunction(null);
                        OpHandler.setOperand(CurrentNumberLabel.Text);
                    }

                    if (!co)
                    {
                        WaitForRightOperand = true;
                    }

                    CurrentExpressionLabel.Text = OpHandler.getExpressionStr(WaitForRightOperand);
                    SetInputSettings(DefaultInputLength, IncreaseFontSize);
                    break;
                case "ClearButton":
                    //SetInputSettings(DefaultInputLength, IncreaseFontSize);
                    break;
            }
        }
    }
}
