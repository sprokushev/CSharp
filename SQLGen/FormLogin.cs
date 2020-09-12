using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLGen
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }

        public string connetionString;
        public List<ConnectDB> ListConnects = new List<ConnectDB>();

        private void btConnect_Click(object sender, EventArgs e)
        {
            if ((cbAuthentication.SelectedIndex == 1) && (tbPassword.Text.Trim() == "") )
            {
                MessageBox.Show("Введите пароль !");
                tbPassword.Focus();
                return;
            }

            if ((cbAuthentication.SelectedIndex == 1) && (tbUsername.Text.Trim() == ""))
            {
                MessageBox.Show("Заполните имя пользователя !");
                tbUsername.Focus();
                return;
            }

            if (tbServerName.Text.Trim() == "")
            {
                MessageBox.Show("Заполните название или IP-адрес сервера !");
                tbServerName.Focus();
                return;
            }

            if (tbDatabaseName.Text.Trim() == "")
            {
                MessageBox.Show("Заполните название базы данных !");
                tbDatabaseName.Focus();
                return;
            }

            connetionString = "";

            if (cbTypeDB.SelectedIndex == 0) // MSSQL
            {
                connetionString = "Data Source=" + tbServerName.Text + ";Initial Catalog=" + tbDatabaseName.Text;

                if (cbAuthentication.SelectedIndex == 0) // Windows login
                {
                    connetionString = connetionString + ";Integrated Security=true";
                }

                if (cbAuthentication.SelectedIndex == 1) // Database login
                {
                    connetionString = connetionString + ";User ID=" + tbUsername.Text + ";Password=" + tbPassword.Text;
                }

            }

            if (cbTypeDB.SelectedIndex == 1) // PGSQL
            {

                connetionString = "Host=" + tbServerName.Text.ToLower() + ";Database=" + tbDatabaseName.Text.ToLower();

                if (cbAuthentication.SelectedIndex == 0) // Windows login
                {
                    connetionString = connetionString + ";Integrated Security=true";
                }

                if (cbAuthentication.SelectedIndex == 1) // Database login
                {
                    connetionString = connetionString + ";Username=" + tbUsername.Text.ToLower() + ";Password=" + tbPassword.Text;
                }

            }
        }

        private void cbAuthentication_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAuthentication.SelectedIndex == 0) // Windows Login
            {
                if (tbUsername != null)
                {
                    tbUsername.Enabled = false;
                }
                if (tbPassword != null)
                {
                    tbPassword.Enabled = false;
                }
            }
            else //Database Login
            {

                if (tbUsername != null)
                {
                    tbUsername.Enabled = true;
                }
                if (tbPassword != null)
                {
                    tbPassword.Enabled = true;
                }
            }

        }

        public string SetConnectionName ()
        {
            string newName = "";

            string newDBType = "";
            switch (cbTypeDB.SelectedIndex)
            {
                case 1:
                    newDBType = "Postgre SQL";
                    break;
                case 0:
                default:
                    newDBType = "Microsoft SQL";
                    break;
            }
            string newServer = tbServerName.Text.Trim();
            string newDatabase = tbDatabaseName.Text.Trim();


            if ( (newDBType != "") && (newServer != "") && (newDatabase != "") )
            {
                newName = newDBType;
                if (newServer != "")
                {
                    newName = newName + " - " + newServer;
                }

                if (newDatabase != "")
                {
                    newName = newName + " ( " + newDatabase + " )";
                }
            }

            return newName;
        }


        private void tbConnectionName_TextChanged(object sender, EventArgs e)
        {
            if (tbConnectionName.Text == "") tbConnectionName.Text = SetConnectionName();
        }

        private void cbConnectionHistory_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = ListConnects.Find(x => (x.DBConnectionName == cbConnectionHistory.SelectedItem.ToString()));

            if (item != null)
            {
                switch (item.ConnType)
                {
                    case ConnType.PGSQL:
                        cbTypeDB.SelectedIndex = 1;
                        break;
                    case ConnType.MSSQL:
                    case ConnType.None:
                    default:
                        cbTypeDB.SelectedIndex = 0;
                        break;
                }

                tbServerName.Text = item.ServerName;
                tbDatabaseName.Text = item.DBName;

                switch (item.AuthType)
                {
                    case AuthType.DATABASE:
                        cbAuthentication.SelectedIndex = 1;
                        break;
                    case AuthType.WINDOWS:
                    default:
                        cbAuthentication.SelectedIndex = 0;
                        break;
                }

                tbUsername.Text = item.Username;

                tbConnectionName.Text = item.DBConnectionName;

            }
        }

        private void cbTypeDB_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbConnectionName.Text = SetConnectionName();
        }

        private void tbServerName_TextChanged(object sender, EventArgs e)
        {
            tbConnectionName.Text = SetConnectionName();
        }

        private void tbDatabaseName_TextChanged(object sender, EventArgs e)
        {
            tbConnectionName.Text = SetConnectionName();
        }

        private void btDel_Click(object sender, EventArgs e)
        {
            if (cbConnectionHistory.SelectedItem != null)
            {
                var item = ListConnects.Find(x => (x.DBConnectionName == cbConnectionHistory.SelectedItem.ToString()));
                if (item != null)
                {
                    ListConnects.Remove(item);
                    cbConnectionHistory.Items.RemoveAt(cbConnectionHistory.SelectedIndex);
                    cbConnectionHistory.Text = "";
                }
            }
        }
    }
}
