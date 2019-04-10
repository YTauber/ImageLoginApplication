using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClassLibrary1;
using System.IO;
using ImageLoginApplication.Models;
using System.Web.Security;

namespace ImageLoginApplication.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        // GET: User
        public ActionResult IndexUser()
        {
            ImageManager mgr = new ImageManager(Properties.Settings.Default.ConStr);
            UserViewModel vm = new UserViewModel();
            UserManager mgr2 = new UserManager(Properties.Settings.Default.ConStr);

            User user = mgr2.GetUserByEmail(User.Identity.Name);

            vm.Images = mgr.GetImagesByUserId(user.Id);

            vm.id = (string)TempData["Id"];
            vm.password = (string)TempData["Password"];
            return View(vm);
        }

        public ActionResult UploadImage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase image, string password)
        {
            UserManager mgr = new UserManager(Properties.Settings.Default.ConStr);
            User user = mgr.GetUserByEmail(User.Identity.Name);

            string hash = BCrypt.Net.BCrypt.HashPassword(password);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            image.SaveAs(Path.Combine(Server.MapPath("/UploadedImages"), fileName));

            //string ext = Path.GetExtension(image.FileName);
            //string fileName = Guid.NewGuid().ToString() + ext;
            //string fullPath = $"{Server.MapPath("/UploadedImages")}\\{fileName}";
            //image.SaveAs(fullPath);

            //string ext = Path.GetExtension(image.FileName);
            //string fileName = Guid.NewGuid().ToString() + ext;
            //string fullPath = $"{Server.MapPath("/UploadedImages")}\\{fileName}";

            Image i = new Image
            {
                FileName = fileName,
                HashPassword = hash,
                Views = 0,
                UserId = user.Id
            };

            ImageManager mgr2 = new ImageManager(Properties.Settings.Default.ConStr);
            mgr2.InsertImage(i);

            TempData["Id"] = i.Id.ToString();
            TempData["Password"] = password;

            return Redirect("/user/indexuser");
        }

        
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("/home/index");
        }
    }
}