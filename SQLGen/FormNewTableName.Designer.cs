namespace SQLGen
{
    public partial class FormNewTableName
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
            this.tbOldTableName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbNewTableName = new System.Windows.Forms.TextBox();
            this.btReplace = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Текущее имя таблицы:";
            // 
            // tbOldTableName
            // 
            this.tbOldTableName.Location = new System.Drawing.Point(151, 12);
            this.tbOldTableName.Name = "tbOldTableName";
            this.tbOldTableName.ReadOnly = true;
            this.tbOldTableName.Size = new System.Drawing.Size(280, 20);
            this.tbOldTableName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Новое имя таблицы:";
            // 
            // tbNewTableName
            // 
            this.tbNewTableName.Location = new System.Drawing.Point(151, 40);
            this.tbNewTableName.Name = "tbNewTableName";
            this.tbNewTableName.Size = new System.Drawing.Size(280, 20);
            this.tbNewTableName.TabIndex = 1;
            // 
            // btReplace
            // 
            this.btReplace.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btReplace.Location = new System.Drawing.Point(101, 86);
            this.btReplace.Name = "btReplace";
            this.btReplace.Size = new System.Drawing.Size(75, 23);
            this.btReplace.TabIndex = 3;
            this.btReplace.Text = "Сменить";
            this.btReplace.UseVisualStyleBackColor = true;
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(279, 86);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 4;
            this.btCancel.Text = "Отмена";
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // FormNewTableName
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 130);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btReplace);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbNewTableName);
            this.Controls.Add(this.tbOldTableName);
            this.Controls.Add(this.label1);
            this.Name = "FormNewTableName";
            this.Text = "Смена имени таблицы";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox tbOldTableName;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox tbNewTableName;
        private System.Windows.Forms.Button btReplace;
        private System.Windows.Forms.Button btCancel;
    }
}