using Image = FaceDetector.Models.Image;

namespace facedetector.Services
{
    internal class LocalFileProviderService : IFileProviderService
    {
        public Task<Image> LoadImageAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) throw new Exception($"{nameof(filePath)} can not be null or empty");
            if (!File.Exists(filePath)) throw new FileNotFoundException($"The file '{filePath}' does not exist");
            var image = new Image(filePath);
            return Task.FromResult(image);
        }

        public Task<IEnumerable<Image>> LoadImagesAsync(string folderPath, string fileExtension = "*.*")
        {
            return Task.FromResult(
                Directory.EnumerateFiles(folderPath, fileExtension, SearchOption.AllDirectories).Select(i => new Image(i))
            );
        }
    }
}
