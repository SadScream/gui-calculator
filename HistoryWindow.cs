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
        public Resolver OpHandler;

        public HistoryWindow(Resolver OpHandler)
        {
            InitializeComponent();

            this.OpHandler = OpHandler;

            for (int i = 0; i < OpHandler.history.Count(); i++)
            {
                ExpressionBox.Items.Add(OpHandler.history[i]);
            }

            OpHandler.EventListener = updateList;
        }

        private void updateList()
        {
            ExpressionBox.Items.Add(OpHandler.history.Last());
        }

        private void uploadButton_Click(object sender, EventArgs e)
        {

        }

        private void downloadButton_Click(object sender, EventArgs e)
        {

        }
    }
}
