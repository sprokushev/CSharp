// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Market
{
    public partial class FormDividend : Form
    {

        public enum DividendType { Dividend, Cupon };

        public ThisAddIn thisAddIn;

        public string seektxt = "";
        public DividendType operation;

        public string ticker = "";
        public string name = "";
        public string account = "";
        public double summa = 0;
        public string currency = "";


        public FormDividend()
        {
            InitializeComponent();
        }

        private void FormDividend_Shown(object sender, EventArgs e)
        {

            boxSeek.Text = seektxt;

            boxAccount.Text = account;

            if (ThisAddIn.ListPapers != null)
            {
                var autoComplete = new AutoCompleteStringCollection();
                autoComplete.AddRange((from item in ThisAddIn.ListPapers select item.name).ToArray());
                boxSeek.AutoCompleteCustomSource = autoComplete;
            }

            if ((ThisAddIn.Currencies != null) && (ThisAddIn.Currencies.payload != null) && (ThisAddIn.Currencies.payload.instruments != null))
            {
                boxCurrency.DataSource = (from item in ThisAddIn.Currencies.payload.instruments select item.currency).ToList();
            }

            if (ThisAddIn.ListPapers != null) 
            {
                ThisAddIn.Paper paper = ThisAddIn.ListPapers.FirstOrDefault(item => item.ticker == ticker);

                if ((paper != null) && (paper.currency != ""))
                    boxCurrency.SelectedItem = paper.currency;
            }

            btnSeek_Click(sender, e);
        }

        private void btnSeek_Click(object sender, EventArgs e)
        {
            thisAddIn.FillListPapersFromExcel();
            var filteredPapers = ThisAddIn.ListPapers.Where(paper => paper.ticker.ToLower().Contains(boxSeek.Text.ToLower()) ||
                                                                     paper.name.ToLower().Contains(boxSeek.Text.ToLower())).ToList();

            listPapers.DataSource = filteredPapers;
            listPapers.DisplayMember = "DisplayText";
            listPapers.ValueMember = "ticker";
            if (filteredPapers.Count>0)
                listPapers.SelectedIndex = 0;
        }

        private void boxSeek_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                btnSeek_Click(sender, e);
                e.Handled = true;
            }
        }

        private void listPapers_SelectedValueChanged(object sender, EventArgs e)
        {
            if (listPapers.SelectedIndex != -1)
            {
                var paper = (ThisAddIn.Paper)listPapers.SelectedItem;
                if (paper != null)
                {
                    ticker = paper.ticker;

                    paper = ThisAddIn.ListPapers.FirstOrDefault(item => item.ticker == ticker);
                    if (paper != null)
                    {
                        name = paper.name;
                        boxCurrency.SelectedItem = paper.currency;
                        string InstrumentTypeName = paper.InstrumentTypeName;
                        if (InstrumentTypeName.Contains("_BOND"))
                        {
                            boxOperation.SelectedItem = "Купон";
                        }
                        else
                        {
                            boxOperation.SelectedItem = "Дивиденд";
                        }
                    }
                }

            }
        }

        private void boxSeek_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Tab))
            {
                btnSeek_Click(sender, e);
                e.Handled = true;
            }

        }

        private void boxCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            currency = (string) boxCurrency.SelectedItem;
        }

        private void boxPrice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnOk_Click(sender, e);
                e.Handled = true;
            }

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void boxSumma_TextChanged(object sender, EventArgs e)
        {
            string decimalSeparator = NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
            string str;

            summa = 0;
            str = boxSumma.Text.Replace(".", decimalSeparator).Replace(",", decimalSeparator);
            double.TryParse(str, out summa);
        }

        private void boxOperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (boxOperation.SelectedItem.ToString() == "Купон")
            {
                operation = DividendType.Cupon;
            }
            else
            {
                operation = DividendType.Dividend;
            }

        }
    }
}