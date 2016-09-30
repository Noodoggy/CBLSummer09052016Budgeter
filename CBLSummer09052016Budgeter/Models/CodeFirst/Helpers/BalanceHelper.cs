using System;
using CBLSummer09052016Budgeter.Models.CodeFirst.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CBLSummer09052016Budgeter.Models.CodeFirst.ViewModels;

namespace CBLSummer09052016Budgeter.Models.CodeFirst.Helpers
{
    public static class BalanceHelper
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public static decimal GetBalance(int accountId)
        {
            var account = db.Accounts.Find(accountId);
            var balance = account.Balance;
            foreach (var t in account.Transaction.NotVoid())
            {
                if (t.TransactionType.Name == "Expense")
                    balance = balance - t.Amount;
                else if (t.TransactionType.Name == "Income")
                    balance = balance + t.Amount;
            }
            return (balance);
        }

        public static decimal GetReconciledBalance(int accountId)
        {
            var account = db.Accounts.Find(accountId);
            var recbalance = account.ReconciledBalance;
            foreach (var t in account.Transaction.NotVoid())
            {
                if (t.TransactionType.Name == "Expense")
                    recbalance = recbalance - t.ReconciledAmount;
                else if (t.TransactionType.Name == "Income")
                    recbalance = recbalance + t.ReconciledAmount;
            }
            return (recbalance);
        }

        public static decimal GetBalance(int accountId, DateTime date)
        {
            var account = db.Accounts.Find(accountId);
            var balance = account.Balance;
            var transactions = account.Transaction.NotVoid().BeforeDate(date);
            foreach (var t in transactions)
            {
                if (t.TransactionType.Name == "Expense")
                    balance = balance - t.Amount;
                else if (t.TransactionType.Name == "Income")
                    balance = balance + t.Amount;
            }
            return (balance);
        }

        public static decimal GetReconciledBalance(int accountId, DateTime date)
        {
            var account = db.Accounts.Find(accountId);
            var balance = account.Balance;
            var transactions = account.Transaction.NotVoid().AreReconciled().BeforeDate(date);
            foreach (var t in transactions)
            {
                if (t.TransactionType.Name == "Expense")
                    balance = balance - t.Amount;
                else if (t.TransactionType.Name == "Income")
                    balance = balance + t.Amount;
            }
            return (balance);
        }

        public static void IsReconciled(int transId)
        {
            var recTrans = db.Transactions.Find(transId);
            if (recTrans.Amount == recTrans.ReconciledAmount)
            { recTrans.Reconciled = true; }
            db.SaveChanges();
        }

        //public static CategoryBudgetViewModel GetBudgetActual(int budgetId)

        /*      { */           //var model = new CategoryBudgetViewModel();
                               //var budget = db.Budgets.Find(budgetId);
                               //model.TotalBudget = budget.BudgetItem.Select(i => i.Amount).Sum();
                               //var house = db.Households.Find(budget.HouseholdId);
                               //var trans = db.Transactions.Where(a => a.Account.HouseholdId == budget.HouseholdId).ToList();
                               //var bi = db.BudgetItems.Where(b => b.Budget.HouseholdId == budget.HouseholdId).ToList();
                               //model.Categories = bi.Select(c => c.Category).Distinct().ToList();
                               //var transcat = new List<Transaction>().ToList();
                               //model.CategoryBudget = new List<decimal>();
                               //model.CategoryActual = new List<decimal>();
                               //decimal te = 0;
                               //decimal ti = 0;
                               //decimal ca = 0;
                               //foreach (var item in model.Categories)
                               //{
                               //    ca = 0;
                               //    var i = 0;
                               //    model.CategoryBudget.Add(item.BudgetItem.Select(a => a.Amount).Sum());
                               //    transcat = trans.Where(c => c.Category.Name == item.Name).ToList();
                               //    if (transcat != null)
                               //    {
                               //        foreach (var t in transcat)
                               //        {


        //            if (t.TransactionType.Name == "Expense")
        //            {
        //                ca = ca + t.Amount;
        //                te = te + t.Amount;
        //            }
        //            else if (t.TransactionType.Name == "Income")
        //            {
        //                ti = ti + t.Amount;
        //            }
        //        }
        //    }
        //    model.CategoryActual.Add(ca);

        //    i++;
        //}
        //model.TotalExpenses = te;
        //model.TotalIncome = ti;


        //return (model);
        //            var tod = DateTimeOffset.Now;
        //            var budget = db.Budgets.Find(budgetId);
        //            var house = db.Households.Find(budget.HouseholdId);
        //            var bar = (from c in house.Category
        //                       from b in c.Transaction
        //                       where b.TransactionType.Name == "Expense"
        //                       let aSum = (from t in c.Transaction
        //                                   where t.Date.Year == tod.Year && t.Date.Month == tod.Month
        //                                   select t.Amount).DefaultIfEmpty().Sum()

        //                       let bSum = (from a in c.BudgetItem
        //                                   select a.Amount).DefaultIfEmpty().Sum()

        //                       select new
        //                       {
        //                           Category = c.Name,
        //                           Actual = aSum,
        //                           Budgeted = bSum
        //                       }).ToArray();
        //        }
        //    }
        //}
    }
}