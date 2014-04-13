using KilometrosDatabase;
using KilometrosDatabase.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kilometros_WebApp.Controllers {
	public class DynamicResourcesController : BaseController {
		//
		// GET: /DynamicResources/
		public BinaryResult Image(string filename, string ext) {
			IPicture picture
				= Database.IPictureStore.Get(filename);

			if ( picture == null )
				throw new HttpException(
					404,
					"Not Found"
				);
			else
				return new BinaryResult() {
					ContentType
						= picture.PictureMimeType,
					Content
						= picture.Picture
				};
		}
	}
}