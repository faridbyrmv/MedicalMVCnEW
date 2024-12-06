namespace Medical.Mail;

public interface IMailService
{
    Task Send(string from, string to, string message, string subject);

}
