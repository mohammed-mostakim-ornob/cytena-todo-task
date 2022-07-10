using Microsoft.AspNetCore.Http;
using Moq;

namespace TodoApplication.Tests.Common.Helpers;

public static class MockHelper
{
    public const string MockImageName = "mockImage.jpeg";
    public const string PngImageName = "mockImage.png";
    public const string TooLargeImageName = "mockImage.png";
    

    private static string CurrentDirectoryPath =>Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.FullName.Replace("/bin", "");

    public static async Task<Mock<IFormFile>> GetMockImageAsync()
    {
        var sourceImg = File.OpenRead(Path.Combine(CurrentDirectoryPath, MockImageName));
        var memoryStream = new MemoryStream();
        var streamWriter = new StreamWriter(memoryStream);
        
        streamWriter.Write(sourceImg);
        await streamWriter.FlushAsync();
        
        memoryStream.Position = 0;
        
        var imageFile = new Mock<IFormFile>();
        
        imageFile.Setup(f => f.FileName).Returns(MockImageName);
        imageFile.Setup(f => f.OpenReadStream()).Returns(memoryStream);
        imageFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns((Stream stream, CancellationToken token) => memoryStream.CopyToAsync(stream, token));

        return imageFile;
    }

    public static FileStream GetMockImageStream()
    {
        return File.OpenRead(Path.Combine(CurrentDirectoryPath, MockImageName));
    }

    public static FileStream GetPngImageStream()
    {
        return File.OpenRead(Path.Combine(CurrentDirectoryPath, PngImageName));
    }
    
    public static FileStream GetTooLargeImageStream()
    {
        return File.OpenRead(Path.Combine(CurrentDirectoryPath, TooLargeImageName));
    }
}