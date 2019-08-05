using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using prjMessageBoard_v2.Models;
using System.Data;
using System.Data.SqlClient;

namespace prjMessageBoard_v2.Controllers
{
    public class MemberController : Controller
    {
        //連接至MSSQL EXPRESS資料庫連接字串
        string _conStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["dbMessageBoard"].ConnectionString;

        /// <summary>
        /// 將頁面導向註冊會員頁面。
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateMember()
        {
            return View();
        }

        /// <summary>
        /// 通過表單取得會員註冊資料，確認資料庫無相同帳號則新增該筆會員紀錄，
        /// 並將頁面導向登入Action。
        /// </summary>
        /// <param name="Account">由表單傳遞，使用者註冊帳號</param>
        /// <param name="Password">由表單傳遞，使用者註冊密碼</param>
        /// <param name="Name">由表單傳遞，使用者註冊名稱</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateMember(string Account, string Password, string Name)
        {
            DataTable dt = new DataTable();
            dt = ModelManager.GetAccount(Account);

            if (dt.Rows.Count != 0)
            {
                ViewBag.Alert = "此帳號已被註冊，請使用別的帳號";
                return View();
            }

            ModelManager.CreateMember(Account,Password,Name);
            return RedirectToAction("Login");
        }

        /// <summary>
        /// 將畫面導向登入頁面。
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 通過表單取得會員登入資料，通過搜尋會員資料庫有無相同紀錄，判斷會員是否登入成功。
        /// 若登入成功，以Session紀錄登錄會員編號作為使用者登入會員的識別；
        /// 若登入失敗，將頁面重新導向＂會員登入＂的檢視頁面。
        /// </summary>
        /// <param name="Account">會員帳號</param>
        /// <param name="Password">會員密碼</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(string Account, string Password)
        {
            DataTable dt = new DataTable();
            dt = ModelManager.GetAccountAndPassword(Account, Password);

            if (dt.Rows.Count != 0)
            {
                SessionManager.MemberID = dt.Rows[0]["ID"];
                SessionManager.MemberWelcome = $"{dt.Rows[0]["Account"]}({dt.Rows[0]["Name"]})您好";
                return RedirectToAction("GetMessageList", "Message");
            }

            ViewBag.Alert = "輸入的帳號或密碼有誤！請重新輸入";
            return View();
        }

        /// <summary>
        /// 會員登出，清除所有Session紀錄，
        /// 將頁面重新導向留言列表。
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("GetMessageList", "Message");
        }
    }
}