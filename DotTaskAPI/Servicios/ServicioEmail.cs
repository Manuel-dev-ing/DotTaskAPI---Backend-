using System.Net;
using System.Net.Mail;

namespace DotTaskAPI.Servicios
{
    public interface IServicioEmail
    {
        Task enviarEmailGmail(string emailReceptor, string asunto, string cuerpo);
    }


    public class ServicioEmail: IServicioEmail
    {
        private readonly IConfiguration configuration;

        public ServicioEmail(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task enviarEmailGmail(string emailReceptor, string asunto, string cuerpo)
        {
            var emailEmisor = configuration.GetValue<string>("CONFIGURACIONES_EMAIL:EMAIL");
            var password = configuration.GetValue<string>("CONFIGURACIONES_EMAIL:PASSWORD");
            var host = configuration.GetValue<string>("CONFIGURACIONES_EMAIL:HOST");
            var puerto = configuration.GetValue<int>("CONFIGURACIONES_EMAIL:PUERTO");


            var smtpCliente = new SmtpClient(host, puerto);
            smtpCliente.EnableSsl = true;
            smtpCliente.UseDefaultCredentials = false;


            smtpCliente.Credentials = new NetworkCredential(emailEmisor, password);
            var mensaje = new MailMessage
            {
                From = new MailAddress(emailEmisor!),
                Subject = asunto,
                Body = cuerpo,
                IsBodyHtml = true
            };
            mensaje.To.Add(emailReceptor);
            await smtpCliente.SendMailAsync(mensaje);

        }


        

    }
}
