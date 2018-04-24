using AdminWebapi.Models;
using BusinessLogic.CommonServices;
using BusinessLogic.HelperServices;
using DBAccess.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AdminWebapi.Controllers
{
    public class CommonController : ApiController
    {
        private readonly IImageUpload _ImageUpload;
        public CommonController(IImageUpload imageUpload)
        {
            _ImageUpload = imageUpload;
        }
        public CustomResponse<string> uploadImage(ImageUploadRequest request)
        {
            var imagePath = _ImageUpload.uploadImage(request.Base64EncodedString, request.FolderNameWhereToSave);
            return new CustomResponse<string> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = imagePath };
        }


    }
}
