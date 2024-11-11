namespace APIRestRabbitMQExchange.Domain.Facturas;

public class Factura
{
    public Codigo? Codigo { get; set; }
    public Cliente? Cliente { get; set; }
    public Email? Email { get; set; }
    public MedioPago? MedioPago { get; set; }
    public Detalle? Detalle { get; set; }
    public Moneda? Moneda { get; set; }
}
