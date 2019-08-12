using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace prjMessageBoard_v2.Models
{
    internal class FileManager
    {
        /// <summary>
        /// 判斷上傳檔案是否為圖片檔，且檔案大小小於20KB
        /// 滿足上述條件傳回True；否則為False
        /// </summary>
        /// <param name="file">傳入檔案</param>
        /// <returns></returns>
        internal static bool CheckPhoto(HttpPostedFileBase file)
        {
            string fileExtension = Path.GetExtension(file.FileName);
            if (file.ContentLength > 20000)
                return false;

            if (fileExtension == ".jpg" || fileExtension == ".png") return true;
                else return false;
        }

        /// <summary>
        /// 生成檔案識別名稱，並將使用者上傳檔案儲存至指定路徑。
        /// </summary>
        /// <param name="file"></param>
        /// <return>檔案名稱</return>
        internal static string SavePhoto(HttpPostedFileBase file)
        {
            string fileName = DateTime.Now.ToString("yyyyMMddHHmmssff");
            fileName = string.Concat(fileName, ".jpg");
            string storePath = Path.Combine(HttpContext.Current.Server.MapPath("~/Photos"), fileName);
            file.SaveAs(storePath);

            return fileName;
        }

        /// <summary>
        /// 刪除指定名稱，於/Photos目錄下的檔案
        /// </summary>
        /// <param name="photoName">欲刪除的檔案名稱</param>
        internal static void RemovePhoto(string photoName)
        {
            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Photos"), photoName);
            FileInfo photoInfo = new FileInfo(path);
            photoInfo.Delete();
        }
    }
}