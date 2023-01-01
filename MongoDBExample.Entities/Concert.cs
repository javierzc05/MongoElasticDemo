using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace MongoDBExample.Entities;

public class Concert : EntityBase
{
    public List<string> Genres { get; set; } = default!;
    
    public string Title { get; set; } = default!;
    
    [StringLength(500)]
    public string Description { get; set; } = default!;
    
    public DateTime DateEvent { get; set; }
    
    public string? ImageUrl { get; set; }
    
    [StringLength(100)]
    public string? Place { get; set; }
    
    public int TicketsQuantity { get; set; }
    
    public decimal UnitPrice { get; set; }
    
    public bool Finalized { get; set; }
}
