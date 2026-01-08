using Image = FaceDetector.Models.Image;
using FaceDetector.Services;
using FaceONNX;
using Microsoft.Extensions.DependencyInjection;
using facedetector.Services;


var serviceCollection = new ServiceCollection();
serviceCollection.AddScoped<FaceDetectionService>();
serviceCollection.AddScoped<IFaceDetector,FaceONNX.FaceDetector>();
serviceCollection.AddScoped<ImageManipulationService>();
serviceCollection.AddScoped<IFileProviderService, LocalFileProviderService>();
var serviceProvider = serviceCollection.BuildServiceProvider();


//LOAD IMAGES
const string folderPath = @"C:\Users\TobiasSchomakers\OneDrive_Personal\OneDrive\Bilder\Eigene Aufnahmen\2025\09";
const string resultsFolder = @"D:\results";
var fileProviderService = serviceProvider.GetRequiredService<IFileProviderService>();

var images = await fileProviderService.LoadImagesAsync(folderPath, "*.jpg");


//DETECT FACES
var faceDetectionService = serviceProvider.GetRequiredService<FaceDetectionService>();
await faceDetectionService.DetectFacesAsync(images);


//FILTER IMAGES
//images = [.. images.Where(i => i.FaceDetectionResults.Count == 1)];

//CROP IMAGES
var imageManipulationService = serviceProvider.GetRequiredService<ImageManipulationService>();

foreach (var image in images)
{
    for (int i = 0; i < image.FaceDetectionResults.Count; i++)
    {
        var result = image.FaceDetectionResults[i];
        //Skip small images
        //if (image.FaceDetectionResults[i].Rectangle.Width < 500 || image.FaceDetectionResults[i].Rectangle.Height < 500) continue;

        Console.WriteLine($"Cropping Image: {image.FilePath} | Face: {i + 1} of {image.FaceDetectionResults.Count}");
        var newFilePath = Path.Combine(resultsFolder, $"{Path.GetFileNameWithoutExtension(image.FilePath)}_{i}{Path.GetExtension(image.FilePath)}");
        System.Console.WriteLine($"Width: {result.Rectangle.Width} | Height: {result.Rectangle.Height} | Angle: {result.Points.RotationAngle}");
        var croppedImage = FaceProcessingExtensions.Align(image.Bitmap, result.Rectangle, result.Points.RotationAngle);
        //var croppedImage = imageManipulationService.CropImageQuadratic(image, result.Rectangle);
        imageManipulationService.SaveImage(croppedImage, newFilePath);
    }
}
