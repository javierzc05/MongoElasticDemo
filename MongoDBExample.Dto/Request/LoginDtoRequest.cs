using System.ComponentModel.DataAnnotations;

namespace MongoDBExample.Dto.Request;

public class LoginDtoRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = default!;
    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = default!;
}