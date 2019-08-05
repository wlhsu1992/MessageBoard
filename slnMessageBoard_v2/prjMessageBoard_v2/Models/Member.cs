using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjMessageBoard_v2.Models
{
    /// <summary>
    /// 會員資料表
    /// </summary>
    public class Member
    {
        /// <summary>
        /// 會員編號
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 會員帳號
        /// </summary>
        public string Account { get; set; } 
        /// <summary>
        /// 會員密碼
        /// </summary>
        public string Password { get; set; } 
        /// <summary>
        /// 會員名稱
        /// </summary>
        public string Name { get; set; }      
    }
}