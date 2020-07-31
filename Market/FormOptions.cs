using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Market
{
    public partial class FormOptions : Form
    {

        public FormOptions()
        {
            InitializeComponent();
        }

        private void btSave_Click(object sender, EventArgs e)
        {

            // записываем в реестр параметры
            if ((ThisAddIn.keyName != null) && (ThisAddIn.keyName != ""))
            {
                Registry.SetValue(ThisAddIn.keyName, "InvestFileName", boxInvestFileName.Text);

                Registry.SetValue(ThisAddIn.keyName, "marketList", boxMarketList.Text);
                Registry.SetValue(ThisAddIn.keyName, "Tinkoff", boxTinkoff.Text);
                Registry.SetValue(ThisAddIn.keyName, "Profit", boxProfit.Text);
                Registry.SetValue(ThisAddIn.keyName, "Dividend", boxDividend.Text);
                Registry.SetValue(ThisAddIn.keyName, "History", boxHistory.Text);

                string[] Brokers;
                if (listBrokers.Items.Count > 0)
                {
                    Brokers = new string[listBrokers.Items.Count];
                    listBrokers.Items.CopyTo(Brokers, 0);
                }
                else
                    Brokers = null;
                Registry.SetValue(ThisAddIn.keyName, "Brokers", Brokers);

                Registry.SetValue(ThisAddIn.keyName, "TickerName", boxTickerName.Text);
                Registry.SetValue(ThisAddIn.keyName, "LastDateName", boxLastDateName.Text);
                Registry.SetValue(ThisAddIn.keyName, "PriceName", boxPriceName.Text);
                Registry.SetValue(ThisAddIn.keyName, "NominalName", boxNominalName.Text);

                string[] Types;
                if (listTypes.Items.Count > 0)
                {
                    Types = new string[listTypes.Items.Count];
                    listTypes.Items.CopyTo(Types, 0);
                }
                else
                    Types = null;
                Registry.SetValue(ThisAddIn.keyName, "Types", Types);
                

            }
            this.DialogResult = DialogResult.OK;
            this.Close();

        }

        private void btAddBroker_Click(object sender, EventArgs e)
        {
            if (boxNewBroker.Text != "")
            {
                listBrokers.Items.Add(boxNewBroker.Text);
                boxNewBroker.Text = "";
            }
        }

        private void btDelBroker_Click(object sender, EventArgs e)
        {
            if ((listBrokers.Items.Count > 0) && (listBrokers.SelectedIndex != -1)) 
                listBrokers.Items.RemoveAt(listBrokers.SelectedIndex);
        }

        private void btAddType_Click(object sender, EventArgs e)
        {
            if (boxNewType.Text != "")
            {
                string[] keyvalue = boxNewType.Text.Split('=');
                if (keyvalue.Length == 2)
                {
                    listTypes.Items.Add(boxNewType.Text);
                }
                boxNewType.Text = "";
            }

        }

        private void btDelType_Click(object sender, EventArgs e)
        {
            if ((listTypes.Items.Count > 0) && (listTypes.SelectedIndex != -1))
                listTypes.Items.RemoveAt(listTypes.SelectedIndex);
        }

        private void FormOptions_Shown(object sender, EventArgs e)
        {
            // считываем из реестра параметры
            if ((ThisAddIn.keyName != null) && (ThisAddIn.keyName != ""))
            {
                boxInvestFileName.Text = (string)Registry.GetValue(ThisAddIn.keyName, "InvestFileName", "инвестиции");
                boxMarketList.Text = (string)Registry.GetValue(ThisAddIn.keyName, "marketList", "List");
                boxTinkoff.Text = (string)Registry.GetValue(ThisAddIn.keyName, "Tinkoff", "Tinkoff");
                boxProfit.Text = (string)Registry.GetValue(ThisAddIn.keyName, "Profit", "Profit");
                boxDividend.Text = (string)Registry.GetValue(ThisAddIn.keyName, "Dividend", "Dividend");
                boxHistory.Text = (string)Registry.GetValue(ThisAddIn.keyName, "History", "History");

                string[] Brokers = (string[])Registry.GetValue(ThisAddIn.keyName, "Brokers", null);
                if (Brokers != null)
                    foreach (var item in Brokers)
                        listBrokers.Items.Add(item);

                boxTickerName.Text = (string)Registry.GetValue(ThisAddIn.keyName, "TickerName", "Тикер");
                boxLastDateName.Text = (string)Registry.GetValue(ThisAddIn.keyName, "LastDateName", "Дата котировки");
                boxPriceName.Text = (string)Registry.GetValue(ThisAddIn.keyName, "PriceName", "Котировка в валюте");
                boxNominalName.Text = (string)Registry.GetValue(ThisAddIn.keyName, "NominalName", "Номинал");

                string[] Types = (string[])Registry.GetValue(ThisAddIn.keyName, "Types", null);
                if (Types != null)
                    foreach (var item in Types)
                        listTypes.Items.Add(item);



            }

        }
    }
}
