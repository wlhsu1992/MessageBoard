using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjMessageBoard_v2.Models
{
    public class MessageBoardModel
    {
        public List<Member> member { get; set; }
        public List<Message> message { get; set; }
        public List<Reply> reply { get; set; }
    }
}