namespace APIRestRabbitMQExchange.Services;

public static class Invoice
{
    public static string EnvoiceTemplate(Dictionary<string, string> dictionary)
    {
        string html = string.Empty;
        html += String.Format("<h4>Factura N°: {0}</h4>", dictionary["@@NroFactura@@"]);
        html += "<br />";
        html += String.Format("<p><b>Fecha:</b> {0}</p>", DateTime.Now.ToShortDateString());
        html += String.Format("<p><b>Cliente:</b> {0}</p>", dictionary["@@NombreApellido@@"]);
        html += String.Format("<p><b>Email:</b> {0}</p>", dictionary["@@Email@@"]);
        html += "<br />";
        html += "<table>";
        html += "<tr>";
        html += String.Format("<td><b>Medio Pago:</b></td><td>{0}</td>", dictionary["@@MedioPago@@"]);
        html += "</tr><tr>";
        html += String.Format("<td><b>Código:</b></td><td>{0}</td>", dictionary["@@NroFactura@@"]);
        html += "</tr><tr>";
        html += String.Format("<td><b>Detalle:</b></td><td>{0}</td>", dictionary["@@Detalle@@"]);
        html += "</tr>";
        html += "<tr>";
        html += String.Format("<td><b>Alícuota IVA: </b></td><td>% {0:F2}</td>", Convert.ToDecimal(dictionary["@@Moneda.IVA.Porcentaje@@"]));
        html += "</tr><tr>";
        html += String.Format("<td><b>IVA: </b></td><td>$ {0:F2}</td>", Convert.ToDecimal(dictionary["@@Moneda.IVA@@"]));
        html += "</tr><tr>";
        html += String.Format("<td><b>Total: </b></td><td>$ {0:F2}</td>", Convert.ToDecimal(dictionary["@@Moneda.Total@@"]));
        html += "</tr>";
        html += "</table>";
        html += "<p style='color: blue'><b>Factura Paga</b></p>";
        html += "<br />";
        html += "<p>Recuerde que cuando la factura está vencida, deberá pagar un 3% más de su valor.</p>";
        return html;
    }
}
