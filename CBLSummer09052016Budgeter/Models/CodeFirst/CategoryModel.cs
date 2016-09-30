using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CBLSummer09052016Budgeter.Models.CodeFirst
{
    public class Category
    {
        public Category()
        {
            this.BudgetItem = new HashSet<BudgetItem>();
            this.Transaction = new HashSet<Transaction>();
            this.Household = new HashSet<Household>();
            this.TransactionType = new HashSet<TransactionType>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
       
        public ICollection<BudgetItem> BudgetItem { get; set; }
        public ICollection<Transaction> Transaction { get; set; }
        public ICollection<Household> Household { get; set; }
        public ICollection<TransactionType> TransactionType { get; set; }
    }
}