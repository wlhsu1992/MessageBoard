using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjMessageBoard_v2.Models
{
    /// <summary>
    /// 會員留言板資料模型繫結類別
    /// </summary>
    public class MessageBoardViewlModel
    {
        /// <summary>
        /// 留言列表資料欄位屬性
        /// </summary>
        public class Discussion
        {
            /// <summary>
            /// 留言編號
            /// </summary>
            public int MessageID { get; set; }
            /// <summary>
            /// 留言標題
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// 留言時間
            /// </summary>
            public DateTime CreateTime { get; set; }
            /// <summary>
            /// 留言者會員帳號
            /// </summary>
            public string Account { get; set; }
            /// <summary>
            /// 留言者會員名稱
            /// </summary>
            public string Name { get; set; }
        }

        /// <summary>
        /// 留言內容(主留言及所有回覆)的資料欄位屬性
        /// </summary>
        public class DiscussionContent
        {
            /// <summary>
            /// 留言者會員名稱
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 留言者會員帳號
            /// </summary>
            public string Account { get; set; }
            /// <summary>
            /// 留言者會員編號
            /// </summary>
            public int MemberID { get; set; }
            /// <summary>
            /// 留言編號
            /// </summary>
            public int MessageID { get; set; }
            /// <summary>
            /// 留言標題
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// 留言內容
            /// </summary>
            public string Content { get; set; }
            /// <summary>
            /// 留言時間
            /// </summary>
            public DateTime CreateTime { get; set; }
            /// <summary>
            /// 儲存圖片名稱
            /// </summary>
            public string PhotoID { get; set; }
            /// <summary>
            /// 留言回覆串列
            /// </summary>
            public List<DiscussionReply> ReplyList { get; set; }
        }

        /// <summary>
        /// 留言回覆內容的資料欄位屬性
        /// </summary>
        public class DiscussionReply
        {
            /// <summary>
            /// 回覆者會員名稱
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 回覆者會員帳號
            /// </summary>
            public string Account { get; set; }
            /// <summary>
            /// 回覆者會員編號
            /// </summary>
            public int MemberID { get; set; }
            /// <summary>
            /// 回覆編號
            /// </summary>
            public int ReplyID { get; set; }
            /// <summary>
            /// 回覆內容
            /// </summary>
            public string ReplyContent { get; set; }
            /// <summary>
            /// 回覆時間
            /// </summary>
            public DateTime ReplyTime { get; set; }
        }

        /// <summary>
        /// 修改留言資料欄位屬性
        /// </summary>
        public class MessageEditor
        {
            /// <summary>
            /// 留言編號
            /// </summary>
            public int ID { get; set; }
            /// <summary>
            /// 留言者會員編號
            /// </summary>
            public int MemberID { get; set; }
            /// <summary>
            /// 留言標題
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// 留言內容
            /// </summary>
            public string Content { get; set; }
            /// <summary>
            /// 建立留言時間
            /// </summary>
            public DateTime CreateTime { get; set; }
        }

        /// <summary>
        /// 修改回覆資料欄位屬性
        /// </summary>
        public class ReplyEditor
        {
            /// <summary>
            /// 回覆留言編號
            /// </summary>
            public int ID { get; set; }
            /// <summary>
            /// 回覆所在的留言串編號
            /// </summary>
            public int MessageID { get; set; }
            /// <summary>
            /// 回覆者編號
            /// </summary>
            public int MemberID { get; set; }
            /// <summary>
            /// 回覆內容
            /// </summary>
            public string ReplyContent { get; set; }
            /// <summary>
            /// 回覆時間
            /// </summary>
            public DateTime ReplyTime { get; set; }
        }
    }
}