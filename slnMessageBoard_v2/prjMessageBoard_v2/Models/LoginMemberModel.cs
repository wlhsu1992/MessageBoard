using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjMessageBoard_v2.Models
{
    /// <summary>
    /// 會員登入後識別資料屬性
    /// </summary>
    public class LoginMemberModel
    {
        /// <summary>
        /// 會員編號
        /// </summary>
        public int MemberID { get; set; }
        /// <summary>
        /// 會員帳號
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 會員名稱
        /// </summary>
        public string Name { get; set; }
    }
}