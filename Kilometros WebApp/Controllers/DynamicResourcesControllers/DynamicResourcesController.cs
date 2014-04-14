using KilometrosDatabase;
using KilometrosDatabase.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kilometros_WebApp.Controllers {
	public class DynamicResourcesController : BaseController {
		// GET: /DynamicResources/Images/{filename}.{ext}
		public BinaryResult Images(string filename, string ext) {
			IPicture picture
				= Database.IPictureStore.Get(filename);

			if ( picture == null || picture.PictureExtension != ext )
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

		// GET: /DynamicResources/ImagesBW/{filename}.{ext}
		public BinaryResult ImagesBW(string filename, string ext) {
			IPicture picture
				= Database.IPictureStore.Get(filename);

			if ( picture == null || picture.PictureExtension != ext )
				throw new HttpException(
					404,
					"Not Found"
				);
			
			Image original
				= Image.FromStream(
					new MemoryStream(picture.Picture)
				);

			// Source: http://tech.pro/tutorial/660/csharp-tutorial-convert-a-color-image-to-grayscale # 3. Short and sweet
			//create a blank bitmap the same size as original
			Bitmap newBitmap
				= new Bitmap(original.Width, original.Height);

			//get a graphics object from the new image
			Graphics g = Graphics.FromImage(newBitmap);

			//create the grayscale ColorMatrix
			ColorMatrix colorMatrix = new ColorMatrix(
				new float[][] {
					new float[] {.3f, .3f, .3f, 0, 0},
					new float[] {.59f, .59f, .59f, 0, 0},
					new float[] {.11f, .11f, .11f, 0, 0},
					new float[] {0, 0, 0, 1, 0},
					new float[] {0, 0, 0, 0, 1}
				}
			);

			//create some image attributes
			ImageAttributes attributes = new ImageAttributes();

			//set the color matrix attribute
			attributes.SetColorMatrix(colorMatrix);

			//draw the original image on the new image
			//using the grayscale color matrix
			g.DrawImage(
				original,
				new Rectangle(
					0, 0,
					original.Width, original.Height
				),
				0, 0,
				original.Width, original.Height,
				GraphicsUnit.Pixel, attributes
			);

			//dispose the Graphics object
			g.Dispose();

			MemoryStream tempStream
				= new MemoryStream();
			newBitmap.Save(
				tempStream,
				original.RawFormat
			);
			
			// > Devolver imagen
			return new BinaryResult() {
				Content
					= tempStream.ToArray(),
				ContentType
					= picture.PictureMimeType
			};
		}
	}
}