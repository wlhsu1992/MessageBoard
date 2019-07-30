using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Data.SqlClient;

namespace prjMessageBoard_v2.Controllers
{
    public class ReplyController : Controller
    {
        string _conStr = @"Server = .\SQLEXPRESS; Database = dbMessageBoard; Integrated Security = true;";

        //----------------------------------------------------------------------------------------------
        //POST: Reply/CreateReply
        [HttpPost]
        public ActionResult CreateReply(int MessageID, int MemberID, string ReplyContent)
        {
            //接收留言回復並新增的地方
            SqlConnection con = new SqlConnection(_conStr);
            con.Open();
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.usp_Reply_Add";

            cmd.Parameters.Add("@MessageID", SqlDbType.Int);
            cmd.Parameters["@MessageID"].Value = MessageID;
            cmd.Parameters.Add("@MemberID", SqlDbType.Int);
            cmd.Parameters["@MemberID"].Value = MemberID;
            cmd.Parameters.Add("@ReplyContent", SqlDbType.NVarChar);
            cmd.Parameters["@ReplyContent"].Value = ReplyContent;

            cmd.ExecuteNonQuery();
            con.Close();

            return RedirectToAction("GetMessageContent","Message", new { MessageID = MessageID });
        }
        //----------------------------------------------------------------------------------------------------
        //GET: Reply/UpdateReply
        public ActionResult UpdateReply(int ReplyID)
        {
            if (Session["MemberID"] == null)
                return RedirectToAction("GetMessageList", "Message");

            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(_conStr);
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.usp_Reply_GetReply";

            cmd.Parameters.Add("@ReplyID", SqlDbType.Int);
            cmd.Parameters["@ReplyID"].Value = ReplyID;

            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(dt);

            return View("UpdateReply", "_LayoutLogin", dt);
        }
        //POST: Reply/UpdateReply
        [HttpPost]
        public ActionResult UpdateReply(int MessageID, int ReplyID, int MemberID, string NewContent)
        {
            SqlConnection con = new SqlConnection(_conStr);
            con.Open();

            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.usp_Reply_Update";
            cmd.Parameters.Add("@ReplyID", SqlDbType.Int);
            cmd.Parameters["@ReplyID"].Value = ReplyID;
            cmd.Parameters.Add("@MemberID", SqlDbType.Int);
            cmd.Parameters["@MemberID"].Value = MemberID;
            cmd.Parameters.Add("@NewContent", SqlDbType.NVarChar);
            cmd.Parameters["@NewContent"].Value = NewContent;

            cmd.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("GetMessageContent","Message", new { MessageID = MessageID });
        }
        //----------------------------------------------------------------------------------------------------
        //POST: Reply/DeleteReply
        [HttpPost]
        public ActionResult DeleteReply(int MessageID, int ReplyID)
        {
            SqlConnection con = new SqlConnection(_conStr);
            con.Open();

            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.usp_Reply_Delete";
            cmd.Parameters.Add("@ReplyID", SqlDbType.Int);
            cmd.Parameters["@ReplyID"].Value = ReplyID;

            cmd.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("GetMessageContent","Message", new { MessageID = MessageID });
        }
    }
}