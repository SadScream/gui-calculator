
namespace Lab4
{
    partial class HistoryWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            OpHandler.EventListener = () => { };
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ExpressionBox = new System.Windows.Forms.ListBox();
            this.uploadButton = new System.Windows.Forms.Button();
            this.downloadButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ExpressionBox
            // 
            this.ExpressionBox.FormattingEnabled = true;
            this.ExpressionBox.HorizontalScrollbar = true;
            this.ExpressionBox.ItemHeight = 15;
            this.ExpressionBox.Location = new System.Drawing.Point(12, 12);
            this.ExpressionBox.Name = "ExpressionBox";
            this.ExpressionBox.Size = new System.Drawing.Size(260, 259);
            this.ExpressionBox.TabIndex = 0;
            this.ExpressionBox.SelectedIndexChanged += new System.EventHandler(this.ExpressionBox_SelectedIndexChanged);
            // 
            // uploadButton
            // 
            this.uploadButton.Location = new System.Drawing.Point(12, 284);
            this.uploadButton.Name = "uploadButton";
            this.uploadButton.Size = new System.Drawing.Size(110, 23);
            this.uploadButton.TabIndex = 1;
            this.uploadButton.Text = "Выгрузить";
            this.uploadButton.UseVisualStyleBackColor = true;
            this.uploadButton.Click += new System.EventHandler(this.uploadButton_Click);
            // 
            // downloadButton
            // 
            this.downloadButton.Location = new System.Drawing.Point(162, 284);
            this.downloadButton.Name = "downloadButton";
            this.downloadButton.Size = new System.Drawing.Size(110, 23);
            this.downloadButton.TabIndex = 2;
            this.downloadButton.Text = "Загрузить";
            this.downloadButton.UseVisualStyleBackColor = true;
            this.downloadButton.Click += new System.EventHandler(this.downloadButton_Click);
            // 
            // HistoryWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 319);
            this.Controls.Add(this.downloadButton);
            this.Controls.Add(this.uploadButton);
            this.Controls.Add(this.ExpressionBox);
            this.Name = "HistoryWindow";
            this.Text = "История";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox ExpressionBox;
        private System.Windows.Forms.Button uploadButton;
        private System.Windows.Forms.Button downloadButton;
    }
}