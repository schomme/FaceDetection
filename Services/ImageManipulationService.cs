using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.CompilerServices;
using FaceDetector.Models;
using FaceONNX;
using Image = FaceDetector.Models.Image;

namespace FaceDetector.Services;

class ImageManipulationService
{
    public Image GetSmallestImage(IEnumerable<Image> images)
    {
        if (!images.Any()) throw new ArgumentException(nameof(images));
        Image? smallestImage = null;
        foreach (var image in images)
        {
            if (smallestImage == null)
            {
                smallestImage = image;
                continue;
            }

            Debug.WriteLine($"({image.Bitmap.Size.Width} | {image.Bitmap.Size.Height})");
            if (smallestImage?.Bitmap.Size.Width > image.Bitmap.Size.Width) smallestImage = image;
            if (smallestImage?.Bitmap.Size.Height > image.Bitmap.Size.Height) smallestImage = image;
        }
        return smallestImage!;
    }

    public Image GetSmallestFaceImage(IEnumerable<Image> images)
    {
        if (!images.Any()) throw new ArgumentException(nameof(images));
        Image? smallestImage = null;
        FaceDetectionResult? faceDetectionResult = null;

        foreach (var image in images)
        {
            foreach (var faceResult in image.FaceDetectionResults)
            {
                if (faceDetectionResult == null)
                {
                    faceDetectionResult = faceResult;
                    smallestImage = image;
                    continue;
                }

                Debug.WriteLine($"({faceResult.Box.Width} | {faceResult.Box.Height})");
                if (faceResult.Box.Width < faceDetectionResult.Box.Width || faceResult.Box.Height < faceDetectionResult.Box.Height)
                {
                    faceDetectionResult = faceResult;
                    smallestImage = image;

                }
            }
        }
        return smallestImage!;
    }

    public Bitmap ResizeImageToTargetSize(Image source, Image targetSizeImage)
    {
        var destRect = new Rectangle(0, 0, targetSizeImage.Bitmap.Width, targetSizeImage.Bitmap.Height);
        var destImage = new Bitmap(targetSizeImage.Bitmap.Width, targetSizeImage.Bitmap.Height);

        destImage.SetResolution(source.Bitmap.HorizontalResolution, source.Bitmap.VerticalResolution);

        using (var graphics = Graphics.FromImage(destImage))
        {
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            using (var wrapMode = new ImageAttributes())
            {
                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                graphics.DrawImage(source.Bitmap, destRect, 0, 0, source.Bitmap.Width, source.Bitmap.Height, GraphicsUnit.Pixel, wrapMode);
            }
        }

        return destImage;
    }

    public Bitmap CropImage(Image image, Rectangle area)
    {
        var widthDelta = area.Width * 0.1;
        var HeightDelta = area.Height * 0.1;

        var X = widthDelta > area.X ? 0 : area.X - widthDelta;
        var Y = HeightDelta > area.Y ? 0 : area.Y - HeightDelta;
        var W = area.Width + 2 * widthDelta;
        var H = area.Height + 2 * HeightDelta;

        var rect = new Rectangle((int)X, (int)Y, (int)W, (int)H);
        return image.Bitmap.Clone(rect, image.Bitmap.PixelFormat);
    }

    public void SaveImage(Bitmap bitmap, string filename)
    {
        bitmap.Save(filename, ImageFormat.Jpeg);
    }
}