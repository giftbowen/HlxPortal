using LeSan.HlxPortal.WebSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LeSan.HlxPortal.WebSite.Controllers
{
    [AccessControl]
    [Authorize]
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        public ActionResult Index()
        {
            return RedirectToAction("AdminIndex");
        }

        public ActionResult AdminIndex()
        {
            List<ApplicationUser> allUsers = IdentityManager.GetAllUsers();
            List<UserViewModel> model = (from user in allUsers
             select new UserViewModel()
             {
                 Id = user.Id,
                 UserName = user.UserName,
                 RegisterTime = user.RegisterTime,
                 Comments = user.Comments,
                 RoleType = user.RoleType
             }).ToList();

            return View(model);
        }

        public ActionResult AddUser()
        {
            return View();
        }

        //
        // POST: /Admin/AddUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser(UserViewModel model)
        {
            var result = IdentityManager.AddUser(model);
            if (result == null || result.Count() == 0)
            {
                return RedirectToAction("AdminIndex");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult EditUser(string uId)
        {
            UserViewModel model = IdentityManager.GetUser(uId);
            model.Password = model.ConfirmPassword = string.Empty;
            return View(model);
        }

        //
        // POST: /Admin/AddUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(UserViewModel model)
        {
            var result = IdentityManager.UpdateUser(model);
            if (string.IsNullOrEmpty(result))
            {
                return RedirectToAction("AdminIndex");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
	}
}