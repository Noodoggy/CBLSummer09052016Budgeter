using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CBLSummer09052016Budgeter.Models.CodeFirst
{
    public class Account
    {
        public Account()
        {
            this.Transaction = new HashSet<Transaction>();
        }
        public int Id { get; set; }
        public int? HouseholdId { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public decimal ReconciledBalance { get; set; }
        public bool Void { get; set; }

        public virtual Household Household { get; set; }

        public virtual ICollection<Transaction>Transaction { get; set; }
        
    }
}