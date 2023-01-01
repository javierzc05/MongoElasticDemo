using System.ComponentModel.DataAnnotations;

namespace MongoDBExample.Dto.Request
{
    public class ConcertDtoRequest
{
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;

    public List<string> Genres { get; set; } = null!;

    [Required]
    // format date yyyy-mm-dd
    public string DateEvent { get; set; } = null!;

    [Required]
    // Regular expression for time HH:MM:ss
    public string TimeEvent { get; set; } = null!;

    // Esto hara que se ignore el campo en el JSON  [JsonIgnore] 
    public string Place { get; set; } = default!;

    public string? Base64Image { get; set; }

    public string? FileName { get; set; }

    public decimal UnitPrice { get; set; }

    [Range(1, 9999, ErrorMessage = "Value must be between 1 and 9999")]
    public int TicketsQuantity { get; set; }
}
}
