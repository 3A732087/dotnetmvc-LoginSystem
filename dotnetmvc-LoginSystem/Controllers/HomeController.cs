using dotnetmvc_LoginSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace dotnetmvc_LoginSystem.Controllers
{
    public class HomeController : Controller
    {
        dbQuanMembersSystemEntities db = new dbQuanMembersSystemEntities();

        public ActionResult Index()
        {
            var members = db.Members.ToList();
            return View(members);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Members newMember)
        {
            if (ModelState.IsValid == false)
            {
                return View();
            }

            //依帳號取得會員並指定給member
            var member = db.Members.Where(m => m.Account == newMember.Account).FirstOrDefault();
            if (member == null)
            {
                //產生鹽
                string salt = Guid.NewGuid().ToString("N");
                string saltAndPassword = string.Concat(newMember.Password, salt);
                SHA256CryptoServiceProvider sha256Hash = new SHA256CryptoServiceProvider();
                byte[] passwordData = Encoding.Default.GetBytes(saltAndPassword);
                byte[] hashData = sha256Hash.ComputeHash(passwordData);
                string hashResult = Convert.ToBase64String(hashData);

                newMember.Password = hashResult;
                newMember.Salt = salt;

                db.Members.Add(newMember);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Message = "此帳號已有人使用，註冊失敗";
            return View();
        }
    }
}