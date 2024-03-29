﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjMessageBoard_v2.Models
{
    public class SessionManager
    {
        /// <summary>
        /// 對指定名稱的Session指派值
        /// </summary>
        /// <param name="sessionName">Session名稱</param>
        /// <param name="sessionValue">Session指派值</param>
        public static void Save<T> (string sessionName, T sessionValue)
        {
            HttpContext.Current.Session[sessionName] = sessionValue;
        }

        /// <summary>
        /// 依照指定Session名稱建立Session
        /// </summary>
        /// <param name="sessionName">指定Session名稱</param>
        /// <returns></returns>
        public static T Get<T> (string sessionName)
        {
            return (T)HttpContext.Current.Session[sessionName];
        }

        /// <summary>
        /// 識別使用者是否為登入狀態
        /// </summary>
        public static int? MemberID
        {
            get
            {
                return Get<int?>(SessionName.MemberID);
            }
            set
            {
                Save(SessionName.MemberID, value);
            }
        }

        /// <summary>
        /// 使用者登入後的歡迎詞
        /// </summary>
        public static string MemberWelcome
        {
            get
            {
                return Get<string>(SessionName.MemberWelcome);
            }
            set
            {
                Save(SessionName.MemberWelcome , value);
            }
        }

        public struct SessionName
        {
            internal const string MemberID = "MemberID";
            internal const string MemberWelcome = "MemberWelcome";
        }
    }
}