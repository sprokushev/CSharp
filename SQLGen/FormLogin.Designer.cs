namespace SQLGen
{
    partial class FormLogin
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
            this.cbConnectionHistory = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbConnectionName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbTypeDB = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbServerName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbDatabaseName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cbAuthentication = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbUsername = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.btConnect = new System.Windows.Forms.Button();
            this.btDel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(41, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "История соединений:";
            // 
            // cbConnectionHistory
            // 
            this.cbConnectionHistory.FormattingEnabled = true;
            this.cbConnectionHistory.Location = new System.Drawing.Point(161, 18);
            this.cbConnectionHistory.Name = "cbConnectionHistory";
            this.cbConnectionHistory.Size = new System.Drawing.Size(515, 21);
            this.cbConnectionHistory.TabIndex = 1;
            this.cbConnectionHistory.SelectedIndexChanged += new System.EventHandler(this.cbConnectionHistory_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(41, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(123, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Название соединения:";
            // 
            // tbConnectionName
            // 
            this.tbConnectionName.Location = new System.Drawing.Point(161, 50);
            this.tbConnectionName.Name = "tbConnectionName";
            this.tbConnectionName.Size = new System.Drawing.Size(515, 20);
            this.tbConnectionName.TabIndex = 3;
            this.tbConnectionName.TextChanged += new System.EventHandler(this.tbConnectionName_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(41, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Тип БД:";
            // 
            // cbTypeDB
            // 
            this.cbTypeDB.FormattingEnabled = true;
            this.cbTypeDB.Items.AddRange(new object[] {
            "Microsoft SQL",
            "Postgre SQL"});
            this.cbTypeDB.Location = new System.Drawing.Point(161, 82);
            this.cbTypeDB.Name = "cbTypeDB";
            this.cbTypeDB.Size = new System.Drawing.Size(294, 21);
            this.cbTypeDB.TabIndex = 5;
            this.cbTypeDB.SelectedIndexChanged += new System.EventHandler(this.cbTypeDB_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(41, 120);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Сервер:";
            // 
            // tbServerName
            // 
            this.tbServerName.Location = new System.Drawing.Point(161, 116);
            this.tbServerName.Name = "tbServerName";
            this.tbServerName.Size = new System.Drawing.Size(294, 20);
            this.tbServerName.TabIndex = 7;
            this.tbServerName.TextChanged += new System.EventHandler(this.tbServerName_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(41, 153);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "База данных:";
            // 
            // tbDatabaseName
            // 
            this.tbDatabaseName.Location = new System.Drawing.Point(161, 149);
            this.tbDatabaseName.Name = "tbDatabaseName";
            this.tbDatabaseName.Size = new System.Drawing.Size(294, 20);
            this.tbDatabaseName.TabIndex = 9;
            this.tbDatabaseName.TextChanged += new System.EventHandler(this.tbDatabaseName_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(41, 186);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(97, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Тип авторизации:";
            // 
            // cbAuthentication
            // 
            this.cbAuthentication.FormattingEnabled = true;
            this.cbAuthentication.Items.AddRange(new object[] {
            "Windows",
            "Database"});
            this.cbAuthentication.Location = new System.Drawing.Point(161, 182);
            this.cbAuthentication.Name = "cbAuthentication";
            this.cbAuthentication.Size = new System.Drawing.Size(294, 21);
            this.cbAuthentication.TabIndex = 11;
            this.cbAuthentication.SelectedIndexChanged += new System.EventHandler(this.cbAuthentication_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(41, 219);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(83, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Пользователь:";
            // 
            // tbUsername
            // 
            this.tbUsername.Location = new System.Drawing.Point(161, 217);
            this.tbUsername.Name = "tbUsername";
            this.tbUsername.Size = new System.Drawing.Size(294, 20);
            this.tbUsername.TabIndex = 13;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(41, 252);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(48, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Пароль:";
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(161, 248);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '*';
            this.tbPassword.Size = new System.Drawing.Size(294, 20);
            this.tbPassword.TabIndex = 15;
            // 
            // btConnect
            // 
            this.btConnect.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btConnect.Location = new System.Drawing.Point(250, 299);
            this.btConnect.Name = "btConnect";
            this.btConnect.Size = new System.Drawing.Size(247, 23);
            this.btConnect.TabIndex = 16;
            this.btConnect.Text = "Подключиться";
            this.btConnect.UseVisualStyleBackColor = true;
            this.btConnect.Click += new System.EventHandler(this.btConnect_Click);
            // 
            // btDel
            // 
            this.btDel.Location = new System.Drawing.Point(688, 16);
            this.btDel.Name = "btDel";
            this.btDel.Size = new System.Drawing.Size(41, 23);
            this.btDel.TabIndex = 17;
            this.btDel.Text = "DEL";
            this.btDel.UseVisualStyleBackColor = true;
            this.btDel.Click += new System.EventHandler(this.btDel_Click);
            // 
            // FormLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(741, 344);
            this.Controls.Add(this.btDel);
            this.Controls.Add(this.btConnect);
            this.Controls.Add(this.tbPassword);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.tbUsername);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.cbAuthentication);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tbDatabaseName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbServerName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbTypeDB);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbConnectionName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbConnectionHistory);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Подключение к БД";
            this.Shown += new System.EventHandler(this.FormLogin_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox cbConnectionHistory;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox tbConnectionName;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.ComboBox cbTypeDB;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox tbServerName;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.TextBox tbDatabaseName;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.ComboBox cbAuthentication;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.TextBox tbUsername;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Button btConnect;
        private System.Windows.Forms.Button btDel;
    }
}