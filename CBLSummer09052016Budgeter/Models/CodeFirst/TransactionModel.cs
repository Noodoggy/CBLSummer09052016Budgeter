using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CBLSummer09052016Budgeter.Models.CodeFirst
{
    public class Transaction
    {
        public int Id { get; set; }
        [Required]
        public int AccountId { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTimeOffset Date { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public int TransactionTypeId { get; set; }
        [Required]
        public int TransactionMethodId { get; set; }
        [Required]
        public int? CategoryId { get; set; }
        public string EnteredById { get; set; }
        public bool Reconciled { get; set; }
        public decimal ReconciledAmount { get; set; }
        public bool Void { get; set; }

        public virtual Account Account { get; set; }
        public virtual Category Category { get; set; }
        public virtual ApplicationUser EnteredBy { get; set; }
        public virtual TransactionType TransactionType { get; set; }
        public virtual TransactionMethod TransactionMethod { get; set; }
    }
}