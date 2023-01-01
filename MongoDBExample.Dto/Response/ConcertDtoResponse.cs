using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDBExample.Dto.Response;

public class ConcertDtoResponse
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Place { get; set; }
    public string DateEvent { get; set; } = null!;
    public string TimeEvent { get; set; } = null!;
    public List<string> Genres { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int TicketsQuantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string Status { get; set; } = null!;
}