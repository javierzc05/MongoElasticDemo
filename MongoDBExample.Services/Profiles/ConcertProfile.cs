using AutoMapper;
using MongoDBExample.Dto.Request;
using MongoDBExample.Dto.Response;
using MongoDBExample.Entities;

namespace MongoDBExample.Services.Profiles;

public class ConcertProfile : Profile
{
    public ConcertProfile()
    {

        // ESTE ES PARA POST Y PUT
        CreateMap<ConcertDtoRequest, Concert>()
            .ForMember(to => to.Genres, from => from.MapFrom(x => x.Genres))
            .ForMember(to => to.Title, from => from.MapFrom(x => x.Title))
            .ForMember(to => to.Description, from => from.MapFrom(x => x.Description))
            .ForMember(to => to.TicketsQuantity, from => from.MapFrom(x => x.TicketsQuantity))
            .ForMember(to => to.UnitPrice, from => from.MapFrom(x => x.UnitPrice))
            .ForMember(to => to.Place, from => from.MapFrom(x => x.Place))
            .ForMember(to => to.DateEvent, from => from.MapFrom(x => DateTime.Parse($"{x.DateEvent} {x.TimeEvent}")));
        
        // GET CON PAGINACION
        // CreateMap<ConcertInfo, ConcertDtoResponse>();

        CreateMap<Concert, ConcertDtoResponse>()
            .ForMember(to => to.Id, from => from.MapFrom(x => x.Id))
            .ForMember(to => to.Genres, from => from.MapFrom(x => x.Genres))
            .ForMember(to => to.Title, from => from.MapFrom(x => x.Title))
            .ForMember(to => to.Description, from => from.MapFrom(x => x.Description))
            .ForMember(to => to.TicketsQuantity, from => from.MapFrom(x => x.TicketsQuantity))
            .ForMember(to => to.UnitPrice, from => from.MapFrom(x => x.UnitPrice))
            .ForMember(to => to.Place, from => from.MapFrom(x => x.Place))
            .ForMember(to => to.DateEvent, from => from.MapFrom(x => x.DateEvent.ToString("yyyy-MM-dd")))
            .ForMember(to => to.TimeEvent, from => from.MapFrom(x => x.DateEvent.ToString("HH:mm:ss")));

    }
}