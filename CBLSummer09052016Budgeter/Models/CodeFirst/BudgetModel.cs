using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CBLSummer09052016Budgeter.Models.CodeFirst
{
    public class Budget
    {
        public Budget()
        {
            this.BudgetItem = new HashSet<BudgetItem>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int? HouseholdId { get; set; }
        public bool Void { get; set; }

        public virtual Household Household { get; set; }

        public virtual ICollection<BudgetItem> BudgetItem { get; set; }

    }
}