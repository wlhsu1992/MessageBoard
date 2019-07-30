using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Data.SqlClient;

namespace prjMessageBoard_v2.Controllers
{
    public class MessageController : Controller
    {
        //連接至MSSQL EXPRESS資料庫連接字串
        string _conStr = @"Server = .\SQLEXPRESS; Database = dbMessageBoard; Integrated Security = true;";

        //----------------------------------------------------------------------------------------------
        // GET: Message/CreateMessage
        public ActionResult CreateMessage()
        {
            if (Session["MemberID"] == null)
                return RedirectToAction("GetMessageList","Message");
            else
                return View("CreateMessage","_LayoutLogin");
        }
        //-----------------------------------------------------------------------------------------------
        // POST: Message/CreateMessage
        [HttpPost]
        public ActionResult CreateMessage(string Title, string Content)
        {
            //使用ADO.NET在Message資料表新增留言資料 [Title,Content,CreateTime]
            SqlConnection con = new SqlConnection(_conStr);
            con.Open();

            SqlCommand cmd = con.CreateCommand();
            cmd.CommandText = "dbo.usp_Message_Add";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Title", SqlDbType.NVarChar);
            cmd.Parameters["@Title"].Value = Title;
            cmd.Parameters.Add("@Content", SqlDbType.NVarChar);
            cmd.Parameters["@Content"].Value = Content;
            cmd.Parameters.Add("@MemberID", SqlDbType.Int);
            cmd.Parameters["@MemberID"].Value = Session["MemberID"];

            cmd.ExecuteNonQuery();
            con.Close();

            return RedirectToAction("GetMessageList","Message");
        }
        //----------------------------------------------------------------------------------------------
        // GET: Message/GetMessageList
        public ActionResult GetMessageList()
        {
            //留言列表
            //使用ADO.NET存取留言板資料表資料，並將其傳遞給前端
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(_conStr);
            con.ConnectionString = _conStr;
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.usp_Message_List";

            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(dt);

            if (dt.Rows.Count == 0)
                ViewBag.Alert = "目前尚無留言";
            
            //判斷使用者是否有登入會員，分別將其導向套用不同的背板
            if (Session["MemberID"] == null)
                return View("GetMessageList", "_LayoutLogout", dt);
            else
                return View("GetMessageList", "_LayoutLogin", dt);
        }
        //----------------------------------------------------------------------------------------------
        //GET: Message/GetMessageContent
        public ActionResult GetMessageContent(int MessageID)
        {
            SqlConnection con = new SqlConnection(_conStr);
            DataSet ds = new DataSet();
            //存取顯示"留言"內容相關的資料庫欄位
            SqlCommand cmdGetMessage = con.CreateCommand();
            cmdGetMessage.CommandType = CommandType.StoredProcedure;
            cmdGetMessage.CommandText = "dbo.usp_Message_GetMessage";
            cmdGetMessage.Parameters.Add("@MessageID", SqlDbType.Int);
            cmdGetMessage.Parameters["@MessageID"].Value = MessageID;

            SqlDataAdapter adpGetMessage = new SqlDataAdapter(cmdGetMessage);
            adpGetMessage.Fill(ds, "Message");

            //存取顯示"回覆"訊息相關的資料庫欄位
            SqlCommand cmdGetReply = con.CreateCommand();
            cmdGetReply.CommandType = CommandType.StoredProcedure;
            cmdGetReply.CommandText = "dbo.usp_Reply_Get";
            cmdGetReply.Parameters.Add("@MessageID", SqlDbType.Int);
            cmdGetReply.Parameters["@MessageID"].Value = MessageID;

            SqlDataAdapter adpGetReply = new SqlDataAdapter(cmdGetReply);
            adpGetReply.Fill(ds, "Reply");

            if (Session["MemberID"] == null)
                return View("GetMessageContent", "_LayoutLogout", ds);
            else
                return View("GetMessageContent", "_LayoutLogin", ds);
        }
        //----------------------------------------------------------------------------------------------
        // GET: Message/SearchMessage
        public ActionResult SearchMessage(string keyword)
        {
            //搜尋留言
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(_conStr);
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.usp_Message_Search";
            cmd.Parameters.Add("@keyword", SqlDbType.NVarChar);
            cmd.Parameters["@keyword"].Value = '%' + keyword + '%';

            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(dt);

            if (dt.Rows.Count == 0)
                ViewBag.Alert = "沒有與關鍵字 「" + keyword + "」相關的標題";

            if (Session["MemberID"] == null)
                return View("GetMessageList", "_LayoutLogout", dt);
            else
                return View("GetMessageList", "_LayoutLogin", dt);
        }
        //-------------------------------------------------------------------------------------------------
        //GET: Message/UpdateMessage
        public ActionResult UpdateMessage(int MessageID)
        {
            if (Session["MemberID"] == null)
                return RedirectToAction("GetMessageList", "Message");

            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(_conStr);
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.usp_Message_Get";
            cmd.Parameters.Add("@MessageID", SqlDbType.Int);
            cmd.Parameters["@MessageID"].Value = MessageID;

            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(dt);

            return View("UpdateMessage","_LayoutLogin", dt);
        }
        //---------------------------------------------------------------------------------------------------
        //POST: Message/UpdateMessage
        [HttpPost]
        public ActionResult UpdateMessage(int MessageID, int MemberID, string NewTitle, string NewContent)
        {
            SqlConnection con = new SqlConnection(_conStr);
            con.Open();
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.usp_Message_Update";

            cmd.Parameters.Add("@NewTitle", SqlDbType.NVarChar);
            cmd.Parameters["@NewTitle"].Value = NewTitle;
            cmd.Parameters.Add("@NewContent", SqlDbType.NVarChar);
            cmd.Parameters["@NewContent"].Value = NewContent;
            cmd.Parameters.Add("@MessageID", SqlDbType.Int);
            cmd.Parameters["@MessageID"].Value = MessageID;
            cmd.Parameters.Add("@MemberID", SqlDbType.Int);
            cmd.Parameters["@MemberID"].Value = MemberID;

            cmd.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("GetMessageContent","Message", new { MessageID = MessageID });
        }
        //---------------------------------------------------------------------------------------------------
        //POST: Message/DeleteMessage
        [HttpPost]
        public ActionResult DeleteMessage(int MessageID)
        {
            SqlConnection con = new SqlConnection(_conStr);
            con.Open();

            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.usp_Message_Delete";
            cmd.Parameters.Add("@MessageID", SqlDbType.Int);
            cmd.Parameters["@MessageID"].Value = MessageID;

            cmd.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("GetMessageList");
        }
    }
}