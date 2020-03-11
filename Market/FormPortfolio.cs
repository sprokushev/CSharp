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
    public partial class FormPortfolio : Form
    {

        public enum OperType { Buy, Sell, PlanBuy };

        public ThisAddIn thisAddIn;

        public string seektxt = "";
        public OperType operation;

        public string ticker = "";
        public string name = "";
        public double lot = 0;
        public string InstrumentTypeName = "";
        public string account = "";
        public double count = 0;
        public double price = 0;
        public double summa = 0;
        public string currency = "";
        public double nominal = 0;


        public FormPortfolio()
        {
            InitializeComponent();
        }

        private void FormPortfolio_Shown(object sender, EventArgs e)
        {

            switch (operation)
            {
                case OperType.Buy:
                    lbOperation.Text = "КУПИТЬ";
                    this.Name = "КУПИТЬ ценную бумагу";
                    boxPlan.Checked = false;
                    boxPlan.Visible = true;
                    break;
                case OperType.PlanBuy:
                    lbOperation.Text = "КУПИТЬ";
                    this.Name = "КУПИТЬ ценную бумагу";
                    boxPlan.Checked = true;
                    boxPlan.Visible = true;
                    break;
                case OperType.Sell:
                default:
                    lbOperation.Text = "ПРОДАТЬ";
                    this.Name = "ПРОДАТЬ ценную бумагу";
                    boxPlan.Checked = false;
                    boxPlan.Visible = false;
                    break;
            }

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

        private void boxCount_TextChanged(object sender, EventArgs e)
        {
            string decimalSeparator = NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
            string str;

            price = 0;
            str = boxPrice.Text.Replace(".", decimalSeparator).Replace(",", decimalSeparator);
            double.TryParse(str, out price);

            count = 0;
            str = boxCount.Text.Replace(".", decimalSeparator).Replace(",", decimalSeparator);
            double.TryParse(str, out count);

            summa = Math.Round(price * count, 2);

            boxSumma.Text = summa.ToString();
        }

        private void listPapers_SelectedValueChanged(object sender, EventArgs e)
        {
            if (listPapers.SelectedIndex != -1)
            {
                ticker = ((ThisAddIn.Paper)listPapers.SelectedItem).ticker;
                name = ThisAddIn.ListPapers.FirstOrDefault(item => item.ticker == ticker).name;
                lot = Math.Floor(ThisAddIn.ListPapers.FirstOrDefault(item => item.ticker == ticker).lot) / 1;
                boxLot.Text = lot.ToString();
                InstrumentTypeName = ThisAddIn.ListPapers.FirstOrDefault(item => item.ticker == ticker).InstrumentTypeName;
                boxCurrency.SelectedItem = ThisAddIn.ListPapers.FirstOrDefault(item => item.ticker == ticker).currency;
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

        private void boxNominal_TextChanged(object sender, EventArgs e)
        {
            string decimalSeparator = NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
            string str;

            nominal = 0;
            str = boxNominal.Text.Replace(".", decimalSeparator).Replace(",", decimalSeparator);
            double.TryParse(str, out nominal);

        }

        private void boxCount_Leave(object sender, EventArgs e)
        {
            if ((lot != 0) && ((count % lot) != 0))
            {
                System.Windows.Forms.MessageBox.Show($"Кол-во {count} должно быть кратно размеру лота {lot}");

            }
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

        private void boxPlan_CheckedChanged(object sender, EventArgs e)
        {
            if ((boxPlan.Checked) && (operation == OperType.Buy))
                operation = OperType.PlanBuy;

            if ((! boxPlan.Checked) && (operation == OperType.PlanBuy))
                operation = OperType.Buy;
        }
    }
}