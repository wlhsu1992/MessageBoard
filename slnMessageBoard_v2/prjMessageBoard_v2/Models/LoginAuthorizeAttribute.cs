using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prjMessageBoard_v2.Models
{
    /// <summary>
    /// 通過Session["MemberID"]判斷使用者是否為登入狀態，
    /// 在將頁面導入指定Action以前進行驗證。
    /// </summary>
    public class LoginAuthorizeAttribute : AuthorizeAttribute 
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (SessionManager.MemberID == null) return false;
            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.HttpContext.Response.Redirect("/Member/Login");
        }
    }
}

