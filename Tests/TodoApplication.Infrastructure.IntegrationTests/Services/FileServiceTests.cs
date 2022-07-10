using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using TodoApplication.Infrastructure.Services.File;
using TodoApplication.Tests.Common.Helpers;
using Xunit;

namespace TodoApplication.Infrastructure.IntegrationTests.Services;

public class FileServiceTests
{
    private readonly FileService _sut;

    public FileServiceTests()
    {
        _sut = new FileService();
    }

    [Fact]
    public async Task SaveAsync_Should_Save_File_And_Return_FilePath()
    {
        // setup
        var tempPath = Path.GetTempPath();
        var fileStream = (await MockHelper.GetMockImageAsync()).Object.OpenReadStream();

        // act
        var result = await _sut.SaveAsync(fileStream, tempPath);
        
        // assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();

        File.Exists(result).Should().BeTrue();
    }

    [Fact]
    public async Task SaveAsync_Should_Throw_ArgumentException()
    {
        // setup
        string? tempPath = null;
        var fileStream = (await MockHelper.GetMockImageAsync()).Object.OpenReadStream();
        
        // act
        Func<Task> act = () => _sut.SaveAsync(fileStream, tempPath);
        
        // assert
        // assert
        await act.Should().ThrowAsync<ArgumentException>();
        
        // setup
        tempPath = string.Empty;
        fileStream = (await MockHelper.GetMockImageAsync()).Object.OpenReadStream();
        
        // act
        act = () => _sut.SaveAsync(fileStream, tempPath);
        
        // assert
        // assert
        await act.Should().ThrowAsync<ArgumentException>();
        
        // setup
        tempPath = Path.GetTempPath();
        fileStream = null;
        
        // act
        act = () => _sut.SaveAsync(fileStream, tempPath);
        
        // assert
        // assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
}