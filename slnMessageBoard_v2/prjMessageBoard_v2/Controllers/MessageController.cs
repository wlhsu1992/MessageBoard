using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Data.SqlClient;
using prjMessageBoard_v2.Models;

namespace prjMessageBoard_v2.Controllers
{
    public class MessageController : Controller
    {
        //連接至MSSQL EXPRESS資料庫連接字串
        string _conStr =  System.Web.Configuration.WebConfigurationManager.ConnectionStrings["dbMessageBoard"].ConnectionString;

        //----------------------------------------------------------------------------------------------
        // GET: Message/CreateMessage
        public ActionResult CreateMessage()
        {
            //此為會員限定頁面，若非會員進入此頁面將頁面導回主頁
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
            //此為會員限定頁面，若非會員進入此頁面將頁面導回主頁
            if (Session["MemberID"] == null)
                return RedirectToAction("GetMessageList", "Message");

            //使用ADO.NET在Message資料表新增留言記錄 [Title,Content,CreateTime]
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "dbo.usp_Message_Add";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Title", SqlDbType.NVarChar);
                cmd.Parameters["@Title"].Value = Title;
                cmd.Parameters.Add("@Content", SqlDbType.NVarChar);
                cmd.Parameters["@Content"].Value = Content;
                cmd.Parameters.Add("@MemberID", SqlDbType.Int);
                cmd.Parameters["@MemberID"].Value = Session["MemberID"];

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("GetMessageList","Message");
        }
        //----------------------------------------------------------------------------------------------
        // GET: Message/GetMessageList
        public ActionResult GetMessageList()
        {
            //建立模型物件，使其與資料庫撈取資料繫結
            MessageBoardModel dbModel = new MessageBoardModel();
            dbModel.message = new List<Message>();
            dbModel.member = new List<Member>();

            //Message,Member資料表取得留言列表記錄
            //留言列表 (留言編號、留言標題、留言時間、留言者帳號、留言者名稱)
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_message_GetList";
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Message message = new Message
                    {
                        ID = reader.GetInt32(reader.GetOrdinal("ID")),
                        Title = reader.GetString(reader.GetOrdinal("Title")),
                        CreateTime = reader.GetDateTime(reader.GetOrdinal("CreateTime"))
                    };
                    dbModel.message.Add(message);
                    Member member = new Member
                    {
                        Account = reader.GetString(reader.GetOrdinal("Account")),
                        Name = reader.GetString(reader.GetOrdinal("Name"))
                    };
                    dbModel.member.Add(member);
                }

                if (dbModel.message.Count == 0)
                    ViewBag.Alert = "目前尚無留言";
                else
                {
                    dbModel.message.Reverse();
                    dbModel.member.Reverse();
                }

                //判斷使用者是否有登入會員，分別將其導向套用不同的背板
                if (Session["MemberID"] == null)
                    return View("GetMessageList", "_LayoutLogout", dbModel);
                else
                    return View("GetMessageList", "_LayoutLogin", dbModel);
            }
        }
        //----------------------------------------------------------------------------------------------
        //GET: Message/GetMessageContent
        public ActionResult GetMessageContent(int MessageID)
        {

            MessageBoardModel dbModel = new MessageBoardModel();
            dbModel.member = new List<Member>();
            dbModel.message = new List<Message>();
            dbModel.reply = new List<Reply>();

            //存取顯示"留言"內容相關的資料庫欄位
            //由List<Member>、<Message>、<Reply> index = 0的值表示
            //留言內容 (留言編號、留言標題、留言內容、留言時間、留言者編號、留言者名稱、留言者帳號)
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_Message_GetContent";
                cmd.Parameters.Add("@MessageID", SqlDbType.Int);
                cmd.Parameters["@MessageID"].Value = MessageID;

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Member member = new Member
                    {
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Account = reader.GetString(reader.GetOrdinal("Account"))
                    };
                    dbModel.member.Add(member);
                    Message message = new Message
                    {
                        ID = reader.GetInt32(reader.GetOrdinal("ID")),
                        Title = reader.GetString(reader.GetOrdinal("Title")),
                        Content = reader.GetString(reader.GetOrdinal("Content")),
                        CreateTime = reader.GetDateTime(reader.GetOrdinal("CreateTime")),
                        MemberID = reader.GetInt32(reader.GetOrdinal("MemberID"))
                    };
                    dbModel.message.Add(message);
                    Reply reply = new Reply();
                    dbModel.reply.Add(reply);
                }
            }

            //存取顯示"回覆"訊息相關的資料庫欄位
            //由List<Member>、<Message>、<Reply> index > 0的值表示
            //回覆內容 (回覆編號、回覆內容、回覆時間、回覆者編號、回覆者名稱、回覆者帳號)
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_Reply_GetContent";
                cmd.Parameters.Add("@MessageID", SqlDbType.Int);
                cmd.Parameters["@MessageID"].Value = MessageID;

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Member member = new Member
                    {
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Account = reader.GetString(reader.GetOrdinal("Account"))
                    };
                    dbModel.member.Add(member);
                    Message message = new Message();
                    dbModel.message.Add(message);
                    Reply reply = new Reply
                    {
                        ID = reader.GetInt32(reader.GetOrdinal("ID")),
                        ReplyContent = reader.GetString(reader.GetOrdinal("ReplyContent")),
                        ReplyTime = reader.GetDateTime(reader.GetOrdinal("ReplyTime")),
                        MemberID = reader.GetInt32(reader.GetOrdinal("MemberID"))
                    };
                    dbModel.reply.Add(reply);
                }
            }

            if (Session["MemberID"] == null)
                return View("GetMessageContent", "_LayoutLogout", dbModel);
            else
                return View("GetMessageContent", "_LayoutLogin", dbModel);
        }
        //----------------------------------------------------------------------------------------------
        // GET: Message/SearchMessage
        public ActionResult SearchMessage(string keyword)
        {
            MessageBoardModel dbModel = new MessageBoardModel();
            dbModel.message = new List<Message>();
            dbModel.member = new List<Member>();

            //由資料庫取得 留言標題符合 keyword 關鍵字的 留言列表記錄
            //留言列表 (留言編號、留言標題、留言時間、留言者帳號、留言者名稱)
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_Message_GetTitle";
                cmd.Parameters.Add("@keyword", SqlDbType.NVarChar);
                cmd.Parameters["@keyword"].Value = $"%{keyword}%";

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Message message = new Message
                    {
                        ID = reader.GetInt32(reader.GetOrdinal("ID")),
                        Title = reader.GetString(reader.GetOrdinal("Title")),
                        CreateTime = reader.GetDateTime(reader.GetOrdinal("CreateTime"))
                    };
                    dbModel.message.Add(message);
                    Member member = new Member
                    {
                        Account = reader.GetString(reader.GetOrdinal("Account")),
                        Name = reader.GetString(reader.GetOrdinal("Name"))
                    };
                    dbModel.member.Add(member);
                }

                if (dbModel.message.Count == 0)
                    ViewBag.Alert = $"沒有與關鍵字「{keyword}」相關的標題";
                else
                { 
                    dbModel.message.Reverse();
                    dbModel.member.Reverse();
                }
                //判斷使用者是否有登入會員，分別將其導向套用不同的背板
                if (Session["MemberID"] == null)
                    return View("GetMessageList", "_LayoutLogout", dbModel);
                else
                    return View("GetMessageList", "_LayoutLogin", dbModel);
            }
        }
        //-------------------------------------------------------------------------------------------------
        //GET: Message/UpdateMessage
        public ActionResult UpdateMessage(int MessageID)
        {
            //此為會員限定頁面，若非會員進入此頁面將頁面導回主頁
            if (Session["MemberID"] == null)
                return RedirectToAction("GetMessageList", "Message");

            MessageBoardModel dbModel = new MessageBoardModel();
            dbModel.message = new List<Message>();

            //由Message資料表取得 指定留言編號的 留言內容
            //留言內容 (留言編號、留言標題、留言時間)
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_Message_GetMessage";
                cmd.Parameters.Add("@MessageID", SqlDbType.Int);
                cmd.Parameters["@MessageID"].Value = MessageID;

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Message message = new Message
                    {
                        ID = reader.GetInt32(reader.GetOrdinal("ID")),
                        Title = reader.GetString(reader.GetOrdinal("Title")),
                        Content = reader.GetString(reader.GetOrdinal("Content")),
                        MemberID = reader.GetInt32(reader.GetOrdinal("MemberID"))
                    };
                    dbModel.message.Add(message);
                }
            }
            return View("UpdateMessage", "_LayoutLogin", dbModel);
        }
        //---------------------------------------------------------------------------------------------------
        //POST: Message/UpdateMessage
        [HttpPost]
        public ActionResult UpdateMessage(int MessageID, int MemberID, string NewTitle, string NewContent)
        {
            //此為會員限定頁面，若非會員進入此頁面將頁面導回主頁
            if (Session["MemberID"] == null)
                return RedirectToAction("GetMessageList", "Message");

            //將表單輸入的留言內容，更新回指定ID的message資料表
            //更新欄位 (留言編號、留言標題、留言時間)
            using (SqlConnection con = new SqlConnection(_conStr))
            {
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

                con.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("GetMessageContent","Message", new { MessageID = MessageID });
        }
        //---------------------------------------------------------------------------------------------------
        //POST: Message/DeleteMessage
        [HttpPost]
        public ActionResult DeleteMessage(int MessageID)
        {
            //此為會員限定頁面，若非會員進入此頁面將頁面導回主頁
            if (Session["MemberID"] == null)
                return RedirectToAction("GetMessageList", "Message");

            //於message資料表中，刪除指定ID的記錄
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_Message_Delete";
                cmd.Parameters.Add("@MessageID", SqlDbType.Int);
                cmd.Parameters["@MessageID"].Value = MessageID;

                con.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("GetMessageList");
        }
    }
}