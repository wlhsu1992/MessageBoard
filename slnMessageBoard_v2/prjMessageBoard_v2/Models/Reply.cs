using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjMessageBoard_v2.Models
{
    /// <summary>
    /// 回覆資料表
    /// </summary>
    public class Reply
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