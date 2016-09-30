using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CBLSummer09052016Budgeter.Models;
using CBLSummer09052016Budgeter.Models.CodeFirst;

using Microsoft.AspNet.Identity;
using CBLSummer09052016Budgeter.Models.CodeFirst.Helpers;

namespace CBLSummer09052016Budgeter.Controllers
{
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index()
        {
            var transactions = db.Transactions.Include(t => t.Account).Include(t => t.Category);
            foreach (var item in transactions)
            {
                BalanceHelper.IsReconciled(item.Id);
            }
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create()

        {
            ViewBag.Date = DateTimeOffset.Now;
            
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Name");
            ViewBag.TransactionMethodId = new SelectList(db.TransactionMethods, "Id", "Name");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AccountId,Description,Amount,TransactionTypeId,TransactionMethodId,CategoryId,Reconciled,ReconciledAmount")] Transaction transaction)
        {
            transaction.Account = db.Accounts.Find(transaction.AccountId);
            transaction.Category = db.Categories.Find(transaction.CategoryId);
            transaction.EnteredById = User.Identity.GetUserId();
            transaction.EnteredBy = db.Users.Find(transaction.EnteredById);
            transaction.Date = DateTimeOffset.Now.LocalDateTime;
            var userId = transaction.EnteredBy;
            var h = userId.HouseholdId;
            var house = db.Households.Find(h);
            var category = db.Categories.Where(c => c.Id == transaction.CategoryId).Include(x => x.Household).FirstOrDefault();
            category.Household.Add(transaction.Account.Household);
            var type = db.TransactionTypes.Where(t => t.Id == transaction.TransactionTypeId).Include(y => y.Category).FirstOrDefault();
            type.Category.Add(transaction.Category);
            category.TransactionType.Add(type);
            db.SaveChanges();
            if (ModelState.IsValid)
            { 
                transaction.TransactionType = db.TransactionTypes.Find(transaction.TransactionTypeId);
                
                db.Transactions.Add(transaction);
                
                //var balance = db.Accounts.Find(transaction.AccountId).Balance;
                //if (transaction.TransactionType.Name == "Expense")
                //    balance = balance - transaction.Amount;
                //else if (transaction.TransactionType.Name == "Income")
                //    balance = balance + transaction.Amount;
                db.SaveChanges();
                BalanceHelper.IsReconciled(transaction.Id);
                return RedirectToAction("Index", "Accounts");
            }

            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Name", transaction.AccountId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Name");
            ViewBag.TransactionMethodId = new SelectList(db.TransactionMethods, "Id", "Name");
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Name", transaction.AccountId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Name");
            ViewBag.TransactionMethodId = new SelectList(db.TransactionMethods, "Id", "Name");
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AccountId,Description,Date,Amount,TransactionTypeId,TransactionMethodId,CategoryId,EnteredById,Reconciled,ReconciledAmount")] Transaction transaction)
        {
            transaction.Account = db.Accounts.Find(transaction.AccountId);
            transaction.Category = db.Categories.Find(transaction.CategoryId);
            transaction.EnteredById = User.Identity.GetUserId();
            transaction.EnteredBy = db.Users.Find(transaction.EnteredById);
            transaction.Date = DateTimeOffset.Now.LocalDateTime;
            var user = db.Users.Find(User.Identity.GetUserId());
            var h = user.HouseholdId;
            var house = db.Households.Find(h);

            var category = db.Categories.Where(c => c.Id == transaction.CategoryId).Include(x => x.Household).FirstOrDefault();
            house.Category.Add(transaction.Category);
            category.Household.Add(transaction.Account.Household);
            var type = db.TransactionTypes.Find(transaction.TransactionTypeId);
            type.Category.Add(transaction.Category);
            category.TransactionType.Add(transaction.TransactionType);

            db.SaveChanges();
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                BalanceHelper.IsReconciled(transaction.Id);
                return RedirectToAction("Index", "Accounts");
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Name", transaction.AccountId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Name");
            ViewBag.TransactionMethodId = new SelectList(db.TransactionMethods, "Id", "Name");
            
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            transaction.Void = true;
            db.SaveChanges();
            return RedirectToAction("Index", "Accounts");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
