namespace APIRestRabbitMQ.Domain.Email;

public class EmailMessage
{
    public Para? Para { get; set; }
    public Asunto? Asunto { get; set; }
    public Cuerpo? Cuerpo { get; set; }
}
