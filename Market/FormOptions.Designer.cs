namespace Market
{
    partial class FormOptions
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
            this.listBrokers = new System.Windows.Forms.ListBox();
            this.boxNewBroker = new System.Windows.Forms.TextBox();
            this.btDelBroker = new System.Windows.Forms.Button();
            this.btAddBroker = new System.Windows.Forms.Button();
            this.btSave = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.boxNominalName = new System.Windows.Forms.TextBox();
            this.boxPriceName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.boxLastDateName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.boxTickerName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.listTypes = new System.Windows.Forms.ListBox();
            this.boxNewType = new System.Windows.Forms.TextBox();
            this.btAddType = new System.Windows.Forms.Button();
            this.btDelType = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.boxInvestFileName = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.boxDividend = new System.Windows.Forms.TextBox();
            this.boxProfit = new System.Windows.Forms.TextBox();
            this.boxTinkoff = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.boxMarketList = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.boxHistory = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBrokers
            // 
            this.listBrokers.FormattingEnabled = true;
            this.listBrokers.Location = new System.Drawing.Point(14, 60);
            this.listBrokers.Name = "listBrokers";
            this.listBrokers.Size = new System.Drawing.Size(349, 82);
            this.listBrokers.Sorted = true;
            this.listBrokers.TabIndex = 10;
            // 
            // boxNewBroker
            // 
            this.boxNewBroker.Location = new System.Drawing.Point(14, 34);
            this.boxNewBroker.Name = "boxNewBroker";
            this.boxNewBroker.Size = new System.Drawing.Size(267, 20);
            this.boxNewBroker.TabIndex = 7;
            // 
            // btDelBroker
            // 
            this.btDelBroker.Image = global::Market.Properties.Resources.Корзина;
            this.btDelBroker.Location = new System.Drawing.Point(328, 19);
            this.btDelBroker.Name = "btDelBroker";
            this.btDelBroker.Size = new System.Drawing.Size(35, 35);
            this.btDelBroker.TabIndex = 9;
            this.btDelBroker.UseVisualStyleBackColor = true;
            this.btDelBroker.Click += new System.EventHandler(this.btDelBroker_Click);
            // 
            // btAddBroker
            // 
            this.btAddBroker.Image = global::Market.Properties.Resources.Плюс;
            this.btAddBroker.Location = new System.Drawing.Point(287, 19);
            this.btAddBroker.Name = "btAddBroker";
            this.btAddBroker.Size = new System.Drawing.Size(35, 35);
            this.btAddBroker.TabIndex = 8;
            this.btAddBroker.UseVisualStyleBackColor = true;
            this.btAddBroker.Click += new System.EventHandler(this.btAddBroker_Click);
            // 
            // btSave
            // 
            this.btSave.Location = new System.Drawing.Point(115, 349);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(75, 23);
            this.btSave.TabIndex = 21;
            this.btSave.Text = "Сохранить";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(234, 349);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 22;
            this.btCancel.Text = "Отмена";
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.boxNominalName);
            this.groupBox1.Controls.Add(this.boxPriceName);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.boxLastDateName);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.boxTickerName);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(417, 18);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(372, 134);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Названия столбцов:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(78, 105);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(56, 13);
            this.label12.TabIndex = 11;
            this.label12.Text = "Номинал:";
            // 
            // boxNominalName
            // 
            this.boxNominalName.Location = new System.Drawing.Point(140, 102);
            this.boxNominalName.Name = "boxNominalName";
            this.boxNominalName.Size = new System.Drawing.Size(218, 20);
            this.boxNominalName.TabIndex = 15;
            this.boxNominalName.Text = "Номинал";
            // 
            // boxPriceName
            // 
            this.boxPriceName.Location = new System.Drawing.Point(140, 76);
            this.boxPriceName.Name = "boxPriceName";
            this.boxPriceName.Size = new System.Drawing.Size(218, 20);
            this.boxPriceName.TabIndex = 14;
            this.boxPriceName.Text = "Котировка в валюте";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 79);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(113, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Котировка в валюте:";
            // 
            // boxLastDateName
            // 
            this.boxLastDateName.Location = new System.Drawing.Point(140, 50);
            this.boxLastDateName.Name = "boxLastDateName";
            this.boxLastDateName.Size = new System.Drawing.Size(218, 20);
            this.boxLastDateName.TabIndex = 13;
            this.boxLastDateName.Text = "Дата котировки";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(42, 53);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Дата котировки:";
            // 
            // boxTickerName
            // 
            this.boxTickerName.Location = new System.Drawing.Point(140, 25);
            this.boxTickerName.Name = "boxTickerName";
            this.boxTickerName.Size = new System.Drawing.Size(218, 20);
            this.boxTickerName.TabIndex = 12;
            this.boxTickerName.Text = "Тикер";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(93, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Тикер:";
            // 
            // listTypes
            // 
            this.listTypes.FormattingEnabled = true;
            this.listTypes.Location = new System.Drawing.Point(9, 56);
            this.listTypes.Name = "listTypes";
            this.listTypes.Size = new System.Drawing.Size(349, 134);
            this.listTypes.Sorted = true;
            this.listTypes.TabIndex = 20;
            // 
            // boxNewType
            // 
            this.boxNewType.Location = new System.Drawing.Point(9, 30);
            this.boxNewType.Name = "boxNewType";
            this.boxNewType.Size = new System.Drawing.Size(267, 20);
            this.boxNewType.TabIndex = 17;
            // 
            // btAddType
            // 
            this.btAddType.Image = global::Market.Properties.Resources.Плюс;
            this.btAddType.Location = new System.Drawing.Point(282, 15);
            this.btAddType.Name = "btAddType";
            this.btAddType.Size = new System.Drawing.Size(35, 35);
            this.btAddType.TabIndex = 18;
            this.btAddType.UseVisualStyleBackColor = true;
            this.btAddType.Click += new System.EventHandler(this.btAddType_Click);
            // 
            // btDelType
            // 
            this.btDelType.Image = global::Market.Properties.Resources.Корзина;
            this.btDelType.Location = new System.Drawing.Point(323, 15);
            this.btDelType.Name = "btDelType";
            this.btDelType.Size = new System.Drawing.Size(35, 35);
            this.btDelType.TabIndex = 19;
            this.btDelType.UseVisualStyleBackColor = true;
            this.btDelType.Click += new System.EventHandler(this.btDelType_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(27, 21);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(153, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Имя файла содержит фразу:";
            // 
            // boxInvestFileName
            // 
            this.boxInvestFileName.Location = new System.Drawing.Point(186, 18);
            this.boxInvestFileName.Name = "boxInvestFileName";
            this.boxInvestFileName.Size = new System.Drawing.Size(205, 20);
            this.boxInvestFileName.TabIndex = 1;
            this.boxInvestFileName.Text = "инвестиции";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.boxHistory);
            this.groupBox2.Controls.Add(this.boxDividend);
            this.groupBox2.Controls.Add(this.boxProfit);
            this.groupBox2.Controls.Add(this.boxTinkoff);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.boxMarketList);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(30, 44);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(372, 138);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Листы";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(165, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Лист для дивидендов/купонов:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(14, 66);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(160, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "Лист для расчета доходности:";
            // 
            // boxDividend
            // 
            this.boxDividend.Location = new System.Drawing.Point(176, 86);
            this.boxDividend.Name = "boxDividend";
            this.boxDividend.Size = new System.Drawing.Size(185, 20);
            this.boxDividend.TabIndex = 6;
            this.boxDividend.Text = "Dividend";
            // 
            // boxProfit
            // 
            this.boxProfit.Location = new System.Drawing.Point(176, 63);
            this.boxProfit.Name = "boxProfit";
            this.boxProfit.Size = new System.Drawing.Size(185, 20);
            this.boxProfit.TabIndex = 5;
            this.boxProfit.Text = "Profit";
            // 
            // boxTinkoff
            // 
            this.boxTinkoff.Location = new System.Drawing.Point(176, 41);
            this.boxTinkoff.Name = "boxTinkoff";
            this.boxTinkoff.Size = new System.Drawing.Size(185, 20);
            this.boxTinkoff.TabIndex = 4;
            this.boxTinkoff.Text = "Tinkoff";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(164, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Лист для портфеля Тинькофф:";
            // 
            // boxMarketList
            // 
            this.boxMarketList.Location = new System.Drawing.Point(176, 19);
            this.boxMarketList.Name = "boxMarketList";
            this.boxMarketList.Size = new System.Drawing.Size(185, 20);
            this.boxMarketList.TabIndex = 3;
            this.boxMarketList.Text = "List";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(168, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Лист для списка ценных бумаг:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.boxNewType);
            this.groupBox3.Controls.Add(this.listTypes);
            this.groupBox3.Controls.Add(this.btAddType);
            this.groupBox3.Controls.Add(this.btDelType);
            this.groupBox3.Location = new System.Drawing.Point(417, 173);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(372, 205);
            this.groupBox3.TabIndex = 16;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Уточнение типов ценных бумаг (Key=Value):";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btAddBroker);
            this.groupBox4.Controls.Add(this.listBrokers);
            this.groupBox4.Controls.Add(this.boxNewBroker);
            this.groupBox4.Controls.Add(this.btDelBroker);
            this.groupBox4.Location = new System.Drawing.Point(28, 188);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(374, 155);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Листы других брокеров:";
            // 
            // boxHistory
            // 
            this.boxHistory.Location = new System.Drawing.Point(176, 109);
            this.boxHistory.Name = "boxHistory";
            this.boxHistory.Size = new System.Drawing.Size(185, 20);
            this.boxHistory.TabIndex = 6;
            this.boxHistory.Text = "History";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(74, 109);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(100, 13);
            this.label9.TabIndex = 11;
            this.label9.Text = "Лист для истории:";
            // 
            // FormOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(797, 386);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.boxInvestFileName);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormOptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Настройки Excel";
            this.Shown += new System.EventHandler(this.FormOptions_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListBox listBrokers;
        private System.Windows.Forms.TextBox boxNewBroker;
        private System.Windows.Forms.Button btAddBroker;
        private System.Windows.Forms.Button btDelBroker;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox boxPriceName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox boxLastDateName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox boxTickerName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox listTypes;
        private System.Windows.Forms.TextBox boxNewType;
        private System.Windows.Forms.Button btAddType;
        private System.Windows.Forms.Button btDelType;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox boxNominalName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox boxInvestFileName;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox boxProfit;
        private System.Windows.Forms.TextBox boxTinkoff;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox boxMarketList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox boxDividend;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox boxHistory;
    }
}