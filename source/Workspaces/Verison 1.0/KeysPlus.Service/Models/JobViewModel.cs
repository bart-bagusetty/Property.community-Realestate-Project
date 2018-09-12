using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using KeysPlus.Data;
//using KeysPlus.Areas.ServiceProviders.Models;

namespace KeysPlus.Service.Models
{
    public class JobViewModel
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }

        public int? ProviderId { get; set; }

        public string ProviderCompany { get; set; }
        public string ProviderName { get; set; }
        public DateTime? JobStartDate { get; set; }

        
        public DateTime? JobEndDate { get; set; }
        public int? JobStatusId { get; set; }
        public string JobStatusName { get; set; }
        public int? PercentDone { get; set; }
        public string Note { get; set; }



        public string JobDescription { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatededOn { get; set; }
        public decimal? PaymentAmount { get; set; }
        
        public string PropertyAddress { get; set; }

        public string CompanyAddress { get; set; }
        public decimal? MaxBudget { get; set; }
        public int QuoteNumber { get; set; }
    }



}