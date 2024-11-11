using APIRestRabbitMQ.Domain.Email;
using APIRestRabbitMQExchange.Domain.Facturas;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace APIRestRabbitMQExchange.Services;

public class ChannelQueueDeclare : IChannelQueueDeclare
{

    public void GetProcess(string orderJSON, string orderSource)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        // Declaración para el Exchange.
        channel.ExchangeDeclare(exchange: "OrderExchange", type: ExchangeType.Direct);

        // Declaración de Colas.
        channel.QueueDeclare(queue: "WebOrderQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);
        channel.QueueDeclare(queue: "MobileOrderQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);
        channel.QueueDeclare(queue: "ThirdPartyOrderQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);

        // Enlace de las Colas para el Exchange con las rutas claves.
        channel.QueueBind(queue: "WebOrderQueue", exchange: "OrderExchange", routingKey: "web");
        channel.QueueBind(queue: "MobileOrderQueue", exchange: "OrderExchange", routingKey: "mobile");
        channel.QueueBind(queue: "ThirdPartyOrderQueue", exchange: "OrderExchange", routingKey: "thirdparty");

        // Procesos del Publicador.
        var body = Encoding.UTF8.GetBytes(orderJSON);

        var routingKey = orderSource.ToString().ToLower() switch
        {
            "web" => "web",
            "mobile" => "mobile",
            "thirdparty" => "thirdparty",
            _ => throw new ArgumentException("Orden invalidada.")
        };

        channel.BasicPublish(
            exchange: "OrderExchange",
            routingKey: routingKey,
            basicProperties: null,
            body: body
        );

        // Procesos del Consumidor.
        var queueName = orderSource.ToString().ToLower() switch
        {
            "web" => "WebOrderQueue",
            "mobile" => "MobileOrderQueue",
            "thirdparty" => "ThirdPartyOrderQueue",
            _ => throw new ArgumentException("Orden invalidada.")
        };

        var lst = new List<Factura>();
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var eaJSON = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(eaJSON);
            var cta = JsonSerializer.Deserialize<Domain.Clientes.Cliente>(message);
            lst.Add(
                new Factura()
                {
                    Codigo = new Codigo(Guid.NewGuid().ToString()),
                    Cliente = new Domain.Facturas.Cliente(cta!.nombre_apellido!),
                    Email = new Domain.Facturas.Email(cta!.email!),
                    MedioPago = new MedioPago(cta!.dispositivo!),
                    Detalle = new Detalle("Factura Mensual del Servicio de Telefonía."),
                    Moneda = new Moneda(36000.00M)
                }
            );

            // Enviar el correo email.
            SendEmailOrder(lst);
        };

        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
    }

    private void SendEmailOrder(List<Factura> factura)
    {
        // Enviar la factura al Cliente vía email.
        var email = new EmailService();
        var es = new EmailMessage();
        var dictionary = new Dictionary<String, String>();
        foreach (var item in factura)
        {
            es.Para = new Para(item.Email!.Value);
            var asunto = item.Codigo!.Value; 
            es.Asunto = new Asunto(String.Format("Factura N°: {0}", asunto.Substring(1, 7).ToUpper()));

            dictionary.Add("@@Email@@", item.Email!.Value);
            dictionary.Add("@@NroFactura@@", asunto.Substring(1, 7).ToUpper());
            dictionary.Add("@@MedioPago@@", item.MedioPago!.Value.ToUpper());
            dictionary.Add("@@NombreApellido@@", item.Cliente!.Value);
            dictionary.Add("@@Detalle@@", item.Detalle!.Value);
            dictionary.Add("@@Moneda.IVA.Porcentaje@@", item.Moneda!.GetAlicuota().ToString());
            dictionary.Add("@@Moneda.IVA@@", item.Moneda!.GetIVA().ToString());
            dictionary.Add("@@Moneda.Total@@", item.Moneda!.GetTotal().ToString());

            es.Cuerpo = new Cuerpo(Invoice.EnvoiceTemplate(dictionary));
        }
        email.SendEmail(es);
    }
}
