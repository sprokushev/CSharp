using Microsoft.Office.Tools.Ribbon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeFinance
{
    public partial class HomeFinanceRibbon
    {
        public event Action btnFillRashodClicked;
        public event Action btnLoadBankExcelClicked;

        private void btnFillRashod_Click(object sender, RibbonControlEventArgs e)
        {
            if (btnFillRashodClicked != null) btnFillRashodClicked(); //-V3083
        }

        private void btnLoadBankExcel_Click(object sender, RibbonControlEventArgs e)
        {
            if (btnLoadBankExcelClicked != null) btnLoadBankExcelClicked(); //-V3083
        }
    }
}
