using MongoDBExample.Dto.Request;
using MongoDBExample.Dto.Response;

namespace MongoDBExample.Services.Interfaces
{
    public interface IGenreService
    {
        Task<BaseResponseGeneric<IEnumerable<GenreDtoResponse>>> GetAsync();

        Task<BaseResponseGeneric<GenreDtoResponse>> GetAsync(string id);

        Task<BaseResponseGeneric<string>> AddAsync(GenreDtoRequest genre);
        
        Task<BaseResponse> UpdateAsync(string id, GenreDtoRequest genre);

        Task<BaseResponse> DeleteAsync(string id);
    }
}
