namespace MongoDBExample.Dto.Request;

public record DtoResetPassword(string Token, string Email, string NewPassword);