using Kms.Cloud.Database;
using Kms.Cloud.WebApp.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace Kms.Cloud.WebApp.Controllers {
    public abstract partial class BaseController {
        public Uri GetDynamicResourceUri(IPicture pictureObject) {
            return this.GetDynamicResourceUri(
                "Images",
                pictureObject.Guid.ToBase64String(),
                pictureObject.PictureExtension
            );
        }

        /// <summary>
        ///     Devuelve una Uri absoluta que apunta al Recurso generado dinámicamente especificado
        ///     por el nombre del archivo (normalmente el GUID del recurso en BD) y su extensión.
        /// </summary>
        /// <param name="method">
        ///     Método en controlador DynamicResources responsable de generar el recurso.
        /// </param>
        /// <param name="filename">
        ///     Nombre del archivo (normalmente el GUID del recurso en BD).
        /// </param>
        /// <param name="ext">
        ///     Extensión esperada por Método.
        /// </param>
        /// <returns>
        ///     URI absoluta que apunta al recurso descrito por los parámetros.
        /// </returns>
        public Uri GetDynamicResourceUri(string method, string filename, string ext) {
            var contentUrl = Url.Content(
                string.Format(
                    "~/{0}/{1}.{2}",
                    "DynamicResources/Images",
                    filename,
                    ext
                )
            );

            return new Uri(
                Request.Url,
                contentUrl
            );
        }

        public IPicture GetUploadedPictureBytes(HttpPostedFileBase pictureFile) {
            // Cargar la imágen en memoria desde el stream de subida
            Image uploadedImage;
            try {
                uploadedImage = Image.FromStream(pictureFile.InputStream);
            } catch ( ArgumentException ex ) {
                throw new InvalidDataException("File format is not supported", ex);
            }

            // Calcular tamaño del cuadrado para el cropping
            var squareSideSize = uploadedImage.Width > uploadedImage.Height
                ? uploadedImage.Height
                : uploadedImage.Width;

            if ( squareSideSize < 128 ) {
                uploadedImage.Dispose();
                throw new ArgumentException("Picture is too small", "pictureFile");
            }

            var croppedBitmap = new Bitmap(
                Settings.Default.KmsUserPictureSquareSize,
                Settings.Default.KmsUserPictureSquareSize
            );
            var croppedGraphics = Graphics.FromImage(croppedBitmap);
            
            // Preparar números para el cropping
            var widthPercent  = (float)uploadedImage.Width / (float)croppedBitmap.Width;
            var heightPercent = (float)uploadedImage.Height / (float)croppedBitmap.Height;
            var resizePercent = widthPercent > heightPercent ? widthPercent : heightPercent;

            var croppedWidth  = resizePercent * uploadedImage.Width;
            var croppedHeight = resizePercent * uploadedImage.Height;

            // Hacer el cropping
            croppedGraphics.DrawImage(
                uploadedImage,
                (Settings.Default.KmsUserPictureSquareSize - croppedWidth) / 2,
                (Settings.Default.KmsUserPictureSquareSize - croppedHeight) / 2,
                croppedWidth,
                croppedHeight
            );

            // Guardar el resultado en JPEG en memoria
            var memoryStream = new MemoryStream();
            var imageCodecInfo =
                ImageCodecInfo.GetImageEncoders()
                    .Where(w => w.MimeType.EndsWith("jpeg"))
                    .First();

            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 80L);

            croppedBitmap.Save(memoryStream, imageCodecInfo, encoderParameters);

            // Sacar los bytes del JPEG en memoria y devolver IPicture
            using ( uploadedImage )
            using ( croppedBitmap )
            using ( croppedGraphics )
            using ( memoryStream ) {
                return new IPicture {
                    Picture          = memoryStream.ToArray(),
                    PictureExtension = "jpg",
                    PictureMimeType  = imageCodecInfo.MimeType
                };
            }
        }
    }
}