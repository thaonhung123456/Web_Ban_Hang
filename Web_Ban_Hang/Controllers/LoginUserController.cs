using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_Ban_Hang.Models;

namespace Web_Ban_Hang.Controllers
{
    public class LoginUserController : Controller
    {
        DBSportStoreEntities db = new DBSportStoreEntities();
        // GET: LoginUser
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LoginAccount(AdminUser _user)
        {
            var check = db.AdminUsers.Where(s => s.NameUser == _user.NameUser && s.PasswordUser == _user.PasswordUser).FirstOrDefault();

            if (check == null)
            {
                ViewBag.ErrInfo = "SaiInfo";
                return View("Index");
            }
            else
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                //Session["ID"] = _user.ID;
                Session["NameUser"] = _user.NameUser;
                return RedirectToAction("Index", "Product");
            }
        }
        public ActionResult RegisterUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RegisterUser(AdminUser _user)
        {
            if (ModelState.IsValid)
            {
                var check_ID = db.AdminUsers.Where(s => s.ID == _user.ID).FirstOrDefault();
                var check_NameUser = db.AdminUsers.Where(s => s.NameUser == _user.NameUser).FirstOrDefault();
                if (check_NameUser == null)
                {
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.AdminUsers.Add(_user);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorRegister = "Tên đã tồn tại";
                    ViewBag.ErrorRegister = "ID đã tồn tại";
                    return View();
                }
            }
            return View();
        }
        public ActionResult LogOutUser()
        {
            Session.Abandon();
            return RedirectToAction("Index", "LoginUser");
        }
    }
}