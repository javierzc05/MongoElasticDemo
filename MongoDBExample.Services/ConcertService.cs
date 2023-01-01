using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDBExample.Dto.Request;
using MongoDBExample.Dto.Response;
using MongoDBExample.Entities;
using MongoDBExample.Repositories.Interfaces;
using MongoDBExample.Services.Interfaces;
using Nest;

namespace MongoDBExample.Services;

public class ConcertService : IConcertService
{
    private readonly IConcertRepository _concertRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly ILogger<ConcertService> _logger;
    private readonly IMapper _mapper;
    private readonly IFileUploader _fileUploader;

    public ConcertService(IConcertRepository concertRepository, IGenreRepository genreRepository, ILogger<ConcertService> logger, IMapper mapper, IFileUploader fileUploader)
    {
        _concertRepository = concertRepository;
        _genreRepository = genreRepository;
        _logger = logger;
        _mapper = mapper;
        _fileUploader = fileUploader;
    }

    public async Task<BaseResponseGeneric<string>> AddAsync(ConcertDtoRequest request)
    {
        var response = new BaseResponseGeneric<string>();

        try{    

            // Gets the concert from the request
            var concert = _mapper.Map<Concert>(request);
            concert.Id = ObjectId.GenerateNewId().ToString();
            concert.ImageUrl = await _fileUploader.UploadFileAsync(request.Base64Image, request.FileName);

            // Gets the genres from the request and adds them to the concert
            concert.Genres = await _genreRepository.GetNamesAsync(request.Genres);

            // Adds the concert to the database
            await _concertRepository.AddAsync(concert);

            response.Data = concert.Id;
            response.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding the concert", ex);
            response.ErrorMessage = "Error adding the concert";
        }

        return response;
    }

    public async Task<BaseResponse> DeleteAsync(string id)
    {
        var response = new BaseResponse();

        try
        {
            await _concertRepository.DeleteAsync(id);
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = "Error while deleting the concert";
            _logger.LogError(ex, "Error while deleting the concert", ex.Message);
        }

        return response;
    }

    public async Task<BaseResponseGeneric<ICollection<ConcertDtoResponse>>> GetAsync(string? filter, int page, int rows)
    {
        var response = new BaseResponseGeneric<ICollection<ConcertDtoResponse>>();

        try
        {
            var concerts = await _concertRepository.GetAsync(filter, page, rows);

            response.Data = _mapper.Map<ICollection<ConcertDtoResponse>>(concerts);
            response.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing the concerts");
            response.ErrorMessage = "Error listing the concerts";
        }

        return response;
    }

    public async Task<BaseResponseGeneric<ICollection<ConcertDtoResponse>>> GetAsync(string? filter, string? beginDate, string? endDate, int page, int rows)
    {
        var response = new BaseResponseGeneric<ICollection<ConcertDtoResponse>>();

        try
        {
            var concerts = new List<Concert>();
            
            if (string.IsNullOrEmpty(beginDate) && string.IsNullOrEmpty(endDate))
            {
                concerts = (List<Concert>) await _concertRepository.GetAsync(filter, page, rows);
           
            } else {
                if (string.IsNullOrEmpty(beginDate)) 
                {
                    beginDate = "1900-01-01";
                }
                if (string.IsNullOrEmpty(endDate)) 
                {
                    endDate = "2099-12-31";
                }
                concerts = (List<Concert>) await _concertRepository.GetAsync(filter, beginDate, endDate, page, rows);
            }

            response.Data = _mapper.Map<ICollection<ConcertDtoResponse>>(concerts);
            response.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing the concerts");
            response.ErrorMessage = "Error listing the concerts";
        }

        return response;
    }

    public async Task<BaseResponseGeneric<ConcertDtoResponse>> GetAsync(string id)
    {
        var response = new BaseResponseGeneric<ConcertDtoResponse>();

            try
            {
                var concert = await _concertRepository.GetAsync(id);

                if (concert == null)
                {
                    response.Success = false;
                    response.ErrorMessage = "No concert was found";
                }
                else
                {
                    response.Data = _mapper.Map<ConcertDtoResponse>(concert);
                    response.Success = true;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = "There was an error getting the concert";

                _logger.LogError(ex, ex.Message);
            }

            return response;
    }

    public async Task<BaseResponse> UpdateAsync(string id, ConcertDtoRequest request)
    {
        var response = new BaseResponse();

        try
        {
            var concert = await _concertRepository.GetAsync(id);

            if (concert == null)
            {
                response.ErrorMessage = "No concert was found";
                return response;
            }
            
            _mapper.Map(request, concert);

            // Gets the genres from the request and adds them to the concert
            concert.Genres = await _genreRepository.GetNamesAsync(request.Genres);

            if (!string.IsNullOrEmpty(request.FileName))
                concert.ImageUrl = await _fileUploader.UploadFileAsync(request.Base64Image, request.FileName);

            await _concertRepository.UpdateAsync(id, concert);
            response.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while updating the concert");
            response.ErrorMessage = "Error while updating the concert";
        }

        return response;
    }
}