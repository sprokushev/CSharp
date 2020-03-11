using Microsoft.Win32;
using PSVClassLibrary;
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
    public partial class FormAskToken : Form
    {
        public FormAskToken()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (boxToken.Text == "")
            {
                MessageBox.Show("Заполните токен!");
                return;
            }

            ThisAddIn.token = boxToken.Text;

            // Шифруем токен
            byte[] Encrypted_Bytes = CryptoClass.encrypt_function(ThisAddIn.token);

            // Записывем токен в реестр
            if ((ThisAddIn.keyName != null) && (ThisAddIn.keyName != ""))
            {
                Registry.SetValue(ThisAddIn.keyName, "Token", Encrypted_Bytes);
                MessageBox.Show("Токен зашифрован и безопасно сохранен для последующих запусков!");
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
