using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using CsvHelper;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;

namespace PasswordManager
{
    public partial class Pagrindinis : Form
    {
        public List<PassInfo> passList = new List<PassInfo>();
        public List<PassInfo> currList = new List<PassInfo>();
        bool passwordsShown = false;
        string uName;
        public Pagrindinis(string name)
        {
            this.uName = name;
            CheckCSVFile();
            InitializeComponent();
            LoadInfo(passList);
            currList = passList;

        }

        public void LoadInfo(List<PassInfo> plist)
        {
            listBox1.Items.Clear();
            if (plist != null)
            {
                foreach (PassInfo pass in plist)
                {

                    listBox1.Items.Add(pass);
                }
            }
        }

        void CheckCSVFile()
        {
            if (!File.Exists(@$"passwords_{uName}.enc"))
            {
                if (!File.Exists(@$"passwords_{uName}.csv"))
                {
                    var myFile = File.Create(@$"passwords_{uName}.csv");
                    myFile.Close();

                }


            }
            else
            {

                AESHelper.AES_DecryptFile(@$"passwords_{uName}.enc", @$"passwords_{uName}.csv");
                File.Delete(@$"passwords_{uName}.enc");
                if (new FileInfo(@$"passwords_{uName}.csv").Length != 0)
                {
                    using (var reader = new StreamReader(@$"passwords_{uName}.csv"))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        while (csv.Read())
                            passList.Add(csv.GetRecord<PassInfo>());
                    }
                    if (passList[0] == null)
                    {
                        passList = null;
                    }
                }
            }
        }

        public void onAppExit()
        {
            LoadInfo(passList);
            currList = passList;

            if (passwordsShown)
            {
                foreach (PassInfo pass in currList)
                {
                    pass.password = AESHelper.AES_EncryptString(pass.password);
                }
            }
            using (var writer = new StreamWriter(@$"passwords_{uName}.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(passList);
            }
        }

        private void Pagrindinis_FormClosed(object sender, FormClosedEventArgs e)
        {
            LoadInfo(passList);
            currList = passList;

            if (passList != null)
            {
                onAppExit();
            }
            AESHelper.AES_EncryptFile(@$"passwords_{uName}.csv", @$"passwords_{uName}.enc");
            File.Delete(@$"passwords_{uName}.csv");
            Application.Exit();
        }

        public static T DeepCopy<T>(T item)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, item);
            stream.Seek(0, SeekOrigin.Begin);
            T result = (T)formatter.Deserialize(stream);
            stream.Close();
            return result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Prideti pridėti = new(this);
            pridėti.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!passwordsShown)
            {
                var a = DeepCopy(currList);
                foreach (var item in a)
                {
                    item.password = AESHelper.AES_DecryptString(item.password);
                }
                LoadInfo(a);
                passwordsShown = true;
            }

            else
            {
                var a = DeepCopy(currList);
                foreach (var item in a)
                {
                    item.password = AESHelper.AES_EncryptString(item.password);
                }
                LoadInfo(currList);
                passwordsShown = false;


            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Atnaujinti atn = new Atnaujinti(passList, this);
            atn.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var a = DeepCopy(passList.Where(x => x.name.Equals(textBox1.Text)).ToList());
            // MessageBox.Show(a.ToString());
            if (a.Count() != 0)
            {
                LoadInfo(a);
                currList = a;
            }
            else
            {
                MessageBox.Show("Nothing found!");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Istrinti istr = new Istrinti(passList, this);
            istr.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            LoadInfo(passList);
            currList = passList;
            if (passList != null)
            {
                onAppExit();
            }
            AESHelper.AES_EncryptFile(@$"passwords_{uName}.csv", @$"passwords_{uName}.enc");
            File.Delete(@$"passwords_{uName}.csv");
            this.Close();
            Application.Exit();
        }

        private void button5_Click(object sender, EventArgs e)
        {

            LoadInfo(passList);
            currList = passList;
            textBox1.Text = "";
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {


            string password = "";
            String str = listBox1.Items[0].ToString();

            string[] stringlist = str.Split(" - ");

                password = stringlist[2];

            if (!passwordsShown)
                password = AESHelper.AES_DecryptString(password);

            Clipboard.SetText(password);
            MessageBox.Show("Slaptažodis išsaugotas į iškarpinę");
        }
    }
}
