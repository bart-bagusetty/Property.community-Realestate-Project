using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeysPlus.Service.Models
{
    public class JobQuoteViewModel
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public decimal Amount { get; set; }
        public string JobQuoteStatus { get; set; }
        public string JobDescription { get; set; }
        public string PropertyName { get; set; }
        public int? ProviderId { get; set; }
        public string ProviderName { get; set; }
        public int? CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyWebsite { get; set; }
        public decimal? MaxBudget { get; set; }
        public string Note { get; set; }        
    }
}