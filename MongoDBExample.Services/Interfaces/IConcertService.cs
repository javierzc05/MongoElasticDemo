using MongoDBExample.Dto.Request;
using MongoDBExample.Dto.Response;

namespace MongoDBExample.Services.Interfaces
{
    public interface IConcertService
    {
        Task<BaseResponseGeneric<ICollection<ConcertDtoResponse>>> GetAsync(string? filter, int page, int rows);

        Task<BaseResponseGeneric<ConcertDtoResponse>> GetAsync(string id);

        Task<BaseResponseGeneric<string>> AddAsync(ConcertDtoRequest concert);
        
        Task<BaseResponse> UpdateAsync(string id, ConcertDtoRequest concert);

        Task<BaseResponse> DeleteAsync(string id);
    }
}
