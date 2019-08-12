using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using prjMessageBoard_v2.Models;

namespace prjMessageBoard_v2.Controllers
{
    public class MessageController : Controller
    {
        private string _conStr =  System.Web.Configuration.WebConfigurationManager.ConnectionStrings["dbMessageBoard"].ConnectionString;

        /// <summary>
        /// 判斷使用者是否為會員，將頁面導向新增留言頁面。
        /// </summary>
        [LoginAuthorize]
        public ActionResult CreateMessage()
        {
            return View("CreateMessage","_LayoutLogin");
        }

        /// <summary>
        /// 接收使用者於表單填寫留言資料，並新增留言記錄至資料表。
        /// 將頁面導向留言列表。
        /// </summary>
        /// <param name="title">欲新增留言標題</param>
        /// <param name="content">欲新增留言內容</param>
        /// <returns></returns>
        [LoginAuthorize]
        [HttpPost]
        public ActionResult CreateMessage(string title, string content, HttpPostedFileBase photo)
        {
            string photoID = string.Empty;
            if(photo != null)
            {
                if (FileManager.CheckPhoto(photo))
                {
                    photoID = FileManager.SavePhoto(photo);
                }
                else
                {
                    ViewBag.Alert = "請上傳jpg或png格式的檔案，且檔案大小不能超過20KB";
                    return View("CreateMessage", "_LayoutLogin");
                }
            }

            MessageBoardModelManager.CreateMessage(title, content, photoID);
            return RedirectToAction("GetDiscussionList", "Message");
        }

        /// <summary>
        /// 於資料表取得留言列表記錄，
        /// 依據會員是否為登入狀態，分別將頁面導向套用不同背板的留言列表頁面。
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDiscussionList()
        {
            List<MessageBoardViewlModel.Discussion> discussionList = MessageBoardModelManager.GetDiscussionList();

            if (discussionList.Count == 0)
                ViewBag.Alert = "目前尚無留言";

            if (SessionManager.MemberID == null)
                return View("GetDiscussionList", "_LayoutLogout", discussionList);
            else
                return View("GetDiscussionList", "_LayoutLogin", discussionList);
        }

        /// <summary>
        /// 存取顯示"留言"及"回覆"內容相關的資料庫欄位，
        /// 依據會員是否為登入狀態，分別將頁面導向套用不同背板的留言列表頁面。
        /// </summary>
        /// <param name="messageID">欲取得記錄的留言編號</param>
        /// <returns></returns>
        public ActionResult GetDiscussionContent(int messageID)
        {
            MessageBoardViewlModel.DiscussionContent message = new MessageBoardViewlModel.DiscussionContent();
            message = MessageBoardModelManager.GetDiscussionContent(messageID);
           
            if (Session["MemberID"] == null)
                return View("GetDiscussionContent", "_LayoutLogout", message);
            else
                return View("GetDiscussionContent", "_LayoutLogin", message);
        }

        /// <summary>
        /// 由資料庫取得留言標題符合的留言記錄。
        /// </summary>
        /// <param name="keyword">欲搜尋留言標題的關鍵字字串</param>
        /// <returns></returns>
        public ActionResult SearchMessage(string keyword)
        {
            List<MessageBoardViewlModel.Discussion> discussionList = MessageBoardModelManager.SearchTitle(keyword);

            if (discussionList.Count == 0)
                ViewBag.Alert = $"沒有與關鍵字「{keyword}」相關的留言標題";

            if (Session["MemberID"] == null)
                return View("GetDiscussionList", "_LayoutLogout", discussionList);
            else
                return View("GetDiscussionList", "_LayoutLogin", discussionList);
        }

        /// <summary>
        /// 取得指定留言編號(messageID)的留言記錄，
        /// 將記錄傳遞至UpdateMessage檢視頁面。
        /// </summary>
        /// <param name="messageID">欲取得記錄的留言編號</param>
        /// <returns></returns>
        [LoginAuthorize]
        public ActionResult UpdateMessage(int messageID)
        {
            MessageBoardViewlModel.MessageEditor message = MessageBoardModelManager.GetMessage(messageID);
            return View("UpdateMessage", "_LayoutLogin", message);
        }

        /// <summary>
        /// 將輸入編輯留言參數標題(newTitle)、內文(newContent)更新回指定編號的資料表，
        /// 將頁面導向被修改留言內容的動作方法。
        /// </summary>
        /// <param name="messageID">欲更新留言的留言編號</param>
        /// <param name="newTitle">修改後留言標題</param>
        /// <param name="newContent">修改後留言內容</param>
        /// <returns></returns>
        [LoginAuthorize]
        [HttpPost]
        public ActionResult UpdateMessage(int messageID, string newTitle, string newContent)
        {
            try
            {
                MessageBoardModelManager.UpdateMessage(messageID, newTitle, newContent);
            }
            catch(Exception ex)
            {
                TempData["Alert"] = "更新時發生錯誤，請重新操作一次";
            }
            return RedirectToAction("GetDiscussionContent","Message", new { messageID = messageID });
        }

        /// <summary>
        /// 刪除指定編號(messageID)的留言、相關回覆記錄、及圖片檔案，將頁面導向留言列表。
        /// </summary>
        /// <param name="messageID">欲刪除記錄的留言編號</param>
        /// <returns></returns>
        [LoginAuthorize]
        [HttpPost]
        public ActionResult DeleteMessage(int messageID)
        {

            string photoID = MessageBoardModelManager.GetPhotoID(messageID);

            if(photoID != string.Empty)
            {
                try
                {
                    FileManager.RemovePhoto(photoID);
                }
                catch (Exception ex)
                {
                    TempData["Alert"] = "刪除時發生錯誤，請重新操作一次";
                }
            }

            try
            {
                MessageBoardModelManager.DeleteMessage(messageID);
            }
            catch (Exception ex)
            {
                TempData["Alert"] = "刪除時發生錯誤，請重新操作一次";
            }
            return RedirectToAction("GetDiscussionList");
        }
    }
}