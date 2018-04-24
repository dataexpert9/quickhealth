using MultipartDataMediaFormatter.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BusinessLogic.HelperServices
{
    public static class ImageHelper
    {
        public static void SaveImage(string base64String, string pathToSave)
        {
            //data:image/gif;base64,
            //this image is a single pixel (black)
            byte[] bytes = Convert.FromBase64String(base64String);

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                using (Image image = Image.FromStream(ms))
                {
                    image.Save(pathToSave, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }

        }

        public static string SaveFileFromBytes(HttpFile httpFile, string folderName)
        {
            if (httpFile != null)
            {
                var ext = httpFile.FileName.Substring(httpFile.FileName.LastIndexOf('.'));
                var uniqueName = Guid.NewGuid().ToString() + ext;
                ///   var currentPath = "E:\\Mehmood ul Hassan" + "\\ProfileImages\\";
                var currentPath = HttpContext.Current.Server.MapPath("~\\api\\ImageDirectory") + "\\" + folderName + "\\";
                if (!Directory.Exists(currentPath))
                {
                    Directory.CreateDirectory(currentPath);
                }
                currentPath = currentPath + uniqueName;
                File.WriteAllBytes(currentPath, httpFile.Buffer);
                return "\\ImageDirectory\\" + folderName + "\\" + uniqueName;
            }
            else { return ""; }

        }

        public static bool DeleteFile(string filePath)
        {
            var DirectoryPath = HttpContext.Current.Server.MapPath("~\\api\\" + filePath);
            if (File.Exists(DirectoryPath))
            {
                File.Delete(DirectoryPath);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
