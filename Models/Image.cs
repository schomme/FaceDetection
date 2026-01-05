using System.Drawing;
using FaceONNX;

namespace FaceDetector.Models;

class Image : IDisposable
{
    public Image(string file)
    {
        FilePath = file;
    }

    public string FilePath { get; set; }
    public Bitmap Bitmap => new(FilePath);
    public List<FaceDetectionResult> FaceDetectionResults { get; set; } = [];

    public void Dispose()
    {
        this.Bitmap?.Dispose();
    }
}