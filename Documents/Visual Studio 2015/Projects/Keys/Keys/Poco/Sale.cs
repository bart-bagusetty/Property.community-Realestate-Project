using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Keys.Poco
{   
    public class Sale
    {
        public int Id { get; set; }

        [Required]
        public int? CustomerId { get; set; }

        [Required]
        public int? ProductId { get; set; }

        [Required]
        public int? StoreId { get; set; }

        [Required]
        public DateTime? Datesold { get; set; }        
    }
}