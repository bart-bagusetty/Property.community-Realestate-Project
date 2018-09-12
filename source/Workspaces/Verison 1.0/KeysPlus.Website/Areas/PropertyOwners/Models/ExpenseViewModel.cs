using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeysPlus.Website.Areas.PropertyOwners.Models
{
    public class ExpenseViewModel
    {

        public int Id { get; set; }
        public int PropertyId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        

    }
    
}