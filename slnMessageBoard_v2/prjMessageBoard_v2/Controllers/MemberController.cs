using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Data.SqlClient;

namespace prjMessageBoard_v2.Controllers
{
    public class MemberController : Controller
    {
        //連接至MSSQL EXPRESS資料庫連接字串
        string _conStr = @"Server = .\SQLEXPRESS; Database = dbMessageBoard; Integrated Security = true;";

        //----------------------------------------------------------------------------------------------
        //GET: Member/CreateMember
        public ActionResult CreateMember()
        {
            return View();
        }
        //-----------------------------------------------------------------------------------------------
        // POST: Member/CreateMember
        [HttpPost]
        public ActionResult CreateMember(string Account, string Password, string Name)
        {
            //會員註冊
            //使用ADO.NET判斷資料庫內有無相同的帳號，
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(_conStr);
            SqlCommand cmdCheck = con.CreateCommand();
            cmdCheck.CommandText = "dbo.usp_Member_Check";
            cmdCheck.CommandType = CommandType.StoredProcedure;

            cmdCheck.Parameters.Add("@Account", SqlDbType.VarChar);
            cmdCheck.Parameters["@Account"].Value = Account;
            SqlDataAdapter adp = new SqlDataAdapter(cmdCheck);
            adp.Fill(dt); 

            //通過表單取得使用者輸入資料
            //若有 → 顯示帳號已有人註冊 → 重新註冊
            if (dt.Rows.Count != 0)
            {
                ViewBag.Alert = "此帳號已被註冊，請使用別的帳號";
                return View();
            }

            //若無 → 將會員資料存入ADO.NET → 導向會員登入頁面
            SqlCommand cmdCreate = con.CreateCommand();
            cmdCreate.CommandText = "dbo.usp_Member_Add";
            cmdCreate.CommandType = CommandType.StoredProcedure;
            con.Open();

            cmdCreate.Parameters.Add("@Account", SqlDbType.VarChar);
            cmdCreate.Parameters["@Account"].Value = Account;
            cmdCreate.Parameters.Add("@Password", SqlDbType.VarChar);
            cmdCreate.Parameters["@Password"].Value = Password;
            cmdCreate.Parameters.Add("@Name", SqlDbType.NVarChar);
            cmdCreate.Parameters["@Name"].Value = Name;

            cmdCreate.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("Login");
        }
        //----------------------------------------------------------------------------------------------
        // GET: Member/Login
        public ActionResult Login()
        {
            return View();
        }
        //-----------------------------------------------------------------------------------------------
        // POST: Member/Login
        [HttpPost]
        public ActionResult Login(string Account, string Password)
        {
            //會員登入
            //使用ADO.NET判斷資料庫內是否有相符的資料
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(_conStr);
            SqlCommand cmdCheck = con.CreateCommand();
            cmdCheck.CommandText = "dbo.usp_Member_Login";
            cmdCheck.CommandType = CommandType.StoredProcedure;

            cmdCheck.Parameters.Add("@Account", SqlDbType.VarChar);
            cmdCheck.Parameters.Add("@Password", SqlDbType.VarChar);
            cmdCheck.Parameters["@Account"].Value = Account;
            cmdCheck.Parameters["@Password"].Value = Password;

            SqlDataAdapter adp = new SqlDataAdapter(cmdCheck);
            adp.Fill(dt);

            //有相符資料 → 紀錄Session，重新導向頁面至留言列表
            if (dt.Rows.Count != 0)
            {
                Session["MemberID"] = dt.Rows[0]["ID"];
                Session["Welcome"] = dt.Rows[0]["Account"] + "(" + dt.Rows[0]["Name"] + ") 您好";
                return RedirectToAction("GetMessageList","Message");
            }
            //無相符資料 → 顯示錯誤，重新導向頁面至會員登入
            ViewBag.Alert = "輸入的帳號或密碼有誤！請重新輸入";
            return View();
        }
        //----------------------------------------------------------------------------------------------
        // GET: Member/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("GetMessageList", "Message");
        }
    }
}