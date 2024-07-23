using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ViewDemo.Filter;
using ViewDemo.Models;
using ViewDemo.Models.Vm;

namespace ViewDemo.Controllers
{
    [Authorize]
    [PermissionFilters]
    public class HomeController : Controller
    {
        private DbModel db = new DbModel();
        
        public ActionResult Index()
        {
            //ViewData["Name"] = "5566得第一";
            //ViewBag.Name = "F4得第二";

            //TempData["Name"] = "EG得第三";
            return View();
            //return RedirectToAction("About");
        }
        
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //登入Action
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(MemberVm memberVm)
        {
            if (!ModelState.IsValid)
            {
                return View(memberVm);
            }

            Member member = ValidateUser(memberVm.Account, memberVm.Password);
            if (member == null)
            {
                //登入失敗
                ViewBag.Message = "登入失敗";
                return View(memberVm);
            }
            //登入成功，表單驗證
            string userData = JsonConvert.SerializeObject(member);
            Utility.SetAuthenTicket(userData, member.Id.ToString());
            
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 驗證使用者
        /// </summary>
        /// <param name="account">輸入帳號</param>
        /// <param name="password">輸入密碼</param>
        /// <returns></returns>
        private Member ValidateUser(string account,string password)
        {
            //確認帳號是否存在
            Member member=db.Members.FirstOrDefault(m => m.Account == account);
            if (member == null)
            {
                return null;
            }
            //確認密碼是否正確
            //資料庫資料
            string dbPassword = member.Password;
            string salt = member.PasswordSalt;
            //產生雜湊密碼
            var hashPassword = Utility.GenerateHashWithSalt(password, salt);
            if (hashPassword != dbPassword)
            {
                return null;
            }
            return member;
        }

        //登出Action
        [AllowAnonymous]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}