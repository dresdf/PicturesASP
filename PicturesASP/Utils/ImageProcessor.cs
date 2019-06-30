using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PicturesASP.Models;
using SkiaSharp;
using System;
using System.IO;

namespace PicturesASP.Utils
{
    public class ImageProcessor
    {
        const int Quality = 75;
        const int imageSize = 500;

        //convert file size to 500X500 and save it on disk
        public void SaveImage(IFormFile file, IHostingEnvironment env, string currentFolder)
        {
            string fileName = Path.GetFileName(file.FileName);
            fileName = DisplayUtils.RenameDuplicates(env, currentFolder, fileName);
            string filePath = Path.Combine(env.WebRootPath, currentFolder, fileName);

            var format = GetFormat(filePath);

            using (var imageStream = file.OpenReadStream())
            using (var inputStream = new SKManagedStream(imageStream))
            using (var codec = SKCodec.Create(inputStream))
            using (var original = SKBitmap.Decode(codec))
            using (var image = HandleOrientation(original, codec.Origin))
            //using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                int width = imageSize;
                int height = (int)Math.Round(width * ((float)image.Height / image.Width));

                var info = new SKImageInfo(width, height);

                using (var resized = image.Resize(info, SKBitmapResizeMethod.Lanczos3))
                using (var thumb = SKImage.FromBitmap(resized))
                using (var fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.ReadWrite))
                {
                    thumb.Encode(format, Quality)
                         .SaveTo(fs);
                }


                //imageProcessor.CreateThumbnails(imageStream, filePath);
                //await file.CopyToAsync(fileStream);
            }
        }

        //creates scaled down versions of the image and saves them on disk
        public void CreateThumbnails(Stream imageStream, string filePath)
        {
            //folder for thumbnails
            string dir = Path.Combine(Path.GetDirectoryName(filePath), "thumbnails");
            Directory.CreateDirectory(dir);

            //file name without extensions
            string displayName = Path.GetFileNameWithoutExtension(filePath);

            //file extensions
            string ext = Path.GetExtension(filePath);

            var format = GetFormat(filePath);

            using (var inputStream = new SKManagedStream(imageStream))
            using (var codec = SKCodec.Create(inputStream))
            using (var original = SKBitmap.Decode(codec))
            using (var image = HandleOrientation(original, codec.Origin))
            {
                foreach (ImageType type in Enum.GetValues(typeof(ImageType)))
                {
                    int width = (int)type;
                    int height = (int)Math.Round(width * ((float)image.Height / image.Width));

                    string thumbnailPath = Path.Combine(dir, $"{displayName}-{width}x{height}{ext}");
                    var info = new SKImageInfo(width, height);

                    using (var resized = image.Resize(info, SKBitmapResizeMethod.Lanczos3))
                    using (var thumb = SKImage.FromBitmap(resized))
                    using (var fs = new FileStream(thumbnailPath, FileMode.CreateNew, FileAccess.ReadWrite))
                    {
                        thumb.Encode(format, Quality)
                             .SaveTo(fs);
                    }
                }
            }
        }

        //takes a filename string
        //returns the file format code for SkiaSharp
        private static SKEncodedImageFormat GetFormat(string fileName)
        {
            string ext = Path.GetExtension(fileName.ToLowerInvariant());

            switch (ext)
            {
                case ".gif":
                    return SKEncodedImageFormat.Gif;
                case ".png":
                    return SKEncodedImageFormat.Png;
                case ".webp":
                    return SKEncodedImageFormat.Webp;
                case ".bmp":
                    return SKEncodedImageFormat.Bmp;
                case ".jpeg":
                    return SKEncodedImageFormat.Jpeg;
            }

            return SKEncodedImageFormat.Jpeg;
        }

        private static SKBitmap HandleOrientation(SKBitmap bitmap, SKCodecOrigin orientation)
        {
            SKBitmap rotated;
            switch (orientation)
            {
                case SKCodecOrigin.BottomRight:

                    using (var surface = new SKCanvas(bitmap))
                    {
                        surface.RotateDegrees(180, bitmap.Width / 2, bitmap.Height / 2);
                        surface.DrawBitmap(bitmap.Copy(), 0, 0);
                    }

                    return bitmap;

                case SKCodecOrigin.RightTop:
                    rotated = new SKBitmap(bitmap.Height, bitmap.Width);

                    using (var surface = new SKCanvas(rotated))
                    {
                        surface.Translate(rotated.Width, 0);
                        surface.RotateDegrees(90);
                        surface.DrawBitmap(bitmap, 0, 0);
                    }

                    return rotated;

                case SKCodecOrigin.LeftBottom:
                    rotated = new SKBitmap(bitmap.Height, bitmap.Width);

                    using (var surface = new SKCanvas(rotated))
                    {
                        surface.Translate(0, rotated.Height);
                        surface.RotateDegrees(270);
                        surface.DrawBitmap(bitmap, 0, 0);
                    }

                    return rotated;

                default:
                    return bitmap;
            }
        }


    }
}
