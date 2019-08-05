using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Data.SqlClient;
using prjMessageBoard_v2.Models;

namespace prjMessageBoard_v2.Controllers
{
    public class MessageController : Controller
    {
        string _conStr =  System.Web.Configuration.WebConfigurationManager.ConnectionStrings["dbMessageBoard"].ConnectionString;

        /// <summary>
        /// 判斷使用者是否為會員，將頁面導向新增留言頁面。
        /// </summary>
        /// <returns></returns>
        [LoginAuthorize]
        public ActionResult CreateMessage()
        {
            return View("CreateMessage","_LayoutLogin");
        }

        /// <summary>
        /// 接收使用者於表單填寫資料，新增留言紀錄至資料表。
        /// 將頁面導向留言列表。
        /// </summary>
        /// <param name="Title">欲新增留言標題</param>
        /// <param name="Content">欲新增留言內容</param>
        /// <returns></returns>
        [LoginAuthorize]
        [HttpPost]
        public ActionResult CreateMessage(string Title, string Content)
        {
            ModelManager.CreateMessage(Title, Content);
            return RedirectToAction("GetMessageList","Message");
        }

        /// <summary>
        /// 於資料表存取留言列表記錄，
        /// 依據會員是否為登入狀態，分別將頁面導向套用不同背板的留言列表頁面。
        /// </summary>
        /// <returns></returns>
        public ActionResult GetMessageList()
        {
            MessageBoardModel dbModel = new MessageBoardModel();
            dbModel = ModelManager.GetList();

            if (dbModel.Message.Count == 0)
                ViewBag.Alert = "目前尚無留言";
            else
            {
                dbModel.Message.Reverse();
                dbModel.Member.Reverse();
            }

            if (SessionManager.MemberID == null)
                return View("GetMessageList", "_LayoutLogout", dbModel);
            else
                return View("GetMessageList", "_LayoutLogin", dbModel);
        }

        /// <summary>
        /// 存取顯示"留言"及"回覆"內容相關的資料庫欄位，
        /// 依據會員是否為登入狀態，分別將頁面導向套用不同背板的留言列表頁面。
        /// </summary>
        /// <param name="MessageID">欲取得紀錄的留言編號</param>
        /// <returns></returns>
        public ActionResult GetMessageContent(int MessageID)
        {
            MessageBoardModel dbModel = new MessageBoardModel();
            dbModel = ModelManager.GetMessageContent(MessageID);
           
            if (Session["MemberID"] == null)
                return View("GetMessageContent", "_LayoutLogout", dbModel);
            else
                return View("GetMessageContent", "_LayoutLogin", dbModel);
        }

        /// <summary>
        /// 由資料庫取得留言標題符合的留言紀錄，
        /// 依據會員是否為登入狀態，分別將頁面導向套用不同背板的留言列表頁面。
        /// </summary>
        /// <param name="keyword">欲搜尋留言標題的關鍵字字串</param>
        /// <returns></returns>
        public ActionResult SearchMessage(string keyword)
        {
            MessageBoardModel dbModel = new MessageBoardModel();
            dbModel = ModelManager.GetTitle(keyword);

            if (dbModel.Message.Count == 0)
                ViewBag.Alert = $"沒有與關鍵字「{keyword}」相關的標題";
            else
            { 
                dbModel.Message.Reverse();
                dbModel.Member.Reverse();
            }
            if (Session["MemberID"] == null)
                return View("GetMessageList", "_LayoutLogout", dbModel);
            else
                return View("GetMessageList", "_LayoutLogin", dbModel);
        }

        /// <summary>
        /// 由資料表取得，指定留言編號的留言內容，
        /// 將資料傳遞至修改留言的檢視頁面。
        /// </summary>
        /// <param name="MessageID">欲取得紀錄的留言編號</param>
        /// <returns></returns>
        [LoginAuthorize]
        public ActionResult UpdateMessage(int MessageID)
        {
            MessageBoardModel dbModel = new MessageBoardModel();
            dbModel = ModelManager.GetMessage(MessageID);
            return View("UpdateMessage", "_LayoutLogin", dbModel);
        }

        /// <summary>
        /// 將表單輸入的留言內容，更新回指定編號的資料表，
        /// 將頁面導向被修改的留言內容頁面。
        /// </summary>
        /// <param name="MessageID">欲更新紀錄的留言編號</param>
        /// <param name="NewTitle">更新留言標題</param>
        /// <param name="NewContent">編輯後留言內容</param>
        /// <returns></returns>
        [LoginAuthorize]
        [HttpPost]
        public ActionResult UpdateMessage(int MessageID, string NewTitle, string NewContent)
        {
            ModelManager.UpdateMessage(MessageID, NewTitle, NewContent);
            return RedirectToAction("GetMessageContent","Message", new { MessageID = MessageID });
        }

        /// <summary>
        /// 於資料表中，刪除指定編號的記錄，
        /// 將頁面導向留言列表。
        /// </summary>
        /// <param name="MessageID">欲刪除紀錄的留言編號</param>
        /// <returns></returns>
        [LoginAuthorize]
        [HttpPost]
        public ActionResult DeleteMessage(int MessageID)
        {
            ModelManager.DeleteMessage(MessageID);
            return RedirectToAction("GetMessageList");
        }
    }
}