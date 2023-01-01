namespace MongoDBExample.Dto.Response;

public class LoginDtoResponse : BaseResponse
{
    public string Token { get; set; } = default!;
    public string FullName { get; set; } = default!;
}