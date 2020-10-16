namespace SQLGen
{
    partial class FormAddScript
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
            this.tbScriptFilename = new System.Windows.Forms.TextBox();
            this.btOpen = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cbGITProject = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbGITTypeObject = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbGITShemaObject = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbGITNameObject = new System.Windows.Forms.ComboBox();
            this.btAdd = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.tbGITFilename = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tbGITFolder = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Файл:";
            // 
            // tbScriptFilename
            // 
            this.tbScriptFilename.Location = new System.Drawing.Point(57, 35);
            this.tbScriptFilename.Name = "tbScriptFilename";
            this.tbScriptFilename.Size = new System.Drawing.Size(594, 20);
            this.tbScriptFilename.TabIndex = 4;
            this.tbScriptFilename.Leave += new System.EventHandler(this.tbScriptFilename_Leave);
            // 
            // btOpen
            // 
            this.btOpen.Location = new System.Drawing.Point(657, 33);
            this.btOpen.Name = "btOpen";
            this.btOpen.Size = new System.Drawing.Size(106, 21);
            this.btOpen.TabIndex = 5;
            this.btOpen.Text = "Найти файл";
            this.btOpen.UseVisualStyleBackColor = true;
            this.btOpen.Click += new System.EventHandler(this.btOpen_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Проект GIT:";
            // 
            // cbGITProject
            // 
            this.cbGITProject.FormattingEnabled = true;
            this.cbGITProject.Location = new System.Drawing.Point(86, 64);
            this.cbGITProject.Name = "cbGITProject";
            this.cbGITProject.Size = new System.Drawing.Size(227, 21);
            this.cbGITProject.TabIndex = 7;
            this.cbGITProject.SelectedIndexChanged += new System.EventHandler(this.cbGITProject_Leave);
            this.cbGITProject.Leave += new System.EventHandler(this.cbGITProject_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Тип объекта:";
            // 
            // cbGITTypeObject
            // 
            this.cbGITTypeObject.FormattingEnabled = true;
            this.cbGITTypeObject.Location = new System.Drawing.Point(86, 96);
            this.cbGITTypeObject.Name = "cbGITTypeObject";
            this.cbGITTypeObject.Size = new System.Drawing.Size(227, 21);
            this.cbGITTypeObject.TabIndex = 7;
            this.cbGITTypeObject.SelectedIndexChanged += new System.EventHandler(this.cbGITTypeObject_Leave);
            this.cbGITTypeObject.Leave += new System.EventHandler(this.cbGITTypeObject_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 132);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Схема:";
            // 
            // cbGITShemaObject
            // 
            this.cbGITShemaObject.FormattingEnabled = true;
            this.cbGITShemaObject.Location = new System.Drawing.Point(86, 128);
            this.cbGITShemaObject.Name = "cbGITShemaObject";
            this.cbGITShemaObject.Size = new System.Drawing.Size(227, 21);
            this.cbGITShemaObject.TabIndex = 7;
            this.cbGITShemaObject.SelectedIndexChanged += new System.EventHandler(this.cbGITShemaObject_Leave);
            this.cbGITShemaObject.Leave += new System.EventHandler(this.cbGITShemaObject_Leave);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 193);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(198, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Имя файла для GIT, без расширения:";
            // 
            // cbGITNameObject
            // 
            this.cbGITNameObject.FormattingEnabled = true;
            this.cbGITNameObject.Location = new System.Drawing.Point(308, 165);
            this.cbGITNameObject.Name = "cbGITNameObject";
            this.cbGITNameObject.Size = new System.Drawing.Size(343, 21);
            this.cbGITNameObject.TabIndex = 7;
            this.cbGITNameObject.SelectedIndexChanged += new System.EventHandler(this.cbGITNameObject_Leave);
            this.cbGITNameObject.Leave += new System.EventHandler(this.cbGITNameObject_Leave);
            // 
            // btAdd
            // 
            this.btAdd.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btAdd.Location = new System.Drawing.Point(266, 248);
            this.btAdd.Name = "btAdd";
            this.btAdd.Size = new System.Drawing.Size(75, 23);
            this.btAdd.TabIndex = 11;
            this.btAdd.Text = "Добавить";
            this.btAdd.UseVisualStyleBackColor = true;
            this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(404, 248);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 12;
            this.btCancel.Text = "Отмена";
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // tbGITFilename
            // 
            this.tbGITFilename.Location = new System.Drawing.Point(12, 209);
            this.tbGITFilename.Name = "tbGITFilename";
            this.tbGITFilename.Size = new System.Drawing.Size(639, 20);
            this.tbGITFilename.TabIndex = 13;
            this.tbGITFilename.Leave += new System.EventHandler(this.tbGITFilename_Leave);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 168);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(290, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Имя объекта (таблицы, представления, функции и т.п.):";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 13);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Каталог GIT:";
            // 
            // tbGITFolder
            // 
            this.tbGITFolder.Location = new System.Drawing.Point(99, 9);
            this.tbGITFolder.Name = "tbGITFolder";
            this.tbGITFolder.ReadOnly = true;
            this.tbGITFolder.Size = new System.Drawing.Size(552, 20);
            this.tbGITFolder.TabIndex = 15;
            // 
            // FormAddScript
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(789, 287);
            this.Controls.Add(this.tbGITFolder);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tbScriptFilename);
            this.Controls.Add(this.btOpen);
            this.Controls.Add(this.cbGITProject);
            this.Controls.Add(this.cbGITTypeObject);
            this.Controls.Add(this.cbGITShemaObject);
            this.Controls.Add(this.cbGITNameObject);
            this.Controls.Add(this.tbGITFilename);
            this.Controls.Add(this.btAdd);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "FormAddScript";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Новый скрипт для отправки в GIT";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox tbScriptFilename;
        private System.Windows.Forms.Button btOpen;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.ComboBox cbGITProject;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.ComboBox cbGITTypeObject;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.ComboBox cbGITShemaObject;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.ComboBox cbGITNameObject;
        private System.Windows.Forms.Button btAdd;
        private System.Windows.Forms.Button btCancel;
        public System.Windows.Forms.TextBox tbGITFilename;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.TextBox tbGITFolder;
    }
}