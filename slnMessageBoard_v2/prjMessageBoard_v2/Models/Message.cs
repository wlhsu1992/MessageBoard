using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjMessageBoard_v2.Models
{
    public class Message
    {
        public int ID { get; set; }                    //留言編號
        public int MemberID { get; set; }             //留言者編號
        public string Title { get; set; }              //留言標題
        public string Content { get; set; }            //留言內容
        public DateTime CreateTime { get; set; }       //留言時間
    }
}