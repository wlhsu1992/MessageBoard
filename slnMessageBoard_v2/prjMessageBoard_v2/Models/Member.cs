using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjMessageBoard_v2.Models
{
    public class Member
    {
        public int ID { get; set; }                    //會員編號
        public string Account { get; set; }            //會員帳號
        public string Password { get; set; }           //會員密碼
        public string Name { get; set; }               //會員姓名
    }
}