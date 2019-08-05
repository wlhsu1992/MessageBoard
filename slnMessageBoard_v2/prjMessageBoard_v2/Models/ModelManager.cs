using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;
using System.Data.SqlClient;

namespace prjMessageBoard_v2.Models
{
    public class ModelManager
    {
        static string _conStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["dbMessageBoard"].ConnectionString;
        /// <summary>
        /// 於Member資料表取得指定使用者帳號Account的紀錄，並以DataTable型別傳回資料。
        /// </summary>
        /// <param name="Account">使用者輸入註冊帳號</param>
        /// <returns></returns>
        static public DataTable GetAccount(string Account)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "dbo.usp_Member_GetAccount";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Account", SqlDbType.VarChar);
                cmd.Parameters["@Account"].Value = Account;
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
            }
            return dt;
        }

        /// <summary>
        /// 新增使用者填寫的會員註冊紀錄至Member資料表。
        /// </summary>
        /// <param name="Account">會員註冊帳號</param>
        /// <param name="Password">會員註冊密碼</param>
        /// <param name="Name">會員註冊名稱</param>
        static public void CreateMember(string Account, string Password, string Name)
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmdCreate = con.CreateCommand();
                cmdCreate.CommandText = "dbo.usp_Member_Add";
                cmdCreate.CommandType = CommandType.StoredProcedure;

                cmdCreate.Parameters.Add("@Account", SqlDbType.VarChar);
                cmdCreate.Parameters["@Account"].Value = Account;
                cmdCreate.Parameters.Add("@Password", SqlDbType.VarChar);
                cmdCreate.Parameters["@Password"].Value = Password;
                cmdCreate.Parameters.Add("@Name", SqlDbType.NVarChar);
                cmdCreate.Parameters["@Name"].Value = Name;

                con.Open();
                cmdCreate.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 由Member資料表取得指定帳號Account及密碼Password的紀錄，
        /// 並將紀錄以DataTable型別傳回。
        /// </summary>
        /// <param name="Account">會員登入帳號</param>
        /// <param name="Password">會員登入密碼</param>
        /// <returns></returns>
        static public DataTable GetAccountAndPassword(string Account, string Password)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmdCheck = con.CreateCommand();
                cmdCheck.CommandText = "dbo.usp_Member_GetAccountAndPassword";
                cmdCheck.CommandType = CommandType.StoredProcedure;

                cmdCheck.Parameters.Add("@Account", SqlDbType.VarChar);
                cmdCheck.Parameters.Add("@Password", SqlDbType.VarChar);
                cmdCheck.Parameters["@Account"].Value = Account;
                cmdCheck.Parameters["@Password"].Value = Password;

                SqlDataAdapter adp = new SqlDataAdapter(cmdCheck);
                adp.Fill(dt);
            }
            return dt;
        }

        /// <summary>
        /// 新增留言紀錄於Message資料表
        /// </summary>
        /// <param name="Title">新增留言標題</param>
        /// <param name="Content">新增留言內容</param>
        static public void CreateMessage(string Title, string Content)
        {
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
                cmd.Parameters["@MemberID"].Value = SessionManager.MemberID;

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 取得留言板列表所需欄位，
        /// 並將取得資料以自訂MessageBoardModel型別傳回。
        /// </summary>
        /// <returns></returns>
        static public MessageBoardModel GetList()
        {
            MessageBoardModel dbModel = new MessageBoardModel();
            dbModel.Message = new List<Message>();
            dbModel.Member = new List<Member>();

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
                    dbModel.Message.Add(message);
                    Member member = new Member
                    {
                        Account = reader.GetString(reader.GetOrdinal("Account")),
                        Name = reader.GetString(reader.GetOrdinal("Name"))
                    };
                    dbModel.Member.Add(member);
                }
            }
            return dbModel;
        }

        /// <summary>
        /// 取得指定留言編號MessageID的留言、回覆內容紀錄，
        /// 並將取得資料以自訂MessageBoardModel型別傳回。
        /// </summary>
        /// <param name="MessageID">欲取得留言內容的留言編號</param>
        /// <returns></returns>
        static public MessageBoardModel GetMessageContent(int MessageID)
        {

            MessageBoardModel dbModel = new MessageBoardModel();
            dbModel.Member = new List<Member>();
            dbModel.Message = new List<Message>();
            dbModel.Reply = new List<Reply>();

            //存取顯示"留言"內容相關的資料庫欄位
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
                    dbModel.Member.Add(member);
                    Message message = new Message
                    {
                        ID = reader.GetInt32(reader.GetOrdinal("ID")),
                        Title = reader.GetString(reader.GetOrdinal("Title")),
                        Content = reader.GetString(reader.GetOrdinal("Content")),
                        CreateTime = reader.GetDateTime(reader.GetOrdinal("CreateTime")),
                        MemberID = reader.GetInt32(reader.GetOrdinal("MemberID"))
                    };
                    dbModel.Message.Add(message);
                    Reply reply = new Reply();
                    dbModel.Reply.Add(reply);
                }
            }

            //存取顯示"回覆"訊息相關的資料庫欄位
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
                    dbModel.Member.Add(member);
                    Message message = new Message();
                    dbModel.Message.Add(message);
                    Reply reply = new Reply
                    {
                        ID = reader.GetInt32(reader.GetOrdinal("ID")),
                        ReplyContent = reader.GetString(reader.GetOrdinal("ReplyContent")),
                        ReplyTime = reader.GetDateTime(reader.GetOrdinal("ReplyTime")),
                        MemberID = reader.GetInt32(reader.GetOrdinal("MemberID"))
                    };
                    dbModel.Reply.Add(reply);
                }
            }
            return dbModel;
        }

        /// <summary>
        /// 使用關鍵字keyword尋找包含此關鍵字標題的留言紀錄，
        /// 並將取得資料以自訂MessageBoardModel型別傳回。
        /// </summary>
        /// <param name="keyword">搜尋留言標題關鍵字</param>
        /// <returns></returns>
        static public MessageBoardModel GetTitle(string keyword)
        {
            MessageBoardModel dbModel = new MessageBoardModel();
            dbModel.Message = new List<Message>();
            dbModel.Member = new List<Member>();

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
                    dbModel.Message.Add(message);
                    Member member = new Member
                    {
                        Account = reader.GetString(reader.GetOrdinal("Account")),
                        Name = reader.GetString(reader.GetOrdinal("Name"))
                    };
                    dbModel.Member.Add(member);
                }
            }
            return dbModel;
        }

        /// <summary>
        /// 取得指定留言編號的留言內容，
        /// 並將取得資料以自訂MessageBoardModel型別傳回。
        /// </summary>
        /// <param name="MessageID">欲取得留言內容的留言編號</param>
        /// <returns></returns>
        static public MessageBoardModel GetMessage(int MessageID)
        {
            MessageBoardModel dbModel = new MessageBoardModel();
            dbModel.Message = new List<Message>();

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
                    dbModel.Message.Add(message);
                }
            }
            return dbModel;
        }

        /// <summary>
        /// 由表單取得修改留言內容，修改至指定留言編號紀錄。
        /// </summary>
        /// <param name="MessageID">欲修改的留言編號</param>
        /// <param name="NewTitle">修改後留言標題</param>
        /// <param name="NewContent">修改後留言內容</param>
        static public void UpdateMessage(int MessageID, string NewTitle, string NewContent)
        {
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

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 根據指定留言編號MessageID，刪除指定留言紀錄及於留言串列下所有回覆內容
        /// </summary>
        /// <param name="MessageID">欲刪除的留言編號</param>
        static public void DeleteMessage(int MessageID)
        {
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
        }

        /// <summary>
        /// 於Reply資料表新增一筆留言回覆記錄。
        /// </summary>
        /// <param name="MessageID">回覆所在的留言編號</param>
        /// <param name="MemberID">回覆者的會員編號</param>
        /// <param name="ReplyContent">回覆內容</param>
        static public void CreateReply(int MessageID, int MemberID, string ReplyContent)
        {
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
        }

        /// <summary>
        /// 於Reply資料表，取得指定回覆編號ReplyID的回覆記錄，
        /// 並將取得資料以自訂MessageBoardModel型別傳回。
        /// </summary>
        /// <param name="ReplyID">欲取得紀錄的回覆編號</param>
        /// <returns></returns>
        static public MessageBoardModel GetReply(int ReplyID)
        {
            MessageBoardModel dbModel = new MessageBoardModel();
            dbModel.Reply = new List<Reply>();

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
                    dbModel.Reply.Add(reply);
                }
            }
            return dbModel;
        }

        /// <summary>
        /// 由表單取得修改回覆內容，修改至指定回覆編號紀錄。
        /// </summary>
        /// <param name="ReplyID">欲修改記錄的回覆編號</param>
        /// <param name="NewContent">修改後的回覆內容</param>
        static public void UpdateReply(int ReplyID, string NewContent)
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_Reply_Update";

                cmd.Parameters.Add("@ReplyID", SqlDbType.Int);
                cmd.Parameters["@ReplyID"].Value = ReplyID;
                cmd.Parameters.Add("@NewContent", SqlDbType.NVarChar);
                cmd.Parameters["@NewContent"].Value = NewContent;

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 由Reply資料表，刪除指定回覆編號MemberID的紀錄。
        /// </summary>
        /// <param name="ReplyID">欲刪除回覆記錄的編號</param>
        static public void DeleteReply(int ReplyID)
        {
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
        }
    }
}