using AutoMapper;
using MongoDBExample.Dto.Request;
using MongoDBExample.Dto.Response;
using MongoDBExample.Entities;

namespace MongoDBExample.Services.Profiles;

public class GenreProfile : Profile
{
    public GenreProfile()
    {
        CreateMap<Genre, GenreDtoResponse>();
        CreateMap<GenreDtoRequest, Genre>();
        // Esto es para hacerlo al reves.ReverseMap();
    }
}