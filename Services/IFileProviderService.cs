using Image = FaceDetector.Models.Image;

namespace facedetector.Services
{
    internal interface IFileProviderService
    {
        public Task<Image> LoadImageAsync(string filePath);
        public Task<IEnumerable<Image>> LoadImagesAsync(string folderPath, string fileExtension = "*.*");
    }
}
