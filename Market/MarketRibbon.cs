using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;

namespace Market
{
    public partial class MarketRibbon
    {

        public event Action btnMarketListClicked;

        private void btnMarketList_Click(object sender, RibbonControlEventArgs e)
        {
            if (btnMarketListClicked != null) btnMarketListClicked();
        }

        public event Action btnTokenClicked;

        private void btnToken_Click(object sender, RibbonControlEventArgs e)
        {
            if (btnTokenClicked != null) btnTokenClicked();
        }

        public event Action btnOptionsClicked;

        private void btnOptions_Click(object sender, RibbonControlEventArgs e)
        {
            if (btnOptionsClicked != null) btnOptionsClicked();
        }

        public event Action btnRefreshValuteCursClicked;

        private void btnRefreshValuteCurs_Click(object sender, RibbonControlEventArgs e)
        {
            if (btnRefreshValuteCursClicked != null) btnRefreshValuteCursClicked();
        }

        public event Action btnTinkoffClicked;

        private void btnTinkoff_Click(object sender, RibbonControlEventArgs e)
        {
            if (btnTinkoffClicked != null) btnTinkoffClicked();
        }

        public event Action btnRefreshAllClicked;

        private void btnRefreshAll_Click(object sender, RibbonControlEventArgs e)
        {
            if (btnRefreshAllClicked != null) btnRefreshAllClicked();
        }

        public event Action btnRefreshBrokersClicked;

        private void btnRefreshBrokers_Click(object sender, RibbonControlEventArgs e)
        {
            if (btnRefreshBrokersClicked != null) btnRefreshBrokersClicked();
        }

        public event Action btnProfitClicked;

        private void btnProfit_Click(object sender, RibbonControlEventArgs e)
        {
            if (btnProfitClicked != null) btnProfitClicked();
        }

        public event Action btnBrokerBuyClicked;

        private void btnBrokerBuy_Click(object sender, RibbonControlEventArgs e)
        {
            if (btnBrokerBuyClicked != null) btnBrokerBuyClicked();
        }

        public event Action btnBrokerSellClicked;

        private void btnBrokerSell_Click(object sender, RibbonControlEventArgs e)
        {
            if (btnBrokerSellClicked != null) btnBrokerSellClicked();
        }

        public event Action btnDividendClicked;

        private void btnDividend_Click(object sender, RibbonControlEventArgs e)
        {
            if (btnDividendClicked != null) btnDividendClicked();
        }
    }
}
