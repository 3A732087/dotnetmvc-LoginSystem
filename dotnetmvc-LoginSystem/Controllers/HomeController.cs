using dotnetmvc_LoginSystem.Models;
using dotnetmvc_LoginSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace dotnetmvc_LoginSystem.Controllers
{
    public class HomeController : Controller
    {
        dbQuanMembersSystemEntities db = new dbQuanMembersSystemEntities();
        MemberServices memberServices = new MemberServices();

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
                Dictionary<string, string> memberDataList = new Dictionary<string, string>();
                memberDataList = memberServices.HashPassword("Register", newMember.Account, newMember.Password);

                newMember.Password = memberDataList["password"].ToString();
                newMember.Salt = memberDataList["salt"].ToString();

                db.Members.Add(newMember);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Message = "此帳號已有人使用，註冊失敗";
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string account, string password)
        {
            if (ModelState.IsValid == false)
            {
                return View();
            }

            var member = db.Members.Where(m => m.Account == account).FirstOrDefault();
            if(member == null)
            {
                ViewBag.Message = "帳號不存在，請重新輸入。";
                return View();
            }
            else
            {
                Dictionary<string, string> memberDataList = new Dictionary<string, string>();
                memberDataList = memberServices.HashPassword("Login", account, password);
                string passoerdCheck = memberDataList["password"].ToString();
                var memberCheck = db.Members.Where(m => m.Account == account && m.Password == passoerdCheck).FirstOrDefault();
                if(memberCheck == null)
                {
                    ViewBag.Message = "帳號或密碼錯誤，請重新輸入。";
                    return View();
                }
                else
                {
                    Session["User"] = memberCheck.Name;
                    //指定使用者帳號通過驗證（即通過登錄驗證）
                    FormsAuthentication.RedirectFromLoginPage(memberCheck.Name, true);
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        [Authorize]
        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Home");
        }


    }
}