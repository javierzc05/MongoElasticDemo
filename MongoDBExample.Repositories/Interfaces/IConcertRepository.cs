using MongoDBExample.Entities;

namespace MongoDBExample.Repositories.Interfaces;

public interface IConcertRepository
{
    Task<ICollection<Concert>> GetAsync(string? filter, int page, int rows);

    Task<ICollection<Concert>> GetAsync(string? filter, string dateBegin, string dateEnd, int page, int rows);

    Task<Concert?> GetAsync(string id);

    Task AddAsync(Concert concert);

    Task UpdateAsync(string id, Concert updatedConcert);

    Task DeleteAsync(string id);
}


