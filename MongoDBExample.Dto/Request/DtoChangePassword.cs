namespace MongoDBExample.Dto.Request;

public record DtoChangePassword(string Email, string OldPassword, string NewPassword);