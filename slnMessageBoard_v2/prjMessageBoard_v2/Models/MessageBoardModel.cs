using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjMessageBoard_v2.Models
{
    /// <summary>
    /// 包含會員留言板專案所使用到的資料庫成員
    /// </summary>
    public class MessageBoardModel
    {
        /// <summary>
        /// 對應Member資料表欄位的List物件
        /// </summary>
        public List<Member> Member { get; set; }
        /// <summary>
        /// 對應Message資料表欄位的List物件
        /// </summary>
        public List<Message> Message { get; set; }
        /// <summary>
        /// 對應Reply資料表欄位的List物件
        /// </summary>
        public List<Reply> Reply { get; set; }
    }
}