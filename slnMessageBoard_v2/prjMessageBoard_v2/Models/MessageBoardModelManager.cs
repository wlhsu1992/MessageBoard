using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;
using System.Data.SqlClient;

namespace prjMessageBoard_v2.Models
{
    /// <summary>
    /// 操作MessageBoard相關資料庫的行為
    /// </summary>
    public class MessageBoardModelManager
    {
        private static string _conStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["dbMessageBoard"].ConnectionString;
        /// <summary>
        /// 判斷資料庫中是否有與傳入參數相同的帳號account，
        /// 若有傳回true，否則false
        /// </summary>
        /// <param name="account">使用者輸入註冊帳號</param>
        /// <returns></returns>
        public static bool GetAccount(string account)
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "dbo.usp_Member_GetAccount";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Account", SqlDbType.VarChar);
                cmd.Parameters["@Account"].Value = account;

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                return reader.HasRows;
            }
        }

        /// <summary>
        /// 新增使用者填寫的會員註冊資料至Member資料表。
        /// </summary>
        /// <param name="account">會員註冊帳號</param>
        /// <param name="password">會員註冊密碼</param>
        /// <param name="name">會員註冊名稱</param>
        public static void CreateMember(string account, string password, string name)
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmdCreate = con.CreateCommand();
                cmdCreate.CommandText = "dbo.usp_Member_Add";
                cmdCreate.CommandType = CommandType.StoredProcedure;

                cmdCreate.Parameters.Add("@Account", SqlDbType.VarChar);
                cmdCreate.Parameters["@Account"].Value = account;
                cmdCreate.Parameters.Add("@Password", SqlDbType.VarChar);
                cmdCreate.Parameters["@Password"].Value = password;
                cmdCreate.Parameters.Add("@Name", SqlDbType.NVarChar);
                cmdCreate.Parameters["@Name"].Value = name;

                con.Open();
                cmdCreate.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 取得並傳資料庫中與會員登入帳號account、密碼password相同的
        /// 會員登入資料(會員編號、帳號、姓名)
        /// </summary>
        /// <param name="account">會員登入帳號</param>
        /// <param name="password">會員登入密碼</param>
        /// <returns></returns>
        public static LoginMemberModel CheckLogin(string account, string password)
        {
            LoginMemberModel memberData = new LoginMemberModel();

            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "dbo.usp_Member_GetAccountAndPassword";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Account", SqlDbType.VarChar);
                cmd.Parameters.Add("@Password", SqlDbType.VarChar);
                cmd.Parameters["@Account"].Value = account;
                cmd.Parameters["@Password"].Value = password;

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    memberData.MemberID = reader.GetInt32(reader.GetOrdinal("ID"));
                    memberData.Account = reader.GetString(reader.GetOrdinal("Account"));
                    memberData.Name = reader.GetString(reader.GetOrdinal("Name"));
                }
                return memberData;
            }
        }

        /// <summary>
        /// 取得留言板列表所需欄位，列表順序依照留言建立時間降序排序
        /// </summary>
        /// <returns></returns>
        public static List<MessageBoardViewlModel.Discussion> GetDiscussionList()
        {
            List<MessageBoardViewlModel.Discussion> messageTitleList = new List<MessageBoardViewlModel.Discussion>();

            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_message_GetList";
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    MessageBoardViewlModel.Discussion messageTitle = new MessageBoardViewlModel.Discussion();
                    messageTitle.MessageID = reader.GetInt32(reader.GetOrdinal("ID"));
                    messageTitle.Title = reader.GetString(reader.GetOrdinal("Title"));
                    messageTitle.CreateTime = reader.GetDateTime(reader.GetOrdinal("CreateTime"));
                    messageTitle.Account = reader.GetString(reader.GetOrdinal("Account"));
                    messageTitle.Name = reader.GetString(reader.GetOrdinal("Name"));
                    messageTitleList.Add(messageTitle);
                    messageTitle = null;
                }
                messageTitleList = messageTitleList.OrderByDescending(m => m.CreateTime).ToList();
            }
            return messageTitleList;
        }

        /// <summary>
        /// 取得指定留言編號(MessageID)的留言、回覆內容記錄，
        /// </summary>
        /// <param name="messageID">欲取得留言內容的留言編號</param>
        /// <returns></returns>
        public static MessageBoardViewlModel.DiscussionContent GetDiscussionContent(int messageID)
        {
            List<MessageBoardViewlModel.DiscussionReply> replyList = new List<MessageBoardViewlModel.DiscussionReply>();

            //存取顯示"回覆"訊息相關的資料庫欄位
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_Reply_GetContent";
                cmd.Parameters.Add("@MessageID", SqlDbType.Int);
                cmd.Parameters["@MessageID"].Value = messageID;

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    MessageBoardViewlModel.DiscussionReply reply = new MessageBoardViewlModel.DiscussionReply();
                    reply.Name = reader.GetString(reader.GetOrdinal("Name"));
                    reply.Account = reader.GetString(reader.GetOrdinal("Account"));
                    reply.MemberID = reader.GetInt32(reader.GetOrdinal("MemberID"));
                    reply.ReplyID = reader.GetInt32(reader.GetOrdinal("ID"));
                    reply.ReplyContent = reader.GetString(reader.GetOrdinal("ReplyContent"));
                    reply.ReplyTime = reader.GetDateTime(reader.GetOrdinal("ReplyTime"));
                    replyList.Add(reply);
                    reply = null;
                }
            }

            MessageBoardViewlModel.DiscussionContent message = new MessageBoardViewlModel.DiscussionContent();

            //存取顯示"留言"內容相關的資料庫欄位
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_Message_GetContent";
                cmd.Parameters.Add("@MessageID", SqlDbType.Int);
                cmd.Parameters["@MessageID"].Value = messageID;

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    message.Name = reader.GetString(reader.GetOrdinal("Name"));
                    message.Account = reader.GetString(reader.GetOrdinal("Account"));
                    message.MemberID = reader.GetInt32(reader.GetOrdinal("MemberID"));
                    message.MessageID = reader.GetInt32(reader.GetOrdinal("ID"));
                    message.Title = reader.GetString(reader.GetOrdinal("Title"));
                    message.Content = reader.GetString(reader.GetOrdinal("Content"));
                    message.CreateTime = reader.GetDateTime(reader.GetOrdinal("CreateTime"));
                    message.PhotoID = reader.GetString(reader.GetOrdinal("PhotoID"));
                    message.ReplyList = replyList;
                }
            }
            return message;
        }

        /// <summary>
        /// 取得符合傳入參數關鍵字(keyword)標題的留言列表記錄，
        /// 並將資料以留言時間降序排序。
        /// </summary>
        /// <param name="keyword">搜尋留言標題關鍵字</param>
        /// <returns></returns>
        public static List<MessageBoardViewlModel.Discussion> SearchTitle(string keyword)
        {
            List<MessageBoardViewlModel.Discussion> messageTitleList = new List<MessageBoardViewlModel.Discussion>();

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
                    MessageBoardViewlModel.Discussion messageTitle = new MessageBoardViewlModel.Discussion();
                    messageTitle.MessageID = reader.GetInt32(reader.GetOrdinal("ID"));
                    messageTitle.Title = reader.GetString(reader.GetOrdinal("Title"));
                    messageTitle.CreateTime = reader.GetDateTime(reader.GetOrdinal("CreateTime"));
                    messageTitle.Account = reader.GetString(reader.GetOrdinal("Account"));
                    messageTitle.Name = reader.GetString(reader.GetOrdinal("Name"));
                    messageTitleList.Add(messageTitle);
                    messageTitle = null;
                }
            }
            messageTitleList = messageTitleList.OrderByDescending(m => m.CreateTime).ToList();

            return messageTitleList;
        }

        /// <summary>
        /// 將輸入留言參數標題(title)、內容(content)、圖片儲存名稱(photoID)，新增至留言記錄
        /// </summary>
        /// <param name="title">欲新增留言標題</param>
        /// <param name="content">欲新增留言內容</param>
        /// <param name="photoID">欲新增圖片檔案編碼</param>
        public static void CreateMessage(string title, string content, string photoID)
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "dbo.usp_Message_Add";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Title", SqlDbType.NVarChar);
                cmd.Parameters["@Title"].Value = title;
                cmd.Parameters.Add("@Content", SqlDbType.NVarChar);
                cmd.Parameters["@Content"].Value = content;
                cmd.Parameters.Add("@MemberID", SqlDbType.Int);
                cmd.Parameters["@MemberID"].Value = SessionManager.MemberID;
                cmd.Parameters.Add("@PhotoID", SqlDbType.VarChar);
                cmd.Parameters["@PhotoID"].Value = photoID;

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 取得指定留言編號(messageID)的圖片檔案欄位資料
        /// </summary>
        /// <param name="messageID">欲取得資料欄位的留言編號</param>
        /// <returns></returns>
        public static string GetPhotoID(int messageID)
        {
            string photoID;

            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_Message_GetPhotoID";
                cmd.Parameters.Add("@MessageID", SqlDbType.Int);
                cmd.Parameters["@MessageID"].Value = messageID;

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                photoID = reader.GetString(reader.GetOrdinal("PhotoID"));
            }

            return photoID;
        }

        /// <summary>
        /// 取得指定留言編號(messageID)的留言內容
        /// </summary>
        /// <param name="messageID">欲取得留言內容的留言編號</param>
        /// <returns></returns>
        public static MessageBoardViewlModel.MessageEditor GetMessage(int messageID)
        {
            MessageBoardViewlModel.MessageEditor message = new MessageBoardViewlModel.MessageEditor();
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_Message_GetMessage";
                cmd.Parameters.Add("@MessageID", SqlDbType.Int);
                cmd.Parameters["@MessageID"].Value = messageID;

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    message.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                    message.Title = reader.GetString(reader.GetOrdinal("Title"));
                    message.Content = reader.GetString(reader.GetOrdinal("Content"));
                    message.MemberID = reader.GetInt32(reader.GetOrdinal("MemberID"));
                }
            }
            return message;
        }

        /// <summary>
        /// 由表單取得修改留言內容，修改至指定留言編號記錄(messageID)。
        /// </summary>
        /// <param name="messageID">欲修改的留言編號</param>
        /// <param name="newTitle">修改後留言標題</param>
        /// <param name="newContent">修改後留言內容</param>
        public static void UpdateMessage(int messageID, string newTitle, string newContent)
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_Message_Update";

                cmd.Parameters.Add("@NewTitle", SqlDbType.NVarChar);
                cmd.Parameters["@NewTitle"].Value = newTitle;
                cmd.Parameters.Add("@NewContent", SqlDbType.NVarChar);
                cmd.Parameters["@NewContent"].Value = newContent;
                cmd.Parameters.Add("@MessageID", SqlDbType.Int);
                cmd.Parameters["@MessageID"].Value = messageID;

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 刪除指定留言編號(messagID)記錄及於留言串列下所有回覆內容
        /// </summary>
        /// <param name="messageID">欲刪除的留言編號</param>
        public static void DeleteMessage(int messageID)
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_Message_Delete";
                cmd.Parameters.Add("@MessageID", SqlDbType.Int);
                cmd.Parameters["@MessageID"].Value = messageID;

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 由輸入參數於資料表新增一筆留言回覆記錄。
        /// </summary>
        /// <param name="messageID">回覆所在的留言編號</param>
        /// <param name="memberID">回覆者的會員編號</param>
        /// <param name="replyContent">回覆內容</param>
        public static void CreateReply(int messageID, int memberID, string replyContent)
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_Reply_Add";

                cmd.Parameters.Add("@MessageID", SqlDbType.Int);
                cmd.Parameters["@MessageID"].Value = messageID;
                cmd.Parameters.Add("@MemberID", SqlDbType.Int);
                cmd.Parameters["@MemberID"].Value = memberID;
                cmd.Parameters.Add("@ReplyContent", SqlDbType.NVarChar);
                cmd.Parameters["@ReplyContent"].Value = replyContent;

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="replyID">欲取得記錄的回覆編號</param>
        /// <returns></returns>
        public static MessageBoardViewlModel.ReplyEditor GetReply(int replyID)
        {
            MessageBoardViewlModel.ReplyEditor reply = new MessageBoardViewlModel.ReplyEditor();

            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_Reply_GetReply";

                cmd.Parameters.Add("@ReplyID", SqlDbType.Int);
                cmd.Parameters["@ReplyID"].Value = replyID;

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    reply.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                    reply.MessageID = reader.GetInt32(reader.GetOrdinal("MessageID"));
                    reply.MemberID = reader.GetInt32(reader.GetOrdinal("MemberID"));
                    reply.ReplyContent = reader.GetString(reader.GetOrdinal("ReplyContent"));
                }
            }
            return reply;
        }

        /// <summary>
        /// 由表單取得修改回覆內容，修改至指定回覆編號記錄。
        /// </summary>
        /// <param name="replyID">欲修改記錄的回覆編號</param>
        /// <param name="newContent">修改後的回覆內容</param>
        public static void UpdateReply(int replyID, string newContent)
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_Reply_Update";

                cmd.Parameters.Add("@ReplyID", SqlDbType.Int);
                cmd.Parameters["@ReplyID"].Value = replyID;
                cmd.Parameters.Add("@NewContent", SqlDbType.NVarChar);
                cmd.Parameters["@NewContent"].Value = newContent;

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 由Reply資料表，刪除指定回覆編號MemberID的記錄。
        /// </summary>
        /// <param name="replyID">欲刪除回覆記錄的編號</param>
        public static void DeleteReply(int replyID)
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_Reply_Delete";
                cmd.Parameters.Add("@ReplyID", SqlDbType.Int);
                cmd.Parameters["@ReplyID"].Value = replyID;

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}