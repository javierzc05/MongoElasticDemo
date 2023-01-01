using System.ComponentModel.DataAnnotations;

namespace MongoDBExample.Dto.Request;

public class RegisterUserDtoRequest
{
    [Required]
    public string FirstName { get; set; } = default!;

    [Required]
    public string LastName { get; set; } = default!;

    [Required, EmailAddress]
    public string Email { get; set; } = default!;
    
    public int Age { get; set; }

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = default!;

    [Required, DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = "Passwords don't match")]
    public string ConfirmPassword { get; set; } = default!;
}