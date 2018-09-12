using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BartPortfolio.EMailing
{
    public class EmailingMessage
    {
        public string FromName { get; set; }

        public string FromEmail { get; set; }
        public string FromEmailPassword { get; set; }

        public string ToEmail { get; set; }

        public string ToName { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string Attachment { get; set; }
    }
}