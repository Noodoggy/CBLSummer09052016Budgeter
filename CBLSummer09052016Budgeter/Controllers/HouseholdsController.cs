using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CBLSummer09052016Budgeter.Models;
using CBLSummer09052016Budgeter.Models.CodeFirst;
using CBLSummer09052016Budgeter.Models.CodeFirst.Helpers;
using System.Configuration;
using SendGrid;
using System.Net.Mail;
using System.Threading.Tasks;
using static CBLSummer09052016Budgeter.Models.CodeFirst.Extensions.Extensions;
using Microsoft.AspNet.Identity;
using CBLSummer09052016Budgeter.Models.CodeFirst.Extensions;
using System;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using CBLSummer09052016Budgeter.Models.CodeFirst.ViewModels;
using System.Collections.Generic;
using static CBLSummer09052016Budgeter.Models.CodeFirst.Helpers.HouseholdUsersHelper;

namespace CBLSummer09052016Budgeter.Controllers
{
    
    public class HouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private HouseholdUsersHelper huh = new HouseholdUsersHelper();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public HouseholdsController()
        {
        }

        public HouseholdsController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: Households
        [AuthorizeHouseholdRequired]
        public ActionResult Index()
        {
            var users = db.Users.Find(User.Identity.GetUserId());
            var model = new HouseholdIndexViewModel();
            model.List = new List<Household>();
            model.UserList = new List<ApplicationUser>();
            foreach (var item in db.Households.Include(a => a.Account).Include(u => u.User).AsEnumerable())
            {
                if ( users.HouseholdId == item.Id)
                {
                    model.List.Add(item);
                }
            }
            foreach (var user in db.Users)
            {
                if (user.HouseholdId == null ||  model.List.Any(n => n.Id != user.HouseholdId))
                {
                    model.UserList.Add(user);
                }
            }



            return View(model);
        }

        [AuthorizeInvitationRequired]
        public ActionResult AcceptedInvite()
        {
            var user = User.Identity;
            var appUser = db.Users.Find(User.Identity.GetUserId());
            if (appUser.PasswordHash == null)
            {
                RedirectToAction("RegisterFirst");
            }
            HouseholdUsersHelper helper = new HouseholdUsersHelper();
            if (Extensions.IsInvited(user))
            {
                var invite = db.Invitations.FirstOrDefault(i => i.Id == appUser.InvitationId);
                invite.Accepted = true;
                var userId = db.Users.FirstOrDefault(u => u.InvitationId == invite.Id);
                helper.AddToHousehold(userId.Id, invite.HouseholdId);
                db.SaveChanges();

                return RedirectToAction("Details", "Households", new { id = userId.HouseholdId });
            }
            return View("Error");
 
        }

        public ActionResult RegisterFirst()
        {
            ViewBag.Message = "You must register first with the same email your invite was sent to";
            return View();
        }

        // GET: Households/Details/5
        [AuthorizeHouseholdRequired]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // GET: Households/Create
        public ActionResult Create()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId != null)
            {
                
                return RedirectToAction("LeaveHousehold");
            }
            return View();
        }

        // POST: Households/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Household household)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                user.HouseholdId = household.Id;
                db.Households.Add(household);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(household);
        }

        // GET: Households/Edit/5
        //[AuthorizeHouseholdRequired]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Household household)
        {
            if (ModelState.IsValid)
            {
                db.Entry(household).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(household);
        }

        // GET: Households/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Household household = db.Households.Find(id);
            db.Households.Remove(household);
            db.SaveChanges();
            return RedirectToAction("Index");
        }



        public ActionResult AddUser(string userId, int? householdId)
        {
            HouseholdUsersHelper helper = new HouseholdUsersHelper();
            helper.AddToHousehold(userId, householdId);
            return View("Index");
        }

        //[AuthorizeHouseholdRequired]
        //public ActionResult RemoveUser(string userId, int householdId)
        //{
        //    HouseholdUsersHelper helper = new HouseholdUsersHelper();
        //    helper.RemoveFromHousehold(userId, householdId);
        //    return View("Index");
        //}

        public ActionResult ListUsers(int householdId)
        {
            HouseholdUsersHelper helper = new HouseholdUsersHelper();
            return View(helper.ListHouseholdUsers(householdId));
        }

        public ActionResult Invite(string id, int hid)
        {
            var household = db.Households.Find(hid);                                             //find project by id
            HouseholdUsersHelper helper = new HouseholdUsersHelper();                         //helper that can be used locally
            var model = new InviteViewModel();                                     //use model to render in view
            /*model.HouseholdName = household.Name;  */                                             //insert project data
            model.HouseId = household.Id;                                                           //insert project id
            model.HouseholdName = household.Name;
            model.UserToAddName = db.Users.Find(id).DisplayName;
            //model.Household = household;

            model.UserId = id;        //mulitselect list with names and selected as users on project


            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Invite([Bind(Include = "HouseId, UserId")] InviteViewModel model)
        {
            

                var userId = db.Users.Find(model.UserId);
                var invite = new Invitation();
                invite.HouseholdId = model.HouseId;
                invite.Accepted = false;
                
                db.Invitations.Add(invite);
            db.SaveChanges();
                userId.InvitationId = invite.Id;
            db.SaveChanges();

                string callbackUrl = await SendInviteConfirmationTokenAsync(userId.Id, "You've been invited!");

            
                ViewBag.Message = userId.DisplayName + " has been invited to your household.";

            return RedirectToAction("Index");
        } 

        public ActionResult InviteByEmail(int id)
        {
            InviteByEmail model = new Models.CodeFirst.ViewModels.InviteByEmail();
            model.HouseholdId = id;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> InviteByEmail(InviteByEmail emailNewUser)
        {
            var newUser = new ApplicationUser();
            newUser.Email = emailNewUser.Email;
            newUser.HouseholdId = emailNewUser.HouseholdId;

            var invite = new Invitation();
            invite.HouseholdId = emailNewUser.HouseholdId;
            invite.Accepted = false;
            db.Invitations.Add(invite);
            db.SaveChanges();
            newUser.InvitationId = invite.Id;
            
            
            var user = new ApplicationUser { DisplayName = newUser.DisplayName, Email = newUser.Email, FirstName = newUser.FirstName, LastName = newUser.LastName, UserName = newUser.Email, InvitationId = newUser.InvitationId, HouseholdId = newUser.HouseholdId };
            var result = await UserManager.CreateAsync(user);
            if (result.Succeeded)
            {
                string callbackUrl = await SendInviteConfirmationTokenAsync(user.Id, "You've been invited!");
                ViewBag.Message = newUser.Email + " has been invited to your household.";
                return View();
            }
                return RedirectToAction("Index");
          
        }

        public ActionResult LeaveHousehold()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var id = user.HouseholdId;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }

            ViewBag.Message = "In order to create a new household, you must leave the current household.";
            
            return View(household);
        }

        [HttpPost]
        [AuthorizeHouseholdRequired]
        public async Task<ActionResult> LeaveHousehold(int id)
        {
            
            var userId = User.Identity.GetUserId();
            var h = Extensions.GetHouseholdId(User.Identity);
            int hid = 0;
            int.TryParse(h, out hid);
            huh.RemoveFromHousehold(userId, hid);
            await ControllerContext.HttpContext.RefreshAuthentication(db.Users.Find(userId));
            return RedirectToAction("Index");
        }

       
        [AuthorizeHouseholdRequired]
        public async Task<ActionResult> RemoveUser(string id, int hid)
        {
            var userId = db.Users.Find(id).Id;

            huh.RemoveFromHousehold(userId, hid);
            await ControllerContext.HttpContext.RefreshAuthentication(db.Users.Find(userId));
            return RedirectToAction("Index");
        }

        private async Task<string> SendInviteConfirmationTokenAsync(string userID, string subject)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmInvite", "Households",
               new { userId = userID, code = code }, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(userID, subject,
               "Join the household you have been invited to by clicking <a href=\"" + callbackUrl + "\">here</a>");

            return callbackUrl;
        }

        public async Task<ActionResult> ConfirmInvite(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            var hash = db.Users.Find(userId).PasswordHash;
            ViewBag.flag = false;
            if(hash == null)
            { ViewBag.flag = true; }
            return View(result.Succeeded ? "ConfirmInvite" : "Error");
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