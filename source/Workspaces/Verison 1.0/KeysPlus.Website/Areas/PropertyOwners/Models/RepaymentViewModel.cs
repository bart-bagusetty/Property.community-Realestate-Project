using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeysPlus.Website.Areas.PropertyOwners.Models
{
    public class RepaymentViewModel
    {

        public int Id { get; set; }
        public int PropertyId { get; set; }
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
      
        public int FrequencyType { get; set; }
        public string CreatedBy { get; set; }
      
        public DateTime? EndDate { get; set; }
        //   public string CurrentSort { get; set; }

    }
    
}