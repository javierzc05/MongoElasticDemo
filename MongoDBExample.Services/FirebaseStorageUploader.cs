using Firebase.Auth;
using Firebase.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDBExample.Entities;
using MongoDBExample.Services.Interfaces;


namespace MongoDBExample.Services;

public class FirebaseStorageUploader : IFileUploader
{
    private readonly IOptions<MongoDbExampleDatabaseSettings> _options;
    private readonly ILogger<FirebaseStorageUploader> _logger;

    public FirebaseStorageUploader(IOptions<MongoDbExampleDatabaseSettings> options, ILogger<FirebaseStorageUploader> logger)
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
            await using var stream = new MemoryStream(Convert.FromBase64String(base64String));
            FirebaseAuthProvider authProvider = new(new FirebaseConfig(_options.Value.FirebaseConfiguration.ApiKey));

            FirebaseAuthLink authConfiguration = await authProvider.SignInWithEmailAndPasswordAsync(
                _options.Value.FirebaseConfiguration.Email,
                _options.Value.FirebaseConfiguration.Password);

            CancellationTokenSource cancellationToken = new();

            FirebaseStorageTask storageManager = new FirebaseStorage(
                _options.Value.FirebaseConfiguration.Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(authConfiguration.FirebaseToken),
                    ThrowOnCancel = true
                })
                .Child(_options.Value.FirebaseConfiguration.StorageFolder)
                .Child(fileName)
                .PutAsync(stream, cancellationToken.Token);

            return await storageManager;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading {fileName}", fileName);
            return string.Empty;
        }
    }
}

