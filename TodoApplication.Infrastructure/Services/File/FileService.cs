using TodoApplication.Common.Constants;
using TodoApplication.Domain.File.Services;

namespace TodoApplication.Infrastructure.Services.File;

public class FileService : IFileService
{
    public async Task<string> SaveAsync(Stream fileStream, string directory)
    {
        if (fileStream == null)
            throw new ArgumentException(ExceptionMessages.FileStreamNull);
        
        if (string.IsNullOrEmpty(directory))
            throw new ArgumentException(ExceptionMessages.FileNameNull);

        var randomFileName = Path.GetRandomFileName();

        CreateDirectoryIfNotExist(directory);
        
        var filePath = Path.Combine(directory, randomFileName);

        await using var outputFileStream = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(outputFileStream);

        return filePath;
    }

    private void CreateDirectoryIfNotExist(string directory)
    {
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
    }
}