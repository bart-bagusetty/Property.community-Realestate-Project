using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace BartPortfolio.EMailing
{
    public class EmailClass
    {
        public Boolean SendContact(string name, string email, string subject, string messagetxt)
        {
            try
            {
                String Subject = "Portfolio Contact";
                String FilePath = AppDomain.CurrentDomain.BaseDirectory + "EmailTemplates\\contact.html";
                StreamReader reader = new StreamReader(FilePath);
                String TemplateBlock = reader.ReadToEnd();
                reader.Close();
                var mailService = EmailingFactory.Create();

                StringBuilder MainData = new StringBuilder(TemplateBlock);
                MainData.Replace("##name##", name);
                MainData.Replace("##email##", email);
                MainData.Replace("##subject##", subject);
                MainData.Replace("##message##", messagetxt);

                var message = new EmailingMessage
                {
                    Subject = Subject,
                    ToEmail = ConfigurationManager.AppSettings["ToMail"],
                    Body = MainData.ToString()
                };
                mailService.SendEmail(message);
                MainData = null;

                return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }
    }
}