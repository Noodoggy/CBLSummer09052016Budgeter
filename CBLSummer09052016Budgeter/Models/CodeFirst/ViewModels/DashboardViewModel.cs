using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CBLSummer09052016Budgeter.Models.CodeFirst.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TotalBudget { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal TotalIncome { get; set; }
        public List<Account> AccountList { get; set; }
        public List<Transaction> RecentTransactions { get; set; }
        public Transaction NewTransaction { get; set; }
        public int BudgetId { get; set; }
    }
}