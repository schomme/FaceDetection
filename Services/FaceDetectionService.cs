using System.Drawing;
using FaceONNX;
using Image = FaceDetector.Models.Image;

namespace FaceDetector.Services;

class FaceDetectionService : IDisposable
{
    private readonly IFaceDetector _faceDetector;
    public FaceDetectionService(IFaceDetector faceDetector)
    {
        _faceDetector = faceDetector;
    }

    public void DetectFaces(Image image)
    {
        image.FaceDetectionResults = [.. _faceDetector.Forward(image.Bitmap)];
    }

    public async Task DetectFacesAsync(IEnumerable<Image> images)
    {
        var tasks = images.Select(i => Task.Run(() =>
            {
                DetectFaces(i);
            }
        ));
        await Task.WhenAll(tasks);
    }

    public void DetectFaces(IEnumerable<Image> images)
    {
        foreach (var image in images)
        {
            DetectFaces(image);
        }
    }
    public void Dispose()
    {
        _faceDetector?.Dispose();
    }
}