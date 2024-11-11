namespace APIRestRabbitMQ.Domain.Email;

public interface IEmailMessage
{
    public void SendEmail(EmailMessage emailMessage);
}
