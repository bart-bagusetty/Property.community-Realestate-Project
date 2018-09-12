using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace BartPortfolio.EMailing
{
    public class EmailingFactory
    {
        public static IEmailing Create()
        {
            return (IEmailing)Assembly.Load("BartPortfolio").CreateInstance("BartPortfolio.EMailing.SystemWebEmailing");
        }
    }
}