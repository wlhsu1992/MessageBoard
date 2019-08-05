using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using prjMessageBoard_v2.Models;
using System.Data;
using System.Data.SqlClient;

namespace prjMessageBoard_v2.Controllers
{
    public class ReplyController : Controller
    {
         string _conStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["dbMessageBoard"].ConnectionString;

        /// <summary>
        /// 新增留言回覆紀錄，
        /// 將頁面導向該則回覆所在留言串的內容頁面。
        /// </summary>
        /// <param name="MessageID">欲新增回覆記錄的留言編號</param>
        /// <param name="MemberID">欲新增回覆的會員編號</param>
        /// <param name="ReplyContent">欲新增回覆的回覆內容</param>
        /// <returns></returns>
        [LoginAuthorize]
        [HttpPost]
        public ActionResult CreateReply(int MessageID, int MemberID, string ReplyContent)
        {
            ModelManager.CreateReply(MessageID, MemberID, ReplyContent);
            return RedirectToAction("GetMessageContent","Message", new { MessageID = MessageID });
        }

        /// <summary>
        /// 由資料表取得指定回覆編號的留言內容，
        /// 將該筆留言內容傳遞給修改留言的檢視頁面。
        /// </summary>
        /// <param name="ReplyID">欲取得紀錄的回覆編號</param>
        /// <returns></returns>
        [LoginAuthorize]
        public ActionResult UpdateReply(int ReplyID)
        {
            MessageBoardModel dbModel = new MessageBoardModel();
            dbModel = ModelManager.GetReply(ReplyID);
            return View("UpdateReply", "_LayoutLogin", dbModel);
        }

        /// <summary>
        /// 由表單輸入的回覆內容，更新回指定紀錄的資料表，
        /// 將頁面導向該則回覆所在留言串的內容頁面。
        /// </summary>
        /// <param name="MessageID">欲更新紀錄的留言編號</param>
        /// <param name="ReplyID">欲更新紀錄的回覆編號</param>
        /// <param name="NewContent">編輯後回覆內容</param>
        /// <returns></returns>
        [LoginAuthorize]
        [HttpPost]
        public ActionResult UpdateReply(int MessageID, int ReplyID, string NewContent)
        {
            ModelManager.UpdateReply(ReplyID, NewContent);
            return RedirectToAction("GetMessageContent","Message", new { MessageID = MessageID });
        }

        /// <summary>
        /// 於資料表中，刪除指定編號的記錄。
        /// 將頁面導向該則回覆所在留言串的內容頁面。
        /// </summary>
        /// <param name="MessageID">欲刪除回覆的留言編號</param>
        /// <param name="ReplyID">欲刪除回覆的回覆編號</param>
        /// <returns></returns>
        [LoginAuthorize]
        [HttpPost]
        public ActionResult DeleteReply(int MessageID, int ReplyID)
        {
            ModelManager.DeleteReply(ReplyID);
            return RedirectToAction("GetMessageContent","Message", new { MessageID = MessageID });
        }
    }
}