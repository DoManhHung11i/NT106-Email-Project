using Final_Project;
using MimeKit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab5
{
    public partial class ViewEmailForm : Form
    {
        public ViewEmailForm()
        {
            InitializeComponent();
        }
        public ViewEmailForm(string subject, string from, string to, string date, string textBody, string htmlBody, List<MimeEntity> attachments)
        {
            InitializeComponent();
            fromLabel.Text = from;
            toLabel.Text = to;
            subjectLabel.Text = subject;
            if (!string.IsNullOrEmpty(htmlBody))
            {
                bodyText.Visible = false;
                webBrowser1.Visible = true;
                webBrowser1.DocumentText = htmlBody;
            }
            else
            {
                webBrowser1.Visible = false;
                bodyText.Visible = true;
                bodyText.Text = textBody;
            }
            if (attachments.Count > 0)
            {
                foreach (var attachment in attachments)
                {
                    ListViewItem item = new ListViewItem(attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name);
                    item.Tag = attachment;
                    attachmentsListView.Items.Add(item);
                }
            }
        }

        private void attachmentsListView_ItemActivate(object sender, EventArgs e)
        {
            if (attachmentsListView.SelectedItems.Count > 0)
            {
                var attachment = attachmentsListView.SelectedItems[0].Tag as MimeEntity;
                if (attachment != null)
                {
                    var fileName = attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name;
                    var filePath = Path.Combine(Path.GetTempPath(), fileName);

                    using (var stream = File.Create(filePath))
                    {
                        if (attachment is MimePart part)
                        {
                            part.Content.DecodeTo(stream);
                        }
                        else if (attachment is MessagePart rfc822)
                        {
                            rfc822.Message.WriteTo(stream);
                        }
                    }

                    System.Diagnostics.Process.Start(filePath);
                }
            }
        }

        private void replyBtn_Click(object sender, EventArgs e)
        {
            
            string originalSender = fromLabel.Text;
            string replyTo = toLabel.Text;  
            string originalSubject = subjectLabel.Text;
            string replySubject = $"Re: {originalSubject}";

            string originalBody;
            if (webBrowser1.Visible)
            {
                
                originalBody = webBrowser1.DocumentText;
            }
            else
            {
                
                originalBody = bodyText.Text;
            }

            
            string quotedBody = $"\n\n--- Tin nhắn gốc ---\nFrom: {originalSender}\nTo: {replyTo}\nSubject: {originalSubject}\n\n{originalBody}";

            
            SendEmailForm sendEmailForm = new SendEmailForm(
                username: replyTo,  
                password: LogIn.Password,       
                SMTPHostName: "smtp.gmail.com",  
                SMTPHostPort: 465     
            );

            
            
            sendEmailForm.receiverBox.Text = ExtractGmailAccount(originalSender);
            sendEmailForm.subjectBox.Text = replySubject;
            sendEmailForm.bodyBox.Text = quotedBody;

            
            sendEmailForm.Show();
        }

        private void forwardBtn_Click(object sender, EventArgs e)
        {
            // Lấy thông tin từ email hiện tại
            string senderEmail = toLabel.Text;
            string subject = "Fwd: " + subjectLabel.Text;
            string to = ""; // Để trống để người dùng tự nhập
            string textBody = "---------- Forwarded message ----------\n" +
                                "From: " + fromLabel.Text + "\n" +
                                "To: " + toLabel.Text + "\n" +
                                "Subject: " + subjectLabel.Text + "\n\n" +
                                bodyText.Text + webBrowser1.DocumentText;
            SendEmailForm sendEmailForm = new SendEmailForm(
                username: senderEmail,  
                password: LogIn.Password,       
                SMTPHostName: "smtp.gmail.com", 
                SMTPHostPort: 465   
            );

            
            sendEmailForm.subjectBox.Text = subject;
            sendEmailForm.bodyBox.Text = textBody;


            sendEmailForm.Show();
        }
        
        string ExtractGmailAccount(string originalSender)
        {
            // Kiểm tra xem chuỗi originalSender có chứa ký tự "<" và ">" hay không
            int startIndex = originalSender.IndexOf('<');
            int endIndex = originalSender.IndexOf('>');

            if (startIndex != -1 && endIndex != -1 && endIndex > startIndex)
            {
                return originalSender.Substring(startIndex + 1, endIndex - startIndex - 1);
            }
            else
            {
                return originalSender;
            }
        }

        private void fromLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
