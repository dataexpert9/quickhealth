using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.CommonServices
{
    public interface IImageUpload
    {
        string uploadImage(string base64EncodedString,string FolderName);
    }
}
