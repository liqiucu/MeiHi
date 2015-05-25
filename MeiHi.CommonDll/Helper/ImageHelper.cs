using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Net.Http;

namespace MeiHi.CommonDll.Helper
{
    /// <summary>
    /// Provides various image untilities, such as high quality resizing and the ability to save a JPEG.
    /// </summary>
    public static class ImageHelper
    {
        public static List<string> SaveImage(string lootUrl, string folder, HttpPostedFileBase[] images)
        {
            try
            {
                List<string> results = new List<string>();

                if (!Directory.Exists(HttpContext.Current.Server.MapPath(folder)))
                {
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath(folder));
                }

                foreach (var file in images)
                {
                    if (file != null)
                    {
                        string fileName = Guid.NewGuid().ToString() + ".jpeg";

                        var filePhysicalPath = HttpContext.Current.Server.MapPath(folder + fileName);
                        file.SaveAs(filePhysicalPath);

                        Image image = Image.FromFile(filePhysicalPath);

                        using (var resized = ImageHelper.ResizeImage(image, 100, 100))
                        {
                            ImageHelper.SaveJpeg(HttpContext.Current.Server.MapPath(folder + "100X100_" + fileName), resized, 100);
                        }

                        image.Dispose();
                        results.Add("http://" + lootUrl + folder + "100X100_" + fileName);
                    }
                }

                return results;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public static List<string> SaveImage(string lootUrl, string folder, MultipartFormDataStreamProvider bodyparts)
        {
            try
            {
                List<string> results = new List<string>();

                if (!Directory.Exists(HttpContext.Current.Server.MapPath(folder)))
                {
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath(folder));
                }

                foreach (var item in bodyparts.FileData)
                {
                    string fileName = Guid.NewGuid().ToString() + ".jpeg";

                    var filePhysicalPath = HttpContext.Current.Server.MapPath(folder + fileName);
                    File.Move(item.LocalFileName, filePhysicalPath);
                    Image image = Image.FromFile(filePhysicalPath);

                    using (var resized = ImageHelper.ResizeImage(image, 100, 100))
                    {
                        ImageHelper.SaveJpeg(HttpContext.Current.Server.MapPath(folder + "100X100_" + fileName), resized, 100);
                    }

                    image.Dispose();

                    results.Add("http://" + lootUrl + folder + "100X100_" + fileName);
                }

                return results;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public static void DeleteImageFromDataBaseAndPhyclePath(string fullName, string folder)
        {
            var temp1 = fullName.Split('/');
            var temp2 = temp1[temp1.Length - 1];
            var temp3 = temp2.Split('_')[1];
            //var fileName = temp2.Split('.')[0];

            string filePhycleName100 = HttpContext.Current.Server.MapPath(folder + temp2);
            string filePhycleName = HttpContext.Current.Server.MapPath(folder + temp3);

            if (File.Exists(filePhycleName100))
            {
                File.Delete(filePhycleName100);
            }

            if (File.Exists(filePhycleName))
            {
                File.Delete(filePhycleName);
            }
        }

        /// <summary>
        /// A quick lookup for getting image encoders
        /// </summary>
        private static Dictionary<string, ImageCodecInfo> encoders = null;

        /// <summary>
        /// A quick lookup for getting image encoders
        /// </summary>
        public static Dictionary<string, ImageCodecInfo> Encoders
        {
            //get accessor that creates the dictionary on demand
            get
            {
                //if the quick lookup isn't initialised, initialise it
                if (encoders == null)
                {
                    encoders = new Dictionary<string, ImageCodecInfo>();
                }

                //if there are no codecs, try loading them
                if (encoders.Count == 0)
                {
                    //get all the codecs
                    foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageEncoders())
                    {
                        //add each codec to the quick lookup
                        encoders.Add(codec.MimeType.ToLower(), codec);
                    }
                }

                //return the lookup
                return encoders;
            }
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static System.Drawing.Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            int imageWidth = image.Size.Width;
            int imageHeight = image.Size.Height;
            if (imageWidth > imageHeight)
            {
                height = (int)(width * (imageHeight * 1.00 / imageWidth));
            }
            else if (imageWidth < imageHeight)
            {
                width = (int)(height * (imageWidth * 1.00 / imageHeight));
            }
            //a holder for the result
            Bitmap result = new Bitmap(width, height);
            //set the resolutions the same to avoid cropping due to resolution differences
            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //use a graphics object to draw the resized image into the bitmap
            using (Graphics graphics = Graphics.FromImage(result))
            {
                //set the resize quality modes to high quality
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //draw the image into the target bitmap
                graphics.DrawImage(image, 0, 0, result.Width, result.Height);
            }

            //return the resulting bitmap
            return result;
        }

        /// <summary> 
        /// Saves an image as a jpeg image, with the given quality 
        /// </summary> 
        /// <param name="path">Path to which the image would be saved.</param> 
        /// <param name="quality">An integer from 0 to 100, with 100 being the 
        /// highest quality</param> 
        /// <exception cref="ArgumentOutOfRangeException">
        /// An invalid value was entered for image quality.
        /// </exception>
        public static void SaveJpeg(string path, Image image, int quality)
        {
            //ensure the quality is within the correct range
            if ((quality < 0) || (quality > 100))
            {
                //create the error message
                string error = string.Format("Jpeg image quality must be between 0 and 100, with 100 being the highest quality.  A value of {0} was specified.", quality);
                //throw a helpful exception
                throw new ArgumentOutOfRangeException(error);
            }

            //create an encoder parameter for the image quality
            EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            //get the jpeg codec
            ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");

            //create a collection of all parameters that we will pass to the encoder
            EncoderParameters encoderParams = new EncoderParameters(1);
            //set the quality parameter for the codec
            encoderParams.Param[0] = qualityParam;
            //save the image using the codec and the parameters
            image.Save(path, jpegCodec, encoderParams);
        }

        /// <summary> 
        /// Returns the image codec with the given mime type 
        /// </summary> 
        public static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            //do a case insensitive search for the mime type
            string lookupKey = mimeType.ToLower();

            //the codec to return, default to null
            ImageCodecInfo foundCodec = null;

            //if we have the encoder, get it to return
            if (Encoders.ContainsKey(lookupKey))
            {
                //pull the codec from the lookup
                foundCodec = Encoders[lookupKey];
            }

            return foundCodec;
        }
    }
}
