using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PasswordManager
{
    public partial class Prideti : Form
    {
        Pagrindinis pgr;
        public Prideti(Pagrindinis currForm)
        {
            pgr = currForm;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBox1.Text) && !String.IsNullOrEmpty(textBox2.Text) && !String.IsNullOrEmpty(textBox3.Text) && !String.IsNullOrEmpty(textBox4.Text) && !String.IsNullOrEmpty(textBox5.Text))
            {
                string pwd = AESHelper.AES_EncryptString(textBox3.Text);
                if (pgr.passList == null) pgr.passList = new List<PassInfo>();
                pgr.passList.Add(new PassInfo(textBox1.Text, textBox2.Text, pwd, textBox4.Text, textBox5.Text));
                pgr.LoadInfo(pgr.passList);
                MessageBox.Show("Slaptažodis Išsaugotas");
                this.Close();
            }
            else
            {
                MessageBox.Show("Yra tuščių langų");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            Random random = new Random();

            string newPwd = "";
            int pwdLength = random.Next(10, 24);
            for (int i = 0; i < pwdLength; i++)
            {
                newPwd += (char)random.Next(33, 122);
            }

            this.textBox3.Text = newPwd;


        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
