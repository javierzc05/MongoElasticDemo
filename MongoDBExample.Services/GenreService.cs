using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDBExample.Dto.Request;
using MongoDBExample.Dto.Response;
using MongoDBExample.Entities;
using MongoDBExample.Repositories.Interfaces;
using MongoDBExample.Services.Interfaces;

namespace MongoDBExample.Services
{
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _repository;
        private readonly ILogger<GenreService> _logger;
        private readonly IMapper _mapper;

        public GenreService(IGenreRepository repository, ILogger<GenreService> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<BaseResponseGeneric<string>> AddAsync(GenreDtoRequest request)
        {
            var response = new BaseResponseGeneric<string>();

            try
            {
                var genre = _mapper.Map<Genre>(request);
                
                await _repository.AddAsync(genre);

                response.Data = genre.Id;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.ErrorMessage = "There was an error adding the genre";
                _logger.LogError(ex, ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse> DeleteAsync(string id)
        {
            var response = new BaseResponse();

            try
            {
                var entity = await _repository.GetAsync(id);

                if (entity == null)
                {
                    response.Success = false;
                    response.ErrorMessage = "No genre was found";
                }
                else
                {
                    await _repository.DeleteAsync(id);
                    response.Success = true;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = "There was an error deleting the genre";

                _logger.LogError(ex, ex.Message);
            }

            return response;
        }

        public async Task<BaseResponseGeneric<GenreDtoResponse>> GetAsync(string id)
        {
            var response = new BaseResponseGeneric<GenreDtoResponse>();

            try
            {
                var genre = await _repository.GetAsync(id);

                if (genre == null)
                {
                    response.Success = false;
                    response.ErrorMessage = "No genre was found";
                }
                else
                {
                    response.Data = _mapper.Map<GenreDtoResponse>(genre);
                    response.Success = true;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = "There was an error getting the genre";

                _logger.LogError(ex, ex.Message);
            }

            return response;
        }

        public async Task<BaseResponseGeneric<IEnumerable<GenreDtoResponse>>> GetAsync()
        {
            var response = new BaseResponseGeneric<IEnumerable<GenreDtoResponse>>();
            try
            {
                var genres = await _repository.GetAsync();

                response.Data = _mapper.Map<IEnumerable<GenreDtoResponse>>(genres);
                response.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting genres");
                response.ErrorMessage = "Error getting genres";
            }

            return response;
        }

        public async Task<BaseResponse> UpdateAsync(string id, GenreDtoRequest request)
        {
            var response = new BaseResponse();
            
            try
            {
                var genre = await _repository.GetAsync(id);

                if (genre == null)
                {
                    response.Success = false;
                    response.ErrorMessage = "No genre was found";
                }
                else
                {
                    // origen y luego destino
                    _mapper.Map(request, genre);
                    await _repository.UpdateAsync(id, genre);
                    response.Success = true;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = "There was an error updating the genre";

                _logger.LogError(ex, ex.Message);
            }
            return response;
        }
    }
}
