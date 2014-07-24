using Kms.Cloud.Database;
using Kms.Cloud.WebApp.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kms.Cloud.WebApp.Controllers {
    [KmsAuthorize]
    public class AccountController : BaseController {
        [HttpPost]
        public ActionResult UploadPicture(HttpPostedFileBase file) {
            IPicture picture;
            try {
                picture = GetUploadedPictureBytes(file);
            } catch ( Exception ex ) {
                if ( ex is ArgumentException )
                    throw new HttpException(400, "Bad Request", ex);
                else if ( ex is InvalidDataException )
                    throw new HttpException(415, "Unsupported Media Type", ex);
                else
                    throw ex;
            }

            if ( CurrentUser.UserPicture == null ) {
                Database.UserPictureStore.Add(new UserPicture {
                    Picture = picture.Picture,
                    PictureExtension = picture.PictureExtension,
                    PictureMimeType = picture.PictureMimeType,
                    User = CurrentUser
                });
            } else {
                CurrentUser.UserPicture.Guid             = Guid.NewGuid();
                CurrentUser.UserPicture.CreationDate     = DateTime.UtcNow;
                CurrentUser.UserPicture.Picture          = picture.Picture;
                CurrentUser.UserPicture.PictureExtension = picture.PictureExtension;
                CurrentUser.UserPicture.PictureMimeType  = picture.PictureMimeType;
            }

            Database.SaveChanges();
            return Redirect("~/Overview");
        }
    }
}