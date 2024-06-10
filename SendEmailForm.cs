using MimeKit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab5
{
    public partial class SendEmailForm : Form
    {
        public SendEmailForm()
        {
            InitializeComponent();
        }
        string username;
        string password;
        string SMTPHostName;
        Int32 SMTPHostPort;
        string attachmentPath;
        public SendEmailForm(string username, string password, string SMTPHostName, Int32 SMTPHostPort)
        {
            InitializeComponent();
            this.username = username;
            this.password = password;
            this.SMTPHostName = SMTPHostName;
            this.SMTPHostPort = SMTPHostPort;
            senderGmailBox.Text = username;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
               
                var addr = MailboxAddress.Parse(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void send_Click(object sender, EventArgs e)
        {
            var senderEmail = senderGmailBox.Text;
            var receiverEmail = receiverBox.Text;


            if (!IsValidEmail(senderEmail) || !IsValidEmail(receiverEmail))
            {
                MessageBox.Show("Địa chỉ email không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect(SMTPHostName, SMTPHostPort, true);
                    client.Authenticate(username, password);

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(senderNameBox.Text, senderGmailBox.Text));
                    message.To.Add(new MailboxAddress("", receiverBox.Text));
                    message.Subject = subjectBox.Text;

                    var body = isHTML.Checked ? new TextPart("html") { Text = bodyBox.Text } : new TextPart("plain") { Text = bodyBox.Text };
                    var multipart = new Multipart("mixed");
                    multipart.Add(body);
                    if (!string.IsNullOrEmpty(attachmentPath))
                    {
                        var attachment = new MimePart()
                        {
                            Content = new MimeContent(System.IO.File.OpenRead(attachmentPath)),
                            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                            ContentTransferEncoding = ContentEncoding.Base64,
                            FileName = System.IO.Path.GetFileName(attachmentPath)
                        };
                        multipart.Add(attachment);
                    }
                    message.Body = multipart;

                    client.Send(message);
                    client.Disconnect(true);

                    MessageBox.Show("Gửi email thành công!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gửi email thất bại. Lỗi: {ex.Message}");
            }
        }

        private void browserBtn_Click(object sender, EventArgs e)
        {
            using(OpenFileDialog ofd = new OpenFileDialog())
            {
                if(ofd.ShowDialog() == DialogResult.OK)
                {
                    attachmentPath = ofd.FileName;
                    pathFile.Text = attachmentPath;
                }
            }
        }

        private void receiverBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
