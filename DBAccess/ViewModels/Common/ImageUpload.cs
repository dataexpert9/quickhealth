using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.ViewModels.Common
{
    public class ImageUploadRequest
    {
        public string Base64EncodedString { get; set; }
        public string FolderNameWhereToSave { get; set; }
    }
}
