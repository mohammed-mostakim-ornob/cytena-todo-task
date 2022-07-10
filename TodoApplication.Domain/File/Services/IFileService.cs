namespace TodoApplication.Domain.File.Services;

public interface IFileService
{
    public Task<string> SaveAsync(Stream fileStream, string directory);
}