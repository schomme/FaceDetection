using Image = FaceDetector.Models.Image;
using FaceDetector.Services;
using System.Diagnostics;

//https://github.com/FaceONNX/FaceONNX/blob/main/netstandard/Examples/FaceDetection/Program.cs

//LOAD IMAGES
const string folderPath = @"C:\Users\TobiasSchomakers\OneDrive_Personal\OneDrive\Bilder\Eigene Aufnahmen\2025\09";
const string resultsFolder = @"D:\results";
var images = Directory.EnumerateFiles(folderPath, "*.jpg", SearchOption.AllDirectories).Select(i => new Image(i)).ToList();


//DETECT FACES
var faceDetectionService = new FaceDetectionService(new FaceONNX.FaceDetector());
await faceDetectionService.DetectFacesAsync(images);


//FILTER IMAGES
images = [.. images.Where(i => i.FaceDetectionResults.Count == 1)];


//CROP IMAGES
var imageManipulationService = new ImageManipulationService();

foreach (var image in images)
{
    Debug.WriteLine($"Cropping Image: {image.FilePath}");
    var newFilePath = Path.Combine(resultsFolder, Path.GetFileName(image.FilePath));
    var croppedImage = imageManipulationService.CropImage(image, image.FaceDetectionResults.First().Rectangle);
    imageManipulationService.SaveImage(croppedImage, newFilePath);
}
