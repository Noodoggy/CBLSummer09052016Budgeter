using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CBLSummer09052016Budgeter.Models.CodeFirst.ViewModels
{
    public class BudgetViewModel
    {
        public Budget NewBudget { get; set; }
        public IEnumerable<Budget> BudgetList { get; set; }
        public BudgetItem NewBudgetItem { get; set; }
    }

    public class CategoryBudgetViewModel
    {
        public List<Category> Categories { get; set; }
        public List<decimal> CategoryBudget { get; set; }
        public List<decimal> CategoryActual { get; set; }
        public decimal TotalBudget { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal TotalIncome { get; set; }
    }
}