using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDBExample.Entities;
using MongoDBExample.Repositories.Interfaces;

namespace MongoDBExample.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly IMongoCollection<Genre> _genreCollection;

        public GenreRepository(IOptions<MongoDbExampleDatabaseSettings> databaseSettings)
        {
            var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);

            _genreCollection = mongoDatabase.GetCollection<Genre>(databaseSettings.Value.GenreCollectionName);
        }

        public async Task AddAsync(Genre genre)
        {
            await _genreCollection.InsertOneAsync(genre);
        }

        public async Task DeleteAsync(string id)
        {
            await _genreCollection.DeleteOneAsync(genre => genre.Id == id);
        }

        public async Task<Genre?> GetAsync(string id)
        {
            return await _genreCollection.Find(genre => genre.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<string>> GetNamesAsync(List<string> ids)
        {
            return await _genreCollection.Find(genre => ids.Contains(genre.Id)).Project(genre => genre.Name).ToListAsync();
        }

        public async Task<List<Genre>> GetAsync()
        {
            return await _genreCollection.Find(_ => true).ToListAsync();
        }

        public async Task UpdateAsync(string id, Genre updatedGenre)
        {
            await _genreCollection.ReplaceOneAsync(genre => genre.Id == id, updatedGenre);
        }

    }
}
