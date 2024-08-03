using System.Net.Mail;

namespace WebApplication1.Interfaces
{
    public class EmailNotification : IEmailNotification
    {
        public void SendEmail(string to, string subject, string body)
        {
            // Configure and send email
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("ralphdaher6@gmail.com");
                mail.To.Add(to);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com"))
                {
                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential("ralphdaher6@gmail.com", "utsupemnvjuuksnf");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
        }
    }
}
