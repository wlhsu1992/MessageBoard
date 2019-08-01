using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjMessageBoard_v2.Models
{
    public class Reply
    {
        public int ID { get; set; }                  //回覆留言編號
        public int MessageID { get; set; }           //回覆所在的留言串編號
        public int MemberID { get; set; }            //回覆者編號 
        public string ReplyContent { get; set; }     //回覆內容
        public DateTime ReplyTime { get; set; }      //回覆時間
    }
}