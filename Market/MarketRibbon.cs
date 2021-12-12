// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
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
            if (btnMarketListClicked != null) btnMarketListClicked(); //-V3083
        }

        public event Action btnTokenClicked;

        private void btnToken_Click(object sender, RibbonControlEventArgs e)
        {
            if (btnTokenClicked != null) btnTokenClicked(); //-V3083
        }

        public event Action btnOptionsClicked;

        private void btnOptions_Click(object sender, RibbonControlEventArgs e)
        {
            if (btnOptionsClicked != null) btnOptionsClicked(); //-V3083
        }

        public event Action btnRefreshValuteCursClicked;

        private void btnRefreshValuteCurs_Click(object sender, RibbonControlEventArgs e)
        {
            if (btnRefreshValuteCursClicked != null) btnRefreshValuteCursClicked(); //-V3083
        }

        public event Action btnTinkoffClicked;

        private void btnTinkoff_Click(object sender, RibbonControlEventArgs e)
        {
            if (btnTinkoffClicked != null) btnTinkoffClicked(); //-V3083
        }

        public event Action btnRefreshAllClicked;

        private void btnRefreshAll_Click(object sender, RibbonControlEventArgs e)
        {
            if (btnRefreshAllClicked != null) btnRefreshAllClicked(); //-V3083
        }

        public event Action btnRefreshBrokersClicked;

        private void btnRefreshBrokers_Click(object sender, RibbonControlEventArgs e)
        {
            if (btnRefreshBrokersClicked != null) btnRefreshBrokersClicked(); //-V3083
        }

        public event Action btnProfitClicked;

        private void btnProfit_Click(object sender, RibbonControlEventArgs e)
        {
            if (btnProfitClicked != null) btnProfitClicked(); //-V3083
        }

        public event Action btnBrokerBuyClicked;

        private void btnBrokerBuy_Click(object sender, RibbonControlEventArgs e)
        {
            if (btnBrokerBuyClicked != null) btnBrokerBuyClicked(); //-V3083
        }

        public event Action btnBrokerSellClicked;

        private void btnBrokerSell_Click(object sender, RibbonControlEventArgs e)
        {
            if (btnBrokerSellClicked != null) btnBrokerSellClicked(); //-V3083
        }

        public event Action btnDividendClicked;

        private void btnDividend_Click(object sender, RibbonControlEventArgs e)
        {
            if (btnDividendClicked != null) btnDividendClicked(); //-V3083
        }
    }
}
