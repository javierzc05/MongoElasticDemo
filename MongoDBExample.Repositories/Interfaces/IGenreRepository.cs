using MongoDBExample.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBExample.Repositories.Interfaces
{
    public interface IGenreRepository
    {
        Task<List<Genre>> GetAsync();

        Task<List<string>> GetNamesAsync(List<string> ids);

        Task<Genre?> GetAsync(string id);

        Task AddAsync(Genre genre);

        Task UpdateAsync(string id, Genre updatedGenre);

        Task DeleteAsync(string id);
    }
}
