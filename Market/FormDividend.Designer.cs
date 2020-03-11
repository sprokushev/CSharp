namespace Market
{
    partial class FormDividend
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
            this.label1 = new System.Windows.Forms.Label();
            this.boxSeek = new System.Windows.Forms.TextBox();
            this.listPapers = new System.Windows.Forms.ListBox();
            this.boxCurrency = new System.Windows.Forms.ComboBox();
            this.boxSumma = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.boxAccount = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSeek = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.boxOperation = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Поиск:";
            // 
            // boxSeek
            // 
            this.boxSeek.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.boxSeek.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.boxSeek.Location = new System.Drawing.Point(67, 19);
            this.boxSeek.Name = "boxSeek";
            this.boxSeek.Size = new System.Drawing.Size(535, 20);
            this.boxSeek.TabIndex = 1;
            this.boxSeek.KeyDown += new System.Windows.Forms.KeyEventHandler(this.boxSeek_KeyDown);
            this.boxSeek.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.boxSeek_KeyPress);
            // 
            // listPapers
            // 
            this.listPapers.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listPapers.FormattingEnabled = true;
            this.listPapers.ItemHeight = 14;
            this.listPapers.Location = new System.Drawing.Point(23, 48);
            this.listPapers.Name = "listPapers";
            this.listPapers.Size = new System.Drawing.Size(611, 88);
            this.listPapers.Sorted = true;
            this.listPapers.TabIndex = 2;
            this.listPapers.SelectedValueChanged += new System.EventHandler(this.listPapers_SelectedValueChanged);
            this.listPapers.Leave += new System.EventHandler(this.listPapers_SelectedValueChanged);
            // 
            // boxCurrency
            // 
            this.boxCurrency.FormattingEnabled = true;
            this.boxCurrency.Location = new System.Drawing.Point(206, 177);
            this.boxCurrency.Name = "boxCurrency";
            this.boxCurrency.Size = new System.Drawing.Size(121, 21);
            this.boxCurrency.TabIndex = 7;
            this.boxCurrency.SelectedIndexChanged += new System.EventHandler(this.boxCurrency_SelectedIndexChanged);
            // 
            // boxSumma
            // 
            this.boxSumma.Location = new System.Drawing.Point(91, 177);
            this.boxSumma.Name = "boxSumma";
            this.boxSumma.Size = new System.Drawing.Size(109, 20);
            this.boxSumma.TabIndex = 4;
            this.boxSumma.TextChanged += new System.EventHandler(this.boxSumma_TextChanged);
            this.boxSumma.KeyDown += new System.Windows.Forms.KeyEventHandler(this.boxPrice_KeyDown);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(364, 181);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Счет:";
            // 
            // boxAccount
            // 
            this.boxAccount.Location = new System.Drawing.Point(403, 177);
            this.boxAccount.Name = "boxAccount";
            this.boxAccount.Size = new System.Drawing.Size(146, 20);
            this.boxAccount.TabIndex = 8;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(41, 180);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Сумма:";
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(199, 215);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 5;
            this.btnOk.Text = "Сохранить";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(367, 215);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSeek
            // 
            this.btnSeek.Image = global::Market.Properties.Resources.Seek;
            this.btnSeek.Location = new System.Drawing.Point(608, 15);
            this.btnSeek.Name = "btnSeek";
            this.btnSeek.Size = new System.Drawing.Size(26, 27);
            this.btnSeek.TabIndex = 10;
            this.btnSeek.UseVisualStyleBackColor = true;
            this.btnSeek.Click += new System.EventHandler(this.btnSeek_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 150);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Операция:";
            // 
            // boxOperation
            // 
            this.boxOperation.FormattingEnabled = true;
            this.boxOperation.Items.AddRange(new object[] {
            "Купон",
            "Дивиденд"});
            this.boxOperation.Location = new System.Drawing.Point(91, 147);
            this.boxOperation.Name = "boxOperation";
            this.boxOperation.Size = new System.Drawing.Size(236, 21);
            this.boxOperation.TabIndex = 13;
            this.boxOperation.SelectedIndexChanged += new System.EventHandler(this.boxOperation_SelectedIndexChanged);
            // 
            // FormDividend
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(656, 256);
            this.Controls.Add(this.boxOperation);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.boxCurrency);
            this.Controls.Add(this.boxAccount);
            this.Controls.Add(this.boxSumma);
            this.Controls.Add(this.listPapers);
            this.Controls.Add(this.btnSeek);
            this.Controls.Add(this.boxSeek);
            this.Controls.Add(this.label1);
            this.Name = "FormDividend";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ДИВИДЕНД / КУПОН";
            this.Shown += new System.EventHandler(this.FormDividend_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox boxSeek;
        private System.Windows.Forms.Button btnSeek;
        private System.Windows.Forms.ListBox listPapers;
        private System.Windows.Forms.ComboBox boxCurrency;
        private System.Windows.Forms.TextBox boxSumma;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox boxAccount;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox boxOperation;
    }
}