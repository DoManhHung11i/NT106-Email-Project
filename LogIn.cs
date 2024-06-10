using Lab5;
using MailKit.Net.Imap;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Final_Project
{
    public partial class LogIn : Form
    {
        public static string Username { get; private set; }
        public static string Password { get; private set; }
        public static ImapClient client { get; private set; }
        public static string IMAPHost { get; private set; }
        public static int IMAPPort  { get; private set; }
        public static string SMTPHost { get; private set; }
        public static int SMTPPort { get; private set; }
        public LogIn()
        {
            InitializeComponent();
            passwordBox.UseSystemPasswordChar = true;
        }

        private void LogInBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Username = usernameBox.Text;
                Password = passwordBox.Text;
                IMAPHost = "imap.gmail.com";
                IMAPPort = 993;
                SMTPHost = "smtp.gmail.com";
                SMTPPort = 465;
                client = new ImapClient();
                client.Connect(IMAPHost, IMAPPort, true);
                client.Authenticate(Username, Password);
                MessageBox.Show("Đăng nhập thành công!");
                MailBox mailBox = new MailBox();
                this.Hide();
                mailBox.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LogIn_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void usernameBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void LogIn_Load(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
