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
         private string _conStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["dbMessageBoard"].ConnectionString;

        /// <summary>
        /// 接收表單參數，新增留言回覆，導向該則回覆所在的內容動作方法。
        /// </summary>
        /// <param name="messageID">欲新增回覆所在的留言編號</param>
        /// <param name="memberID">欲新增回覆的會員編號</param>
        /// <param name="replyContent">欲新增回覆的回覆內容</param>
        /// <returns></returns>
        [LoginAuthorize]
        [HttpPost]
        public ActionResult CreateReply(int messageID, int memberID, string replyContent)
        {
            MessageBoardModelManager.CreateReply(messageID, memberID, replyContent);
            return RedirectToAction("GetDiscussionContent","Message", new { messageID = messageID });
        }

        /// <summary>
        /// 取得指定回覆編號(replyID)的回覆記錄，將記錄傳遞給更新檢視頁面。
        /// </summary>
        /// <param name="replyID">欲取得記錄的回覆編號</param>
        /// <returns></returns>
        [LoginAuthorize]
        public ActionResult UpdateReply(int replyID)
        {
            MessageBoardViewlModel.ReplyEditor reply = MessageBoardModelManager.GetReply(replyID);
            return View("UpdateReply", "_LayoutLogin", reply);
        }

        /// <summary>
        /// 接收表單參數，修改回覆內容。
        /// 若修改發生錯誤，顯示提示訊息，導向輸入修改回覆表單動作方法；
        /// 若修改成功，導向該則回覆所在的內容動作方法
        /// </summary>
        /// <param name="messageID">欲更新記錄的留言編號</param>
        /// <param name="replyID">欲更新記錄的回覆編號</param>
        /// <param name="newContent">編輯後回覆內容</param>
        /// <returns></returns>
        [LoginAuthorize]
        [HttpPost]
        public ActionResult UpdateReply(int messageID, int replyID, string newContent)
        {
            try
            {
                MessageBoardModelManager.UpdateReply(replyID, newContent);
            }
            catch (Exception ex)
            {
                TempData["Alert"] = "修改時發生錯誤，請重新操作一次";
                return RedirectToAction("UpdateReply", "Reply", new { replyID = replyID });
            }
            return RedirectToAction("GetDiscussionContent","Message", new { messageID = messageID });
        }

        /// <summary>
        /// 刪除指定編號的記錄，導向該則回覆所在的內容動作方法。
        /// </summary>
        /// <param name="messageID">欲刪除回覆的留言編號</param>
        /// <param name="replyID">欲刪除回覆的回覆編號</param>
        /// <returns></returns>
        [LoginAuthorize]
        [HttpPost]
        public ActionResult DeleteReply(int messageID, int replyID)
        {
            try
            {
                MessageBoardModelManager.DeleteReply(replyID);
            }
            catch (Exception ex)
            {
                TempData["Alert"] = "刪除時發生錯誤，請重新操作一次";
            }
            return RedirectToAction("GetDiscussionContent","Message", new { messageID = messageID });
        }
    }
}