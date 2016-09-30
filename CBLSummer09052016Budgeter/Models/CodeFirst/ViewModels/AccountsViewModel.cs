using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CBLSummer09052016Budgeter.Models.CodeFirst.ViewModels
{
    public class AccountsViewModel
    {
        public List<Account> AccountList { get; set; }
        public Transaction NewTransaction { get; set; }
        public Account NewAccount { get; set; }
    }
}