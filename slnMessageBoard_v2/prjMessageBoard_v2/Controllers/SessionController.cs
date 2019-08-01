using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prjMessageBoard_v2.Controllers
{
    public class SessionController : Controller
    {
        // GET: Session
        public ActionResult Index()
        {
            Session["MemberID"] = null;
            Session["Welcome"] = "會員帳號(會員名稱)你好";

            return RedirectToAction("GetMessageList", "Message");
        }
    }
}