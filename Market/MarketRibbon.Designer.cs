// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
namespace Market
{
    partial class MarketRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public MarketRibbon()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MarketRibbon));
            this.tabInvest = this.Factory.CreateRibbonTab();
            this.grpOptions = this.Factory.CreateRibbonGroup();
            this.btnOptions = this.Factory.CreateRibbonButton();
            this.grpTinkoff = this.Factory.CreateRibbonGroup();
            this.btnToken = this.Factory.CreateRibbonButton();
            this.grpValute = this.Factory.CreateRibbonGroup();
            this.btnRefreshValuteCurs = this.Factory.CreateRibbonButton();
            this.lbUSD = this.Factory.CreateRibbonLabel();
            this.lbEUR = this.Factory.CreateRibbonLabel();
            this.grpStock = this.Factory.CreateRibbonGroup();
            this.btnMarketList = this.Factory.CreateRibbonButton();
            this.btnRefreshAll = this.Factory.CreateRibbonButton();
            this.btnTinkoff = this.Factory.CreateRibbonButton();
            this.btnRefreshBrokers = this.Factory.CreateRibbonButton();
            this.grpInvest = this.Factory.CreateRibbonGroup();
            this.btnBrokerBuy = this.Factory.CreateRibbonButton();
            this.btnBrokerSell = this.Factory.CreateRibbonButton();
            this.btnDividend = this.Factory.CreateRibbonButton();
            this.btnProfit = this.Factory.CreateRibbonButton();
            this.tabInvest.SuspendLayout();
            this.grpOptions.SuspendLayout();
            this.grpTinkoff.SuspendLayout();
            this.grpValute.SuspendLayout();
            this.grpStock.SuspendLayout();
            this.grpInvest.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabInvest
            // 
            this.tabInvest.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tabInvest.Groups.Add(this.grpOptions);
            this.tabInvest.Groups.Add(this.grpTinkoff);
            this.tabInvest.Groups.Add(this.grpValute);
            this.tabInvest.Groups.Add(this.grpStock);
            this.tabInvest.Groups.Add(this.grpInvest);
            this.tabInvest.Label = "Инвестиции";
            this.tabInvest.Name = "tabInvest";
            // 
            // grpOptions
            // 
            this.grpOptions.Items.Add(this.btnOptions);
            this.grpOptions.Label = "Настройка";
            this.grpOptions.Name = "grpOptions";
            // 
            // btnOptions
            // 
            this.btnOptions.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnOptions.Image")));
            this.btnOptions.Label = "Настройки Excel";
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.ShowImage = true;
            this.btnOptions.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnOptions_Click);
            // 
            // grpTinkoff
            // 
            this.grpTinkoff.Items.Add(this.btnToken);
            this.grpTinkoff.Label = "Токен зарегистрирован";
            this.grpTinkoff.Name = "grpTinkoff";
            // 
            // btnToken
            // 
            this.btnToken.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnToken.Image = global::Market.Properties.Resources.Security;
            this.btnToken.Label = "Токен авторизации (Тинькофф)";
            this.btnToken.Name = "btnToken";
            this.btnToken.ShowImage = true;
            this.btnToken.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnToken_Click);
            // 
            // grpValute
            // 
            this.grpValute.Items.Add(this.btnRefreshValuteCurs);
            this.grpValute.Items.Add(this.lbUSD);
            this.grpValute.Items.Add(this.lbEUR);
            this.grpValute.Label = "Курсы валют";
            this.grpValute.Name = "grpValute";
            // 
            // btnRefreshValuteCurs
            // 
            this.btnRefreshValuteCurs.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnRefreshValuteCurs.Image = global::Market.Properties.Resources.Обновить;
            this.btnRefreshValuteCurs.Label = "Обновить курсы валют";
            this.btnRefreshValuteCurs.Name = "btnRefreshValuteCurs";
            this.btnRefreshValuteCurs.ShowImage = true;
            this.btnRefreshValuteCurs.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnRefreshValuteCurs_Click);
            // 
            // lbUSD
            // 
            this.lbUSD.Label = "USD = ";
            this.lbUSD.Name = "lbUSD";
            // 
            // lbEUR
            // 
            this.lbEUR.Label = "EUR=";
            this.lbEUR.Name = "lbEUR";
            // 
            // grpStock
            // 
            this.grpStock.Items.Add(this.btnMarketList);
            this.grpStock.Items.Add(this.btnRefreshAll);
            this.grpStock.Items.Add(this.btnTinkoff);
            this.grpStock.Items.Add(this.btnRefreshBrokers);
            this.grpStock.Label = "Биржа";
            this.grpStock.Name = "grpStock";
            // 
            // btnMarketList
            // 
            this.btnMarketList.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnMarketList.Image = ((System.Drawing.Image)(resources.GetObject("btnMarketList.Image")));
            this.btnMarketList.Label = "Обновить список акций";
            this.btnMarketList.Name = "btnMarketList";
            this.btnMarketList.ShowImage = true;
            this.btnMarketList.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnMarketList_Click);
            // 
            // btnRefreshAll
            // 
            this.btnRefreshAll.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnRefreshAll.Image = global::Market.Properties.Resources.Обновить;
            this.btnRefreshAll.Label = "Обновить все портфели";
            this.btnRefreshAll.Name = "btnRefreshAll";
            this.btnRefreshAll.ShowImage = true;
            this.btnRefreshAll.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnRefreshAll_Click);
            // 
            // btnTinkoff
            // 
            this.btnTinkoff.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnTinkoff.Image = global::Market.Properties.Resources.Tinkoff;
            this.btnTinkoff.Label = "Обновить Тинькофф";
            this.btnTinkoff.Name = "btnTinkoff";
            this.btnTinkoff.ShowImage = true;
            this.btnTinkoff.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnTinkoff_Click);
            // 
            // btnRefreshBrokers
            // 
            this.btnRefreshBrokers.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnRefreshBrokers.Image = global::Market.Properties.Resources.VTB;
            this.btnRefreshBrokers.Label = "Обновить ВТБ, Сбербанк и прочие";
            this.btnRefreshBrokers.Name = "btnRefreshBrokers";
            this.btnRefreshBrokers.ShowImage = true;
            this.btnRefreshBrokers.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnRefreshBrokers_Click);
            // 
            // grpInvest
            // 
            this.grpInvest.Items.Add(this.btnBrokerBuy);
            this.grpInvest.Items.Add(this.btnBrokerSell);
            this.grpInvest.Items.Add(this.btnDividend);
            this.grpInvest.Items.Add(this.btnProfit);
            this.grpInvest.Label = "Инвестиции";
            this.grpInvest.Name = "grpInvest";
            // 
            // btnBrokerBuy
            // 
            this.btnBrokerBuy.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnBrokerBuy.Image = global::Market.Properties.Resources.Plus;
            this.btnBrokerBuy.Label = "Покупка";
            this.btnBrokerBuy.Name = "btnBrokerBuy";
            this.btnBrokerBuy.ShowImage = true;
            this.btnBrokerBuy.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnBrokerBuy_Click);
            // 
            // btnBrokerSell
            // 
            this.btnBrokerSell.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnBrokerSell.Image = global::Market.Properties.Resources.Minus;
            this.btnBrokerSell.Label = "Продажа";
            this.btnBrokerSell.Name = "btnBrokerSell";
            this.btnBrokerSell.ShowImage = true;
            this.btnBrokerSell.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnBrokerSell_Click);
            // 
            // btnDividend
            // 
            this.btnDividend.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnDividend.Image = global::Market.Properties.Resources.Dividend;
            this.btnDividend.Label = "Дивиденд / купон";
            this.btnDividend.Name = "btnDividend";
            this.btnDividend.ShowImage = true;
            this.btnDividend.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnDividend_Click);
            // 
            // btnProfit
            // 
            this.btnProfit.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnProfit.Image = global::Market.Properties.Resources.Profit;
            this.btnProfit.Label = "Доходность";
            this.btnProfit.Name = "btnProfit";
            this.btnProfit.ShowImage = true;
            this.btnProfit.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnProfit_Click);
            // 
            // MarketRibbon
            // 
            this.Name = "MarketRibbon";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.tabInvest);
            this.tabInvest.ResumeLayout(false);
            this.tabInvest.PerformLayout();
            this.grpOptions.ResumeLayout(false);
            this.grpOptions.PerformLayout();
            this.grpTinkoff.ResumeLayout(false);
            this.grpTinkoff.PerformLayout();
            this.grpValute.ResumeLayout(false);
            this.grpValute.PerformLayout();
            this.grpStock.ResumeLayout(false);
            this.grpStock.PerformLayout();
            this.grpInvest.ResumeLayout(false);
            this.grpInvest.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tabInvest;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpOptions;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnMarketList;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnToken;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpStock;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnOptions;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpValute;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnRefreshValuteCurs;
        internal Microsoft.Office.Tools.Ribbon.RibbonLabel lbUSD;
        internal Microsoft.Office.Tools.Ribbon.RibbonLabel lbEUR;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnTinkoff;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnRefreshBrokers;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnRefreshAll;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpInvest;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnProfit;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpTinkoff;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnBrokerSell;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnBrokerBuy;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnDividend;
    }

    partial class ThisRibbonCollection
    {
        internal MarketRibbon MarketRibbon
        {
            get { return this.GetRibbon<MarketRibbon>(); }
        }
    }
}
