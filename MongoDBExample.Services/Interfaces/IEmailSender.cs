namespace MongoDBExample.Services.Interfaces;

public interface IEmailSender
{
    Task SendEmailAsync(EmailMessageInfo message);
}   

public record EmailMessageInfo(string To, string ToName, string Subject, string Body);