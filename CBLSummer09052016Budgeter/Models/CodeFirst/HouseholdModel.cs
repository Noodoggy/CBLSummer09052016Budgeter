using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CBLSummer09052016Budgeter.Models.CodeFirst
{
    public class Household
    {
        public Household()
        {
            this.Account = new HashSet<Account>();
            this.Budget = new HashSet<Budget>();
            this.User = new HashSet<ApplicationUser>();
            this.Category = new HashSet<Category>();
                
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool Void { get; set; }

        public virtual ICollection<Budget> Budget { get; set; }
        public virtual ICollection<Account> Account { get; set; }
        public virtual ICollection<ApplicationUser> User { get; set; }
        public virtual ICollection<Category> Category { get; set; }
    }
}