namespace APIRestRabbitMQ.Domain.Email;

public record Para(string Value)
{
    public string GetNombre() => Value.Split('@')[0];
}
