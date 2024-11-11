namespace APIRestRabbitMQExchange.Domain.Facturas;

public record Moneda(decimal Value)
{
    public decimal GetAlicuota() => 0.21M;
    public decimal GetIVA() => Value * 0.21M;
    public decimal GetTotal() => Value + (Value * 0.21M);
}
