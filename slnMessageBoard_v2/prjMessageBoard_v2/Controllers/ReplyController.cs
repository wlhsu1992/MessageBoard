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
    public class ReplyController : Controller
    {
         string _conStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["dbMessageBoard"].ConnectionString;

        //----------------------------------------------------------------------------------------------
        //POST: Reply/CreateReply
        [HttpPost]
        public ActionResult CreateReply(int MessageID, int MemberID, string ReplyContent)
        {
            //此為會員限定頁面，若非會員進入此頁面將頁面導回主頁
            if (Session["MemberID"] == null)
                return RedirectToAction("GetMessageList", "Message");

            //Reply資料表 新增 回覆紀錄
            //回覆紀錄　(留言編號、留言者編號、回覆內容)
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_Reply_Add";

                cmd.Parameters.Add("@MessageID", SqlDbType.Int);
                cmd.Parameters["@MessageID"].Value = MessageID;
                cmd.Parameters.Add("@MemberID", SqlDbType.Int);
                cmd.Parameters["@MemberID"].Value = MemberID;
                cmd.Parameters.Add("@ReplyContent", SqlDbType.NVarChar);
                cmd.Parameters["@ReplyContent"].Value = ReplyContent;

                con.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("GetMessageContent","Message", new { MessageID = MessageID });
        }
        //----------------------------------------------------------------------------------------------------
        //GET: Reply/UpdateReply
        public ActionResult UpdateReply(int ReplyID)
        {
            //此為會員限定頁面，若非會員進入此頁面將頁面導回主頁
            if (Session["MemberID"] == null)
                return RedirectToAction("GetMessageList", "Message");

            MessageBoardModel dbModel = new MessageBoardModel();
            dbModel.reply = new List<Reply>();

            //Reply資料表 取得 指定編號ReplyID的回覆內容
            //回覆內容　(回覆編號、回覆的留言串編號、會員編號、回覆內容)
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_Reply_GetReply";

                cmd.Parameters.Add("@ReplyID", SqlDbType.Int);
                cmd.Parameters["@ReplyID"].Value = ReplyID;

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Reply reply = new Reply
                    {
                        ID = reader.GetInt32(reader.GetOrdinal("ID")),
                        MessageID = reader.GetInt32(reader.GetOrdinal("MessageID")),
                        MemberID = reader.GetInt32(reader.GetOrdinal("MemberID")),
                        ReplyContent = reader.GetString(reader.GetOrdinal("ReplyContent")),
                    };
                    dbModel.reply.Add(reply);
                }
            }
            return View("UpdateReply", "_LayoutLogin", dbModel);
        }
        //-------------------------------------------------------------------------------------------
        //POST: Reply/UpdateReply
        [HttpPost]
        public ActionResult UpdateReply(int MessageID, int ReplyID, int MemberID, string NewContent)
        {
            //此為會員限定頁面，若非會員進入此頁面將頁面導回主頁
            if (Session["MemberID"] == null)
                return RedirectToAction("GetMessageList", "Message");

            //Reply資料表 修改 指定編號ReplyID的回覆內容
            //修改回覆欄位　(回覆內容)
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_Reply_Update";

                cmd.Parameters.Add("@ReplyID", SqlDbType.Int);
                cmd.Parameters["@ReplyID"].Value = ReplyID;
                cmd.Parameters.Add("@MemberID", SqlDbType.Int);
                cmd.Parameters["@MemberID"].Value = MemberID;
                cmd.Parameters.Add("@NewContent", SqlDbType.NVarChar);
                cmd.Parameters["@NewContent"].Value = NewContent;

                con.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("GetMessageContent","Message", new { MessageID = MessageID });
        }
        //----------------------------------------------------------------------------------------------------
        //POST: Reply/DeleteReply
        [HttpPost]
        public ActionResult DeleteReply(int MessageID, int ReplyID)
        {
            //此為會員限定頁面，若非會員進入此頁面將頁面導回主頁
            if (Session["MemberID"] == null)
                return RedirectToAction("GetMessageList", "Message");

            //Reply資料表 刪除 指定編號ReplyID的回覆記錄
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_Reply_Delete";
                cmd.Parameters.Add("@ReplyID", SqlDbType.Int);
                cmd.Parameters["@ReplyID"].Value = ReplyID;

                con.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("GetMessageContent","Message", new { MessageID = MessageID });
        }
    }
}