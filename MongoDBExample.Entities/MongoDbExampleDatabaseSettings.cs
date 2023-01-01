namespace MongoDBExample.Entities
{
    public class MongoDbExampleDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string GenreCollectionName { get; set; } = null!;

        public string ConcertCollectionName { get; set; } = null!;

        public string UserCollectionName { get; set; } = null!;

        public StorageConfiguration StorageConfiguration { get; set; } = default!;

        public Jwt Jwt { get; set; } = default!;

        public SmtpConfiguration SmtpConfiguration { get; set; } = default!;
        
        public FirebaseConfiguration FirebaseConfiguration { get; set; } = default!;
    }

    public class Jwt
    {
        public string Secret { get; set; } = default!;
        public string Audience { get; set; } = default!;

        public string Issuer { get; set; } = default!;
    }

    public class SmtpConfiguration
    {
        public string ApiKey { get; set; } = default!;
        public string From { get; set; } = default!;

        public string FromName { get; set; } = default!;
    }

    public class StorageConfiguration
    {
        public string Path { get; set; } = default!;
        public string PublicUrl { get; set; } = default!;
    }

    public class FirebaseConfiguration
    {
        public string ApiKey { get; set; } = default!;
        public string Bucket { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string StorageFolder { get; set; } = default!;
    }
}
