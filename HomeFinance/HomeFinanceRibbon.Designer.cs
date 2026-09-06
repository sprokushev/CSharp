namespace HomeFinance
{
    partial class HomeFinanceRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public HomeFinanceRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabHomeFinance = this.Factory.CreateRibbonTab();
            this.grpHomeFinance = this.Factory.CreateRibbonGroup();
            this.btnFillRashod = this.Factory.CreateRibbonButton();
            this.btnLoadBankExcel = this.Factory.CreateRibbonButton();
            this.tabHomeFinance.SuspendLayout();
            this.grpHomeFinance.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabHomeFinance
            // 
            this.tabHomeFinance.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tabHomeFinance.Groups.Add(this.grpHomeFinance);
            this.tabHomeFinance.Label = "TabAddIns";
            this.tabHomeFinance.Name = "tabHomeFinance";
            // 
            // grpHomeFinance
            // 
            this.grpHomeFinance.Items.Add(this.btnFillRashod);
            this.grpHomeFinance.Items.Add(this.btnLoadBankExcel);
            this.grpHomeFinance.Label = "Семейный бюджет";
            this.grpHomeFinance.Name = "grpHomeFinance";
            // 
            // btnFillRashod
            // 
            this.btnFillRashod.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnFillRashod.Image = global::HomeFinance.Properties.Resources.sheet;
            this.btnFillRashod.Label = "Заполнить вид и статью расхода";
            this.btnFillRashod.Name = "btnFillRashod";
            this.btnFillRashod.ShowImage = true;
            this.btnFillRashod.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnFillRashod_Click);
            // 
            // btnLoadBankExcel
            // 
            this.btnLoadBankExcel.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnLoadBankExcel.Image = global::HomeFinance.Properties.Resources.Excel;
            this.btnLoadBankExcel.Label = "Загрузить Excel из онлайн-банка";
            this.btnLoadBankExcel.Name = "btnLoadBankExcel";
            this.btnLoadBankExcel.ShowImage = true;
            this.btnLoadBankExcel.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnLoadBankExcel_Click);
            // 
            // HomeFinanceRibbon
            // 
            this.Name = "HomeFinanceRibbon";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.tabHomeFinance);
            this.tabHomeFinance.ResumeLayout(false);
            this.tabHomeFinance.PerformLayout();
            this.grpHomeFinance.ResumeLayout(false);
            this.grpHomeFinance.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tabHomeFinance;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpHomeFinance;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnFillRashod;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnLoadBankExcel;
    }

    partial class ThisRibbonCollection
    {
        internal HomeFinanceRibbon Ribbon1
        {
            get { return this.GetRibbon<HomeFinanceRibbon>(); }
        }
    }
}
