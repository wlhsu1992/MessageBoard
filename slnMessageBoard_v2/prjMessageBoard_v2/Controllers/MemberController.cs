using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using prjMessageBoard_v2.Models;

namespace prjMessageBoard_v2.Controllers
{
    public class MemberController : Controller
    {
        //連接至MSSQL EXPRESS資料庫連接字串
        private string _conStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["dbMessageBoard"].ConnectionString;

        /// <summary>
        /// 將頁面導向會員註冊檢視頁面。
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateMember()
        {
            return View();
        }

        /// <summary>
        /// 通過表單取得會員註冊資料，確認資料庫無相同帳號，
        /// 若無，新增會員記錄、導向登入檢視頁面；
        /// 若有，錯誤訊息提示、導向註冊檢視頁面。
        /// </summary>
        /// <param name="account">使用者註冊帳號</param>
        /// <param name="password">使用者註冊密碼</param>
        /// <param name="name">使用者註冊名稱</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateMember(string account, string password, string name)
        {
            bool hasAccount = MessageBoardModelManager.GetAccount(account);

            if (hasAccount)
            {
                ViewBag.Alert = "此帳號已被註冊，請使用別的帳號";
                return View();
            }
            MessageBoardModelManager.CreateMember(account,password,name);
            return RedirectToAction("Login");
        }

        /// <summary>
        /// 將畫面導向會員登入檢視頁面。
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 取得資料庫中符合傳入參數的會員資料。
        /// 若有，建立Session存入會員資料，導向留言列表Actoin；
        /// 若無，導向註冊檢視頁面。
        /// </summary>
        /// <param name="account">會員帳登入號</param>
        /// <param name="password">會員登入密碼</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(string account, string password)
        {
            LoginMemberModel loginMemberInfo = MessageBoardModelManager.CheckLogin(account, password);

            if (loginMemberInfo != null)
            {
                SessionManager.MemberID = loginMemberInfo.MemberID;
                SessionManager.MemberWelcome = $"{loginMemberInfo.Account}({loginMemberInfo.Name})您好";
                return RedirectToAction("GetDiscussionList", "Message");
            }
            ViewBag.Alert = "輸入的帳號或密碼有誤！請重新輸入";
            return View();
        }

        /// <summary>
        /// 會員登出，清除所有Session記錄，
        /// 導向留言列表動作方法。
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("GetDiscussionList", "Message");
        }
    }
}