using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CBLSummer09052016Budgeter.Models.CodeFirst
{
    public class TransactionType
    {
        public TransactionType()
        {
              this.Transaction = new HashSet<Transaction>();
              this.Category = new HashSet<Category>();
        }
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Transaction> Transaction { get; set; }
        public ICollection<Category> Category { get; set; }
    }
}