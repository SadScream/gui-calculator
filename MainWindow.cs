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

        private void Solve()
        {
            /*
             * Выводит на CurrentNumberLabel результат вычисления
             */

            if (OpHandler.Solve()) // вычисляем
            {
                CurrentNumberLabel.Text = OpHandler.GetResult().ToString(); // выводим

                // полностью стираем выражение и в качестве левого операнда устанавливаем результат
                OpHandler.operand.Right.SetDefault();
                OpHandler.operand.Left.SetDefault();
                OpHandler.SetExpression(null);

                CurrentExpressionLabel.Text = OpHandler.GetExpressionStr();

                OpHandler.operand.Left.SetByStr(OpHandler.GetResult().ToString());
                OpHandler.operand.SetLeftActive(true);
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
            Operand activeOperand = OpHandler.operand.Active();

            if (activeOperand.GetFunction() != null)
            {
                // если текущий операнд является функцией, то при вводе числа
                // зануляем этот операнд и записываем на его место вводимое число
                
                activeOperand.SetDefault();
                CurrentExpressionLabel.Text = OpHandler.GetExpressionStr();
            }

            if (activeOperand.WaitForInput() && activeOperand.GetText() != "0")
            {
                // на случай, когда в качестве одного из операндов выступает вычисленное значение
                // например, когда введем 2+sqrt(1), то в поле для ввода вставится результат вычисление
                // функции sqrt(1). Тогда если после этого мы нажмем на какую-либо цифру, то этот результат
                // должен стереться и вместо него мы вставим вводимое число

                activeOperand.SetByStr("0");
            }
            
            OpHandler.PutNum(number);

            CurrentNumberLabel.Text = activeOperand.GetNumber().ToString();
            DecreaseFontSize();
        }

        private void DoubleOperatorClicked(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            string operation = b.Name;

            if (OpHandler.operand.RightIsActive() && 
                !OpHandler.operand.Active().WaitForInput())
            {
                // если в данный момент уже введено какое-то выражение и вновь производится клик
                // по оператору, то решаем это выражение

                Solve();
            }

            switch (operation)
            {
                case "PlusButton":
                    OpHandler.PutOperator(OpHandler.Sum);
                    break;
                case "MinusButton":
                    OpHandler.PutOperator(OpHandler.Dif);
                    break;
                case "MuliplyButton":
                    OpHandler.PutOperator(OpHandler.Mult);
                    break;
                case "DivisionButton":
                    OpHandler.PutOperator(OpHandler.Div);
                    break;
            }

            CurrentExpressionLabel.Text = OpHandler.GetExpressionStr();
            
            if (OpHandler.operand.RightIsActive())
            {
                // устанавливаем в качестве правого оператора то же число, что и для левого оператора,
                // чтоб в случае, если пользователь сразу нажмет '=', посчитать результат для одного и того же числа
                // как в оригнинальном калькуляторе

                OpHandler.operand.Right.SetByNum(OpHandler.operand.Left.GetNumber());
            }
        }

        private void SingleOperatorClicked(object sender, EventArgs e)
        {
            // была выбрана операция, для которой требуется один операнд
            // например, возведение в квадрат, взятие квадратного корня, нат. логарифм и тд

            string operation = ((Button)sender).Name;
            string currentStrValue = CurrentNumberLabel.Text;

            switch (operation)
            {
                case "SquareButton":
                    OpHandler.PutFunction(OpHandler.function.Sqr);
                    break;
                case "RadicalButton":
                    OpHandler.PutFunction(OpHandler.function.Sqrt);
                    break;
                case "CosButton":
                    OpHandler.PutFunction(OpHandler.function.Cos);
                    break;
                case "SinButton":
                    OpHandler.PutFunction(OpHandler.function.Sin);
                    break;
                case "TgButton":
                    OpHandler.PutFunction(OpHandler.function.Tg);
                    break;
                case "CtgButton":
                    OpHandler.PutFunction(OpHandler.function.Ctg);
                    break;
                case "LnButton":
                    OpHandler.PutFunction(OpHandler.function.Ln);
                    break;
                case "LgButton":
                    OpHandler.PutFunction(OpHandler.function.Lg);
                    break;
                case "ReverseButton":
                    OpHandler.PutFunction(OpHandler.function.Rev);
                    break;
                case "PercentButton":
                    OpHandler.PutFunction(OpHandler.function.Perc);
                    break;
                case "PosOrNegButton":
                    OpHandler.PutNegative();
                    break;
            }

            CurrentNumberLabel.Text = OpHandler.operand.Active().GetNumber().ToString();
            CurrentExpressionLabel.Text = OpHandler.GetExpressionStr();
            SetInputSettings(DefaultInputLength, IncreaseFontSize);
        }

        private void EditingButtonClicked(object sender, EventArgs e)
        {
            /*
             * была нажата клавиша , | del | ce | c
             */

            string operation = ((Button)sender).Name;
            string currentValue = OpHandler.operand.Active().GetText();

            switch (operation)
            {
                case "CommaButton":
                    OpHandler.PutComma();
                    break;
                case "DeleteButton":
                    Operand activeOperand = OpHandler.operand.Active();

                    if (activeOperand.GetText() == "0" || activeOperand.GetFunction() != null)
                        return;

                    if (activeOperand.GetText().Length == 1 || 
                        CurrentNumberLabel.Text.Length == 2 && CurrentNumberLabel.Text.First() == '-')
                    {
                        activeOperand.SetByStr("0");
                        CurrentNumberLabel.Text = activeOperand.GetNumber().ToString();

                        if (CurrentNumberLabel.Text.First() == '-')
                            UpdateInputSettings(-1, delegate () { });

                        return;
                    }

                    if (CurrentNumberLabel.Text.Last() == ',')
                    {
                        UpdateInputSettings(-1, delegate () { });
                    }

                    activeOperand.SetByStr(CurrentNumberLabel.Text.Remove(CurrentNumberLabel.Text.Length - 1));
                    CurrentNumberLabel.Text = activeOperand.GetNumber().ToString();
                    IncreaseFontSize();
                    break;
                case "CancelEntryButton":
                    OpHandler.operand.Active().SetDefault();
                    CurrentNumberLabel.Text = OpHandler.operand.Active().GetNumber().ToString();
                    CurrentExpressionLabel.Text = OpHandler.GetExpressionStr();
                    SetInputSettings(DefaultInputLength, IncreaseFontSize);
                    break;
                case "ClearButton":
                    OpHandler.operand.Right.SetDefault();
                    OpHandler.operand.Left.SetDefault();
                    OpHandler.operand.SetLeftActive();
                    OpHandler.SetExpression(null);
                    CurrentNumberLabel.Text = OpHandler.operand.Active().GetNumber().ToString();
                    CurrentExpressionLabel.Text = OpHandler.GetExpressionStr();
                    break;
            }
            CurrentNumberLabel.Text = OpHandler.operand.Active().GetText();
        }

        private void HistoryButtonClicked(object sender, EventArgs e)
        {
            // создает окно с историей операций

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
