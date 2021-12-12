// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
namespace Market
{
    partial class FormPortfolio
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
            this.btnSeek = new System.Windows.Forms.Button();
            this.listPapers = new System.Windows.Forms.ListBox();
            this.lbOperation = new System.Windows.Forms.Label();
            this.boxCount = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.boxNominal = new System.Windows.Forms.TextBox();
            this.boxCurrency = new System.Windows.Forms.ComboBox();
            this.boxPrice = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.boxAccount = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.boxSumma = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.boxLot = new System.Windows.Forms.TextBox();
            this.boxPlan = new System.Windows.Forms.CheckBox();
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
            // lbOperation
            // 
            this.lbOperation.AutoSize = true;
            this.lbOperation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbOperation.Location = new System.Drawing.Point(30, 153);
            this.lbOperation.Name = "lbOperation";
            this.lbOperation.Size = new System.Drawing.Size(62, 13);
            this.lbOperation.TabIndex = 4;
            this.lbOperation.Text = "КУПИТЬ:";
            // 
            // boxCount
            // 
            this.boxCount.CausesValidation = false;
            this.boxCount.Location = new System.Drawing.Point(91, 150);
            this.boxCount.Name = "boxCount";
            this.boxCount.Size = new System.Drawing.Size(80, 20);
            this.boxCount.TabIndex = 3;
            this.boxCount.TextChanged += new System.EventHandler(this.boxCount_TextChanged);
            this.boxCount.Leave += new System.EventHandler(this.boxCount_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(177, 152);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "шт";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(342, 179);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Номинал:";
            // 
            // boxNominal
            // 
            this.boxNominal.Location = new System.Drawing.Point(404, 175);
            this.boxNominal.Name = "boxNominal";
            this.boxNominal.Size = new System.Drawing.Size(73, 20);
            this.boxNominal.TabIndex = 9;
            this.boxNominal.TextChanged += new System.EventHandler(this.boxNominal_TextChanged);
            // 
            // boxCurrency
            // 
            this.boxCurrency.FormattingEnabled = true;
            this.boxCurrency.Location = new System.Drawing.Point(206, 175);
            this.boxCurrency.Name = "boxCurrency";
            this.boxCurrency.Size = new System.Drawing.Size(121, 21);
            this.boxCurrency.TabIndex = 7;
            this.boxCurrency.SelectedIndexChanged += new System.EventHandler(this.boxCurrency_SelectedIndexChanged);
            // 
            // boxPrice
            // 
            this.boxPrice.Location = new System.Drawing.Point(91, 175);
            this.boxPrice.Name = "boxPrice";
            this.boxPrice.Size = new System.Drawing.Size(109, 20);
            this.boxPrice.TabIndex = 4;
            this.boxPrice.TextChanged += new System.EventHandler(this.boxCount_TextChanged);
            this.boxPrice.KeyDown += new System.Windows.Forms.KeyEventHandler(this.boxPrice_KeyDown);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(49, 178);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Цена:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(365, 153);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Счет:";
            // 
            // boxAccount
            // 
            this.boxAccount.Location = new System.Drawing.Point(404, 149);
            this.boxAccount.Name = "boxAccount";
            this.boxAccount.Size = new System.Drawing.Size(146, 20);
            this.boxAccount.TabIndex = 8;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(41, 204);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Сумма:";
            // 
            // boxSumma
            // 
            this.boxSumma.Location = new System.Drawing.Point(91, 201);
            this.boxSumma.Name = "boxSumma";
            this.boxSumma.ReadOnly = true;
            this.boxSumma.Size = new System.Drawing.Size(109, 20);
            this.boxSumma.TabIndex = 5;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(201, 252);
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
            this.btnCancel.Location = new System.Drawing.Point(369, 252);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(203, 153);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(24, 13);
            this.label8.TabIndex = 6;
            this.label8.Text = "лот";
            // 
            // boxLot
            // 
            this.boxLot.Location = new System.Drawing.Point(233, 149);
            this.boxLot.Name = "boxLot";
            this.boxLot.ReadOnly = true;
            this.boxLot.Size = new System.Drawing.Size(94, 20);
            this.boxLot.TabIndex = 5;
            // 
            // boxPlan
            // 
            this.boxPlan.AutoSize = true;
            this.boxPlan.Location = new System.Drawing.Point(206, 204);
            this.boxPlan.Name = "boxPlan";
            this.boxPlan.Size = new System.Drawing.Size(139, 17);
            this.boxPlan.TabIndex = 12;
            this.boxPlan.Text = "Планируемая покупка";
            this.boxPlan.UseVisualStyleBackColor = true;
            this.boxPlan.CheckedChanged += new System.EventHandler(this.boxPlan_CheckedChanged);
            // 
            // FormPortfolio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(670, 302);
            this.Controls.Add(this.boxPlan);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.boxCurrency);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.boxAccount);
            this.Controls.Add(this.boxNominal);
            this.Controls.Add(this.boxLot);
            this.Controls.Add(this.boxSumma);
            this.Controls.Add(this.boxPrice);
            this.Controls.Add(this.boxCount);
            this.Controls.Add(this.lbOperation);
            this.Controls.Add(this.listPapers);
            this.Controls.Add(this.btnSeek);
            this.Controls.Add(this.boxSeek);
            this.Controls.Add(this.label1);
            this.Name = "FormPortfolio";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "КУПИТЬ ценную бумагу";
            this.Shown += new System.EventHandler(this.FormPortfolio_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox boxSeek;
        private System.Windows.Forms.Button btnSeek;
        private System.Windows.Forms.ListBox listPapers;
        private System.Windows.Forms.Label lbOperation;
        private System.Windows.Forms.TextBox boxCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox boxNominal;
        private System.Windows.Forms.ComboBox boxCurrency;
        private System.Windows.Forms.TextBox boxPrice;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox boxAccount;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox boxSumma;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox boxLot;
        private System.Windows.Forms.CheckBox boxPlan;
    }
}