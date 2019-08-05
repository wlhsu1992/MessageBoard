using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjMessageBoard_v2.Models
{
    /// <summary>
    /// 留言資料表
    /// </summary>
    public class Message
    {
        /// <summary>
        /// 留言編號
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 留言者編號
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
}