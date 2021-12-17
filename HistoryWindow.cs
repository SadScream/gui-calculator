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
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text File|*.txt";
            saveFileDialog.Title = "Сохранить историю";
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog.OpenFile();

                foreach (string elem in ExpressionBox.Items)
                {
                    byte[] info = Encoding.UTF8.GetBytes(elem + "\n");
                    fs.Write(info);
                }

                fs.Close();
            }
        }

        private void downloadButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text File|*.txt";
            openFileDialog.Title = "Открыть файл";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;

                var fileStream = openFileDialog.OpenFile();

                using (System.IO.StreamReader reader = new System.IO.StreamReader(fileStream))
                {
                    for (int i = 1; reader.Peek() >= 0; i++)
                    {
                        MainW.Clear();
                        string str = reader.ReadLine();
                        int result = ExpParser.Parse(str);

                        Console.WriteLine("parser int result: {0}", result);

                        if (result == -1)
                        {
                            OpHandler.EventListener = updateList;
                            MessageBox.Show(String.Format("Ошибка в строке {0} позиции {1}", 
                                i, ExpParser.GetPosition()+1),
                                "Ошибка", MessageBoxButtons.OK);
                            return;
                        }

                        if (OpHandler.GetOperator() != null)
                        {
                            MainW.Solve();
                        }
                        else
                        {
                            MainW.SetNumber(OpHandler.OperandManager.Active().GetNumber().ToString());
                        }
                    }
                }
            }
        }

        private void ExpressionBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ExpressionBox.SelectedItem == null) return;

            MainW.Clear();

            OpHandler.EventListener = () => { }; // чтобы не захламлять окно истории, удаляем функцию обновления
            int result = ExpParser.Parse(ExpressionBox.SelectedItem.ToString());

            Console.WriteLine("parser int result: {0}", result);

            if (result == -1)
            {
                OpHandler.EventListener = updateList;
                MessageBox.Show(String.Format("Ошибка в позиции {0}", ExpParser.GetPosition()+1),
                    "Ошибка", MessageBoxButtons.OK);
                return;
            }

            if (OpHandler.GetOperator() != null)
            {
                MainW.Solve();
            } else
            {
                MainW.SetNumber(OpHandler.OperandManager.Active().GetNumber().ToString());
            }

            OpHandler.EventListener = updateList;
        }
    }
}
