using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ClassLibrary1;
using ImageLoginApplication.Models;

namespace ImageLoginApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewImage(int id)
        {
            ImageManager mgr = new ImageManager(Properties.Settings.Default.ConStr);
            ViewImageModel vm = new ViewImageModel();
            vm.Message = (string)TempData["Message"];
            vm.image = mgr.GetImageById(id);

            if (Session["AllowedId"] != null)
            {
                List<int> AllowedId = (List<int>)Session["AllowedId"];
                if (AllowedId.Contains(id))
                {
                    vm.HasPermission = true;
                    mgr.UpdateView(id);
                }
            }
            

            return View(vm);
        }

        public ActionResult LogIn()
        {
            LogInViewModel vm = new LogInViewModel();
            vm.Message = (string)TempData["Message"];
            return View(vm);
        }

        [HttpPost]
        public ActionResult ViewImage(int id, string password)
        {
            ImageManager mgr = new ImageManager(Properties.Settings.Default.ConStr);
            Image i = mgr.GetImageById(id);
            bool Valid = BCrypt.Net.BCrypt.Verify(password, i.HashPassword);
            if (!Valid)
            {
                TempData["Message"] = "Invalid Password";
            }
            else
            {
                List<int> AllowedId;
                if (Session["AllowedId"] == null)
                {
                    AllowedId = new List<int>();
                    Session["AllowedId"] = AllowedId;
                }
                else
                {
                    AllowedId = (List<int>)Session["AllowedId"];
                }
                AllowedId.Add(id);
            }

            return Redirect($"/home/viewimage?id={id}");
        }

        [HttpPost]
        public ActionResult LogIn(string email, string password)
        {
            UserManager mgr = new UserManager(Properties.Settings.Default.ConStr);
            User user = mgr.LogIn(email, password);

            if (user == null)
            {
                TempData["Message"] = "Invalid Log In";
                return Redirect("/home/login");
            }

            FormsAuthentication.SetAuthCookie(email, true);

            return Redirect("/user/indexuser");
        }

        [HttpPost]
        public ActionResult AddUser(string password, User user)
        {
            user.HashPassword = BCrypt.Net.BCrypt.HashPassword(password);
            UserManager mgr = new UserManager(Properties.Settings.Default.ConStr);
            mgr.InsertUser(user);
            return Redirect("/home/index");
        }
    }
}