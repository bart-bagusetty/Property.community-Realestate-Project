using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace BartPortfolio.EMailing
{
    public class EmailConfig
    {
        /// <summary>
        /// Mail Address used to send emails out
        /// </summary>
        public static string MailAddress
        {
            get
            {
                return ConfigurationManager.AppSettings["MailAddress"];
            }
        }

        /// <summary>
        /// Authentication user name needed to send mails on
        /// the SMTP Server
        /// </summary>
        public static string ContestSMTPServerAuthPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["SMTPServerAuthPassword"];
            }
        }

        /// <summary>
        /// Authentication user name needed to send mails on
        /// the SMTP Server
        /// </summary>
        public static string ContestSMTPServerAuthUserName
        {
            get
            {
                return ConfigurationManager.AppSettings["SMTPServerAuthUserName"];
            }
        }

        /// <summary>
        /// SMTP Server used to send emails out
        /// </summary>
        public static string SmtpServer
        {
            get
            {
                return ConfigurationManager.AppSettings["SMTPServer"];
            }
        }

        /// <summary>
        /// Port of SMTP Server used to send emails out
        /// </summary>
        public static string SmtpServerPort
        {
            get
            {
                return ConfigurationManager.AppSettings["SMTPServerPort"];
            }
        }

        /// <summary>
        /// CA1053: Static holder types should not have constructors
        /// </summary>
        private void NoInstancesNeeded()
        {
        }
    }
}