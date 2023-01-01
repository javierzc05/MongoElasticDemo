using MongoDB.Driver;
using MongoDBExample.Entities;
using MongoDBExample.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using Nest;

namespace MongoDBExample.Repositories;

public class ConcertRepository : IConcertRepository
{
    private readonly IMongoCollection<Concert> concertsCollection;
    private readonly IElasticClient _elasticClient;


    public ConcertRepository(IOptions<MongoDbExampleDatabaseSettings> databaseSettings, IElasticClient elasticClient)
    {
        var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);

        concertsCollection = mongoDatabase.GetCollection<Concert>(databaseSettings.Value.ConcertCollectionName);

        _elasticClient = elasticClient;
    }

    public async Task<ICollection<Concert>> GetAsync(string? filter, string beginDate, string endDate, int page, int rows)
    {
        var skip = (page - 1) * rows;

        var searchResponse = await _elasticClient.SearchAsync<Concert>(s => s
            .From(skip)
            .Size(rows)
            .Query(q => q
                .Bool(b => b
                    .Must(m => m
                        .QueryString(qs => qs
                            // Add fields here where you want to perform the search
                            .Fields(fields => fields
                                .Field(f => f.Title)
                                .Field(f => f.Description)
                                .Field(f => f.Genres)
                                .Field(f => f.Place)
                            )
                            .Query("*" + (filter ?? string.Empty) + "*")
                        )
                    )
                    .Filter(f => f
                        .DateRange(r => r
                            .Field(f => f.DateEvent)
                            .GreaterThanOrEquals(beginDate)
                            .LessThanOrEquals(endDate)
                        )
                    )
                )
            )
        );

        return searchResponse.Documents.ToList();
    }
    public async Task<ICollection<Concert>> GetAsync(string? filter, int page, int rows)
    {
        var skip = (page - 1) * rows;
        // var topLevelProjection = Builders<Concert>.Projection
        //     .Exclude(u => u.Id)
        //     .Include(u => u.Title)
        //     .Include(u => u.DateEvent);

        // return await concertsCollection
        //     .Find(c => c.Title.ToLower().Contains((filter ?? string.Empty).ToLower()))
        //     //.Find(Builders<Concert>.Filter.Empty)
        //     .SortBy(c => c.DateEvent)
        //     .Skip(skip)
        //     .Limit(rows)
        //     .ToListAsync();


        var searchResponse = await _elasticClient.SearchAsync<Concert>(s => s
            .From(skip)
            .Size(rows)
            .Query(q => q
                .QueryString(qs => qs
                    // Add fields here where you want to perform the search
                    .Fields(fields => fields
                        .Field(f => f.Title)
                        .Field(f => f.Description)
                        .Field(f => f.Genres)
                        .Field(f => f.Place)
                    )
                    .Query("*" + (filter ?? string.Empty) + "*")
                )
            ));

        return searchResponse.Documents.ToList();
    }

    public async Task<Concert?> GetAsync(string id)
    {
        // return await concertsCollection.Find(concert => concert.Id == id).FirstOrDefaultAsync();
        var response = await _elasticClient.GetAsync<Concert>(id);
        return response.Source;
    }

    public async Task AddAsync(Concert concert)
    {
        await _elasticClient.IndexDocumentAsync(concert);
    }

    public async Task UpdateAsync(string id, Concert updatedConcert)
    {
        //await concertsCollection.ReplaceOneAsync(concert => concert.Id == id, updatedConcert);
        await _elasticClient.UpdateAsync<Concert>(id, u => u.Doc(updatedConcert));
    }

    public async Task DeleteAsync(string id)
    {
        //await concertsCollection.DeleteOneAsync(concert => concert.Id == id);
        await _elasticClient.DeleteAsync<Concert>(id);
    }
}