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
using Newtonsoft.Json;
using BCrypt.Net;

namespace PasswordManager
{
    public partial class Prisijungimas : Form
    {
        List<Users.UserList> users = new();
        public Prisijungimas()
        {
            checkFile();
            InitializeComponent();
            panel1.BringToFront();
        }

        void checkFile()
        {
            if (!File.Exists(@"users.json"))
            {

                panel2.BringToFront();
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            panel1.BringToFront();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel2.BringToFront();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!File.Exists(@"users.json"))
            {
                Users.Root root = new();
                Users.UserList ul = new();
                Users.UserInfo ui = new();
                ui.totalUsers = 1;
                ul.id = 1;
                ul.name = textBox3.Text;
                ul.password = BcryptHelper.HashPassword(textBox4.Text);
                users.Add(ul);
                ui.userList = users;
                root.userInfo = ui;

                using (StreamWriter file = File.CreateText(@"users.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, root);
                }

            }
            else
            {
                Users.Root users;
                using (StreamReader file = File.OpenText(@"users.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    users = (Users.Root)serializer.Deserialize(file, typeof(Users.Root));
                    users.userInfo.totalUsers++;
                    Users.UserList ul = new();
                    ul.id = users.userInfo.totalUsers;
                    ul.name = textBox3.Text;
                    ul.password = BcryptHelper.HashPassword(textBox4.Text);
                    users.userInfo.userList.Add(ul);



                }

                using (StreamWriter file = File.CreateText(@"users.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, users);
                }
            }

            MessageBox.Show("Registracija Sėkminga!");
            panel1.BringToFront();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Users.Root users = null;
            Users.UserList currUser = new();
            using (StreamReader file = File.OpenText(@"users.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                users = (Users.Root)serializer.Deserialize(file, typeof(Users.Root));
                string name = textBox1.Text;
                string pwd = textBox2.Text;

                currUser = users.userInfo.userList.Find(x => x.name == name && BcryptHelper.CheckPassword(pwd, x.password));
                if (currUser != null)
                {
                    Pagrindinis form = new Pagrindinis(name);
                    this.Hide();
                    form.Show();
                }
                else
                {
                    MessageBox.Show("Tokio vartotojo Nėra");
                }



            }
        }
    }
}