using RabbitMQ.Client;

namespace APIRestRabbitMQExchange.Services;

public interface IChannelQueueDeclare
{
    public void GetProcess(string orderJSON, string orderSource);
}
