using APIRestRabbitMQ.Domain.Email;
using MailKit.Net.Smtp;
using MimeKit;

namespace APIRestRabbitMQExchange.Services;

public class EmailService 
{
    public void SendEmail(EmailMessage emailMessage)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(
            "Telefonía del Sur", 
            "clientes@telefoniadelsur.com.ar"));
        message.To.Add(new MailboxAddress(
            emailMessage.Para!.GetNombre(), 
            emailMessage.Para!.Value));
        message.Subject = emailMessage.Asunto!.Value;

        message.Body = new TextPart("html")
        {
            Text = emailMessage.Cuerpo!.Value
        };

        using (var client = new SmtpClient())
        {
            client.Connect("sandbox.smtp.mailtrap.io", 587, false);
            client.Authenticate("c6f43686b129d4", "54776cc8e1ec1d");
            client.Send(message);
            client.Disconnect(true);
        }
    }
}
