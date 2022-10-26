using TiendaVirtual.Common;

namespace TiendaVirtual.Ayudadores
{
    public interface IAyudaCorreo
    {
        Response SendMail(string toName, string toEmail, string subject, string body);
    }
}
