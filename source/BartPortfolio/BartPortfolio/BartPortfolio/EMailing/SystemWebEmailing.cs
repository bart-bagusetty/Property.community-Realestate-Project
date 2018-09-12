using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Web;

namespace BartPortfolio.EMailing
{
    public class SystemWebEmailing: IEmailing
    {
        #region IEmailing Members

        /// <summary>
        /// Sends the given message
        /// </summary>
        public void SendEmail(EmailingMessage mail)
        {

            Thread thread = new Thread(x => sendEmail(mail));
            thread.Start(mail);
        }
        private void sendEmail(EmailingMessage mail)
        {
            try
            {
                var mailMessage = new MailMessage();
                mailMessage.To.Add(mail.ToEmail);
                mailMessage.From = new MailAddress(EmailConfig.MailAddress);
                mailMessage.Subject = mail.Subject;
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = mail.Body;

                var mailClient = new SmtpClient(EmailConfig.SmtpServer);
                int iPort;
                if (int.TryParse(EmailConfig.SmtpServerPort, out iPort))
                {
                    mailClient.Port = iPort;
                }
                mailClient.Credentials = new NetworkCredential(EmailConfig.ContestSMTPServerAuthUserName,
                                                               EmailConfig.ContestSMTPServerAuthPassword);
                mailClient.EnableSsl = true;
                mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                mailClient.Send(mailMessage);

                Thread.Sleep(900);
            }
            catch (SmtpException ex)
            {

            }
        }

        public void SendEmailAttachment(EmailingMessage Objmail)
        {

            Thread thread = new Thread(x => sendemailAttachment(Objmail));
            thread.Start(Objmail);
        }
        private void sendemailAttachment(EmailingMessage Objmail)
        {
            var mailMessage = new MailMessage();
            mailMessage.To.Add(Objmail.ToEmail);
            mailMessage.From = new MailAddress(EmailConfig.MailAddress);
            mailMessage.Subject = Objmail.Subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = Objmail.Body;
            if (Objmail.Attachment != null)
            {
                System.Net.Mail.Attachment attachment;
                attachment = new System.Net.Mail.Attachment(Objmail.Attachment);
                mailMessage.Attachments.Add(attachment);
            }

            var mailClient = new SmtpClient(EmailConfig.SmtpServer);
            int iPort;
            if (int.TryParse(EmailConfig.SmtpServerPort, out iPort))
            {
                mailClient.Port = iPort;
            }
            mailClient.Credentials = new NetworkCredential(EmailConfig.ContestSMTPServerAuthUserName,
                                                           EmailConfig.ContestSMTPServerAuthPassword);
            mailClient.EnableSsl = true;
            mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            mailClient.Send(mailMessage);

            Thread.Sleep(900);
        }


        #endregion
    }
}