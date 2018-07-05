using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.HelperServices;
using System.IO;
using System.Web;

namespace BusinessLogic.CommonServices
{
    public class ImageUpload : IImageUpload
    {
        public string uploadImage(string base64EncodedString,string folderName)
        {
            var uniqueName = Guid.NewGuid().ToString() + ".jpg";
           ///   var currentPath = "E:\\Mehmood ul Hassan" + "\\ProfileImages\\";
            var currentPath = HttpContext.Current.Server.MapPath("~\\ImageDirectory") + "\\"+ folderName + "\\";
            if (!Directory.Exists(currentPath))
            {
                Directory.CreateDirectory(currentPath);
            }
            currentPath = currentPath + uniqueName;
            ImageHelper.SaveImage(base64EncodedString, currentPath);
            return "\\ImageDirectory\\" + folderName + "\\" + uniqueName;
        }
    }
}
