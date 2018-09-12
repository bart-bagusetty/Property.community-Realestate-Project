using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BartPortfolio.EMailing
{
    public interface IEmailing
    {
        void SendEmail(EmailingMessage mail);

        void SendEmailAttachment(EmailingMessage Objmail);
    }
}