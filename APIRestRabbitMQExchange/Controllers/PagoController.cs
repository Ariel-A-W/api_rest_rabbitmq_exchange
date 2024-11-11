using APIRestRabbitMQExchange.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using APIRestRabbitMQExchange.Application.Requests;

namespace APIRestRabbitMQExchange.Controllers;

[ApiController]
[Route("[controller]")]
public class PagoController : ControllerBase
{
    private readonly IChannelQueueDeclare _channelQueueDeclare;

    public PagoController(
        IChannelQueueDeclare channelQueueDeclare
    )
    {
        _channelQueueDeclare = channelQueueDeclare;
    }

    [HttpPost("/pagoWeb")]
    public string PayWeb(
        [FromQuery] string cliente, string email
    )
    {
        var sources = Sources.WEB.ToString().ToLower();

        var person = new ClienteRequest
        {
            nombre_apellido = cliente, 
            email = email, 
            dispositivo = sources
        };
        string facturaJSON = JsonConvert.SerializeObject(person);

        _channelQueueDeclare.GetProcess(facturaJSON, "web");

        return "Factura Paga.";
    }

    [HttpPost("/pagoMobile")]
    public string PayMobile(
        [FromQuery] string cliente, string email
    )
    {
        var sources = Sources.MOBILE.ToString().ToLower();

        var person = new ClienteRequest
        {
            nombre_apellido = cliente,
            email = email,
            dispositivo = sources
        };
        string facturaJSON = JsonConvert.SerializeObject(person);

        _channelQueueDeclare.GetProcess(facturaJSON, "mobile");

        return "Factura Paga.";
    }

    [HttpPost("/pagoOtro")]
    public string PayOther(
        [FromQuery] string cliente, string email
    )
    {
        var sources = Sources.THIRD_PARTY.ToString().ToLower();

        var person = new ClienteRequest
        {
            nombre_apellido = cliente,
            email = email,
            dispositivo = sources
        };
        string facturaJSON = JsonConvert.SerializeObject(person);

        _channelQueueDeclare.GetProcess(facturaJSON, "thirdparty");

        return "Factura Paga.";
    }
}
