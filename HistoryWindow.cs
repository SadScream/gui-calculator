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
        public ExpressionHandler resolver;
        public UParser ExpParser;

        public HistoryWindow(MainWindow main, ExpressionHandler resolver)
        {
            InitializeComponent();

            this.resolver = resolver;

            for (int i = 0; i < resolver.GetHistory().Count(); i++)
            {
                ExpressionBox.Items.Add(resolver.GetHistory()[i]);
            }

            MainW = main;
            resolver.EventListener = updateList;
            ExpParser = new UParser(resolver);
        }

        private void updateList()
        {
            ExpressionBox.Items.Add(resolver.GetHistory().Last());
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
                        KeyValuePair<Decimal, string> result = ExpParser.Evaluate(str);

                        Console.WriteLine("parser int result: {0}", result.Key);

                        if (result.Value.Length != 0)
                        {
                            resolver.EventListener = updateList;
                            MessageBox.Show(String.Format("Ошибка: {0}", result.Value),
                                "Ошибка", MessageBoxButtons.OK);
                            return;
                        }

                        resolver.SetOperandToDefault();
                        MainW.SetNumber(result.Key.ToString());
                    }
                }
            }
        }

        private void ExpressionBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ExpressionBox.SelectedItem == null) return;

            MainW.Clear();

            resolver.EventListener = () => { }; // чтобы не захламлять окно истории, удаляем функцию обновления
            KeyValuePair<Decimal, string> result = ExpParser.Evaluate(ExpressionBox.SelectedItem.ToString());

            Console.WriteLine("parser int result: {0}", result.Key);

            if (result.Value.Length != 0)
            {
                resolver.EventListener = updateList;
                MessageBox.Show(String.Format("Ошибка: {0}", result.Value),
                    "Ошибка", MessageBoxButtons.OK);
                return;
            }

            resolver.SetOperandToDefault();
            MainW.SetNumber(result.Key.ToString());

            resolver.EventListener = updateList;
        }
    }
}
