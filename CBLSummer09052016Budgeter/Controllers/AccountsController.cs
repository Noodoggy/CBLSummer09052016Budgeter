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
using CBLSummer09052016Budgeter.Models.CodeFirst.Extensions;
using CBLSummer09052016Budgeter.Models.CodeFirst.Helpers;
using CBLSummer09052016Budgeter.Models.CodeFirst.ViewModels;
using Microsoft.AspNet.Identity;
using static CBLSummer09052016Budgeter.Models.CodeFirst.Extensions.Extensions;

namespace CBLSummer09052016Budgeter.Controllers
{
    [AuthorizeHouseholdRequired]
    public class AccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Accounts
        public ActionResult Index()
        {
            var uhid = Extensions.GetHouseholdId(User.Identity);
            var hid = Convert.ToInt32(uhid, 16);
            var accounts = new AccountsViewModel();
            accounts.AccountList = db.Accounts.Where(ac => ac.HouseholdId == hid).Include(a => a.Household).NotVoid().ToList();
            foreach (var item in accounts.AccountList)
            {
                var trans = item.Transaction;
                foreach (var t in trans)
                {
                    BalanceHelper.IsReconciled(t.Id);
                }
                
            }
            ViewBag.HouseholdId = db.Users.Find(User.Identity.GetUserId()).Household.Id;
            ViewBag.Household = db.Users.Find(User.Identity.GetUserId()).Household.Name;
            ViewBag.Date = DateTimeOffset.Now;
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Name");
            ViewBag.TransactionMethodId = new SelectList(db.TransactionMethods, "Id", "Name");
            
            accounts.NewAccount = new Account();
            accounts.NewAccount.HouseholdId = ViewBag.HouseholdId;


            return View(accounts);
        }

        // GET: Accounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            var date = new DateTime(2016, 09, 13);
            ViewBag.monthBalance = Extensions.IndexBalance(account, date);
            ViewBag.monthReconciledBalance = Extensions.IndexReconciledBalance(account, date);
            ViewBag.currentBalance = BalanceHelper.GetBalance(account.Id);
            ViewBag.reconciledBalance = BalanceHelper.GetReconciledBalance(account.Id);
            return View(account);
        }

        // GET: Accounts/Create
        public ActionResult Create()
        {
            ViewBag.HouseholdId = db.Users.Find(User.Identity.GetUserId()).Household.Id;
            ViewBag.Household = db.Users.Find(User.Identity.GetUserId()).Household.Name;
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,HouseholdId,Name,Balance,ReconciledBalance")] Account account)
        {
            
            account.Household = db.Households.Find(account.HouseholdId);
            account.ReconciledBalance = account.Balance;
            if (ModelState.IsValid)
            {

                db.Accounts.Add(account);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HouseholdId = account.HouseholdId;
            return View(account);
        }

        // GET: Accounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", account.HouseholdId);
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,HouseholdId,Name,Balance,ReconciledBalance")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", account.HouseholdId);
            return View(account);
        }

        // GET: Accounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }



        


        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.Accounts.Find(id);
            account.HouseholdId = null;
            account.Void = true;
            db.SaveChanges();
            return RedirectToAction("Index");
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
