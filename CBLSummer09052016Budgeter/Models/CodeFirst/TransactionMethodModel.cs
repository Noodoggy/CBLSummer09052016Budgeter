using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CBLSummer09052016Budgeter.Models.CodeFirst
{
    public class TransactionMethod
    {
        public TransactionMethod()
        {
            this.Transaction = new HashSet<Transaction>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        

        public ICollection<Transaction> Transaction { get; set; }
    }
}