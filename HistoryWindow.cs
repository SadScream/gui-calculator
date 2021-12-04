using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab4
{
    partial class HistoryWindow : Form
    {
        public MainWindow MainW;
        public Resolver OpHandler;
        public Parser ExpParser;

        public HistoryWindow(MainWindow main, Resolver OpHandler)
        {
            InitializeComponent();

            this.OpHandler = OpHandler;

            for (int i = 0; i < OpHandler.GetHistory().Count(); i++)
            {
                ExpressionBox.Items.Add(OpHandler.GetHistory()[i]);
            }

            MainW = main;
            OpHandler.EventListener = updateList;
            ExpParser = new Parser(OpHandler);
        }

        private void updateList()
        {
            ExpressionBox.Items.Add(OpHandler.GetHistory().Last());
        }

        private void uploadButton_Click(object sender, EventArgs e)
        {

        }

        private void downloadButton_Click(object sender, EventArgs e)
        {

        }

        private void ExpressionBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainW.Clear();
            ListBox ExpressionBox = (ListBox)sender;
            ExpParser.Parse(ExpressionBox.SelectedItem.ToString());
            MainW.SetCurrentExpression(OpHandler.GetExpressionStr());

            if (OpHandler.Solve())
                MainW.SetCurrentNumber(OpHandler.GetResult().ToString());
            else
                MainW.SetCurrentNumber(OpHandler.operand.Active().GetNumber().ToString());
            //listbox.SelectedItem;
        }
    }
}
