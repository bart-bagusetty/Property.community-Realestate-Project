using KeysPlus.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KeysPlus.Website.Models;

namespace KeysPlus.Website.Areas.PropertyOwners.Models
{
    public class PropertyFinancialViewModel
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? Mortgage { get; set; }
        public decimal? TotalRepayment { get; set; }
        public decimal? TotalExpenses { get; set; }
        public decimal? Yield { get; set; }
        public decimal? CurrentHomeValue { get; set; }
        public decimal? CapitalGain { get; set; }
        public decimal? ActualRentalPayments { get; set; }
        public decimal? Repayment { get; set; }
        public int Frequency { get; set; }
        public DateTime StartDate{ get; set; }
        public DateTime EndDate { get; set; }
        public decimal? Expense { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public int HomeValueType { get; set; }

    }
}