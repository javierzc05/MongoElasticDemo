using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDBExample.Entities;
using MongoDBExample.Services.Interfaces;

namespace MongoDBExample.Services;

public class FileUploader : IFileUploader
{
    private readonly IOptions<MongoDbExampleDatabaseSettings> _options;
    private readonly ILogger<FileUploader> _logger;

    public FileUploader(IOptions<MongoDbExampleDatabaseSettings> options, ILogger<FileUploader> logger)
    {
        _options = options;
        _logger = logger;
    }

    public async Task<string> UploadFileAsync(string? base64String, string? fileName)
    {
        if (string.IsNullOrEmpty(base64String) || string.IsNullOrEmpty(fileName))
        {
            return string.Empty;
        }

        try
        {
            var bytes = Convert.FromBase64String(base64String);

            var path = Path.Combine(_options.Value.StorageConfiguration.Path, fileName);

            await using (var stream = new FileStream(path, FileMode.Create))
            {
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }

            return $"{_options.Value.StorageConfiguration.PublicUrl}{fileName}";

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading {fileName}", fileName);

            return string.Empty;
        }
    }
}